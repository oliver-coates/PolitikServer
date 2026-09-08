using Microsoft.AspNetCore.Mvc;
using PolitikServer.Core;

var builder = WebApplication.CreateBuilder(args);

// Paths:
var logsRoot = Path.Combine(builder.Environment.ContentRootPath, "gamedata", "logs");
var dataRoot = Path.Combine(builder.Environment.ContentRootPath, "gamedata");
var contentRoot = Path.Combine(builder.Environment.ContentRootPath, "gamedata", "gameworld");
var backupsRoot = Path.Combine(builder.Environment.ContentRootPath, "gamedata", "snapshots");
var accountsPath = Path.Combine(builder.Environment.ContentRootPath, "gamedata", "accounts.json");

// Initialising Directories:
Directory.CreateDirectory(contentRoot);
Directory.CreateDirectory(dataRoot);
Directory.CreateDirectory(logsRoot);

GameConfig gameConfig = new();
try
{
    gameConfig = GameConfigLoader.LoadOrCreate(Path.Combine(dataRoot, "config.json"));
}
catch (Exception e)
{
    Console.WriteLine("Error while establishing game configuration:");
    Console.WriteLine($"{e.Message}");

    Environment.Exit(0);
}

// Setup logging:
var logWriter = new LogWriter(logsRoot);
builder.Services.AddSingleton(logWriter);
builder.Logging.AddProvider(new TurnFileLoggerProvider(logWriter));

// Setup networking:
builder.Services.AddSingleton(gameConfig);
builder.Services.AddSingleton<LoginRateLimiter>();
builder.Services.AddSingleton<SessionStore>();
builder.Services.AddSingleton(new AccountStore(accountsPath));
builder.Services.AddSingleton<AccountManager>();

// Setup world:
builder.Services.AddSingleton(new DefinitionLibrary(contentRoot));
builder.Services.AddSingleton<EntityLibrary>();
builder.Services.AddSingleton<GameStateInitialiser>();
builder.Services.AddSingleton<TurnManager>();
builder.Services.AddSingleton<NationClaimManager>();

// Setup turn resolution:
builder.Services.AddSingleton<ProvinceTurnResolver>();
builder.Services.AddSingleton<TurnResolver>();

// Setup turn management
builder.Services.AddSingleton<TurnProcessor>();
builder.Services.AddSingleton<TurnBackupManager>();

// Turn Scheduling:
if (gameConfig.AllowRealTimePlay)
{
    // Real time play
    builder.Services.AddSingleton<TurnRealTimePlayManager>();
}
else
{
    // Standard turn-based play
    builder.Services.AddHostedService<TurnSchedulerService>();
}

// Setup commands:
builder.Services.AddHostedService<ServerCommandService>();

CertificateLoader.Setup(builder, gameConfig); // Make not static

var app = builder.Build();

// Initialise everything:
app.Services.GetRequiredService<TurnBackupManager>().Initialise(backupsRoot);

bool doInitialiseGame = true; // Eventaully we will want to be loading from an existing save, for now always initialise as though a new server
if (doInitialiseGame)
{ 
    app.Services.GetRequiredService<GameStateInitialiser>().Initialise();
    app.Services.GetRequiredService<TurnManager>().Initialise();   

    // Make a backup of the world (this will move somewhere else eventually)
    string worldJson = app.Services.GetRequiredService<DefinitionLibrary>().GetAllGameDefinitionsAsJson();
    app.Services.GetRequiredService<TurnBackupManager>().MakeWorldBackup(worldJson);
}


// PUBLIC ROUTES:
// Getting content version (Game Version, Game Definition Version, etc)
app.MapGet("/server/version", ([FromServices] DefinitionLibrary definitionLibrary, [FromServices] GameConfig config) => ServerInfo.Get(definitionLibrary, config));

app.MapPost("/account/login", 
    (HttpContext ctx, AccountLoginRequest request, [FromServices] AccountManager accountManager) => accountManager.Login(ctx, request));

app.MapPost("/account/register", 
    (HttpContext ctx, AccountRegisterRequest request, [FromServices] AccountManager accountManager) => accountManager.RegisterAccount(ctx, request));


// PRIVATE ROUTES:
// These require an authentication token
var authed = app.MapGroup("").AddEndpointFilter<RequireSessionFilter>();

// Account creation:


// Getting game data:
authed.MapGet("/world/data", ([FromServices] DefinitionLibrary lib) => Results.Text(lib.GetAllGameDefinitionsAsJson(), "application/json"));
authed.MapGet("/gamestate/data", ([FromServices] EntityLibrary lib) => Results.Text(lib.GetAllEntitiesAsJson(), "application/json"));

// Readying your nation - only allowed in real-time-play
if (gameConfig.AllowRealTimePlay)
{
    authed.MapPost("/turn/ready", async (HttpContext context,
                                    [FromServices] TurnRealTimePlayManager rtpManager,
                                    [FromServices] EntityLibrary entityLibrary,
                                    [FromServices] GameConfig config,
                                    [FromServices] TurnProcessor turnProcessor) => await rtpManager.ReadyNation(context, entityLibrary, config, turnProcessor));

    authed.MapGet("/turn/readiness", ([FromServices] EntityLibrary entities, [FromServices] TurnRealTimePlayManager rtpManager)
                                        => rtpManager.GetReadiness(entities) );
}

authed.MapGet("/nations/unclaimed", ([FromServices] NationClaimManager claimManager) => 
                                        Results.Json(new 
                                        { 
                                            unclaimedNations = claimManager.GetAllUnclaimedNationIds() 
                                        }));

authed.MapPost("/nations/{nationId}/claim", 
    (HttpContext ctx, string nationId, [FromServices] NationClaimManager claimManager) =>
    {
        var session = ctx.GetSession();
        var result = claimManager.TryClaimNation(session.PlayerId, nationId);

        switch (result)
        {
            case NationClaimManager.ClaimAttemptResult.NotFound:
                return Results.NotFound($"Nation '{nationId}' does not exist.");
            
            case NationClaimManager.ClaimAttemptResult.AlreadyClaimed:
                return Results.Conflict($"This nation has already been claimed :(");
            
            case NationClaimManager.ClaimAttemptResult.ThisPlayerAlreadyHasNation:
                return Results.BadRequest($"You already control a nation.");
            
            case NationClaimManager.ClaimAttemptResult.Success:
                return Results.Accepted($"Welcome, {claimManager.GetRandomCountryLeaderTitle()}.");

            default:
                throw new Exception($"Unhanded Claim Request!");
        }
    });

// --- End routes

CertificateLoader.NotifyInConsole();

var test = app.Services.GetService<EntityLibrary>()?.GetAllEntitiesAsJson();

app.Run();

