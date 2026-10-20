using Microsoft.AspNetCore.Mvc;
using PolitikServer.Core;
using PolitikServer.Core.Serialization;

var builder = WebApplication.CreateBuilder(args);

ApplicationPaths.InitialiseDirectories(builder.Environment.ContentRootPath);

ServerConfig serverConfig = new();
try
{
    serverConfig = ServerConfigLoader.LoadOrCreate(ApplicationPaths.ServerConfigPath);
}
catch (Exception e)
{
    Console.WriteLine("Error while establishing game configuration:");
    Console.WriteLine($"{e.Message}");

    Environment.Exit(0);
}

// Setup logging:
var logWriter = new LogWriter(ApplicationPaths.LogsRoot);
builder.Services.AddSingleton(logWriter);
builder.Logging.AddProvider(new TurnFileLoggerProvider(logWriter));

// Setup networking:
builder.Services.AddSingleton(serverConfig);
builder.Services.AddSingleton<LoginRateLimiter>();
builder.Services.AddSingleton<SessionStore>();
builder.Services.AddSingleton<AccountStore>();
builder.Services.AddSingleton<AccountManager>();
builder.Services.AddSingleton(new CertificateLoader(builder, serverConfig));

// Setup world:
builder.Services.AddSingleton<DefinitionValidator>();
builder.Services.AddSingleton<DefinitionLoader>();
builder.Services.AddSingleton<DefinitionLibrary>();
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
if (serverConfig.AllowRealTimePlay)
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

// Build the app:
var app = builder.Build();

// --- Initialise everything:

// Validate then initialise game world:
try
{
    app.Services.GetRequiredService<DefinitionValidator>().ValidateGameWorld();
}
catch (Exception e)
{
    Console.WriteLine($"Could not locate game world file. Ensure you have a valid TinyPolitik game world located at '{ApplicationPaths.ContentRoot}'.");
    Console.WriteLine(e);

    Environment.Exit(0);
}

try
{
    app.Services.GetRequiredService<DefinitionLoader>().Load();
}
catch (Exception e)
{
    Console.WriteLine("Error while loading game world:");
    Console.WriteLine(e);

    Environment.Exit(0);
}

// Initialise the game state
bool doInitialiseGame = true; // Eventaully we will want to be loading from an existing save (in cases of power outage, etc), for now always initialise as though a new server
if (doInitialiseGame)
{ 
    app.Services.GetRequiredService<GameStateInitialiser>().Initialise();
    app.Services.GetRequiredService<TurnManager>().Initialise();   

    // Make a backup of the world (this will move somewhere else eventually)
    string worldJson = app.Services.GetRequiredService<DefinitionLibrary>().ToJson();
    app.Services.GetRequiredService<TurnBackupManager>().MakeWorldBackup(worldJson);
}

// Finally load our accounts
app.Services.GetRequiredService<AccountStore>().TryLoad();

// PUBLIC ROUTES:
// Getting content version (Game Version, Game Definition Version, etc)
app.MapGet("/server/version", ([FromServices] DefinitionLibrary definitionLibrary, [FromServices] ServerConfig config) => ServerInfo.Get(definitionLibrary, config));

app.MapPost("/account/login", 
    (HttpContext ctx, AccountLoginRequest request, [FromServices] AccountManager accountManager) => accountManager.Login(ctx, request));

app.MapPost("/account/register", 
    (HttpContext ctx, AccountRegisterRequest request, [FromServices] AccountManager accountManager) => accountManager.RegisterAccount(ctx, request));


// PRIVATE ROUTES:
// These require an authentication token
var authed = app.MapGroup("").AddEndpointFilter<RequireSessionFilter>();

// Getting game data:
// If we want to speed this up we could cache the below data - to prevent having to serialize every endpoint call.
authed.MapGet("/world/data", ([FromServices] DefinitionLibrary lib) => Results.Text(lib.ToJson(), "application/json"));
authed.MapGet("/gamestate/data", ([FromServices] EntityLibrary lib) => Results.Text(lib.ToJson(), "application/json"));

// Readying your nation - only allowed in real-time-play
if (serverConfig.AllowRealTimePlay)
{
    authed.MapPost("/turn/ready", async (HttpContext context,
                                    [FromServices] TurnRealTimePlayManager rtpManager,
                                    [FromServices] EntityLibrary entityLibrary,
                                    [FromServices] ServerConfig config,
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
            case NationClaimManager.ClaimAttemptResult.NationNotFound:
                return Results.NotFound($"Nation '{nationId}' does not exist.");
            
            case NationClaimManager.ClaimAttemptResult.PlayerNotFound:
                return Results.NotFound($"Player '{session.PlayerId}' does not exist.");
            
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

authed.MapPost("/nations/{nationId}/abandon",
    (HttpContext ctx, string nationId, [FromServices] NationClaimManager claimManager) =>
    {
        var session = ctx.GetSession();
        var result = claimManager.TryAbandonNation(session.PlayerId, nationId);

        switch (result)
        {
            case NationClaimManager.AbandonAttemptResult.NationNotFound:
                return Results.NotFound($"Nation '{nationId}' does not exist.");
            
            case NationClaimManager.AbandonAttemptResult.PlayerNotFound:
                return Results.NotFound($"Player '{session.PlayerId}' does not exist.");
        
            case NationClaimManager.AbandonAttemptResult.DoesNotControlNation:
                return Results.BadRequest($"You do not control nation '{nationId}'.");
            
            case NationClaimManager.AbandonAttemptResult.Success:
                return Results.Accepted("You have abandoned your country.");
            
            default:
                throw new Exception("Unhandled abandon request.");
        }
    });

// --- End routes

app.Services.GetRequiredService<CertificateLoader>().NotifyInConsole();

var testEntities = app.Services.GetService<EntityLibrary>()?.ToJson();
var testDefinitions = app.Services.GetService<DefinitionLibrary>()?.ToJson();

app.Run();
