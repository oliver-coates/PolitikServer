using Microsoft.AspNetCore.Mvc;
using PolitikServer.Core;

var builder = WebApplication.CreateBuilder(args);

// Paths:
var logsRoot = Path.Combine(builder.Environment.ContentRootPath, "logs");
var dataRoot = Path.Combine(builder.Environment.ContentRootPath, "gamedata");
var contentRoot = Path.Combine(builder.Environment.ContentRootPath, "gameworld");
var backupsRoot = Path.Combine(builder.Environment.ContentRootPath, "snapshots");

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

// Setup world:
builder.Services.AddSingleton(new DefinitionLibrary(contentRoot));
builder.Services.AddSingleton<EntityLibrary>();
builder.Services.AddSingleton<GameStateInitialiser>();
builder.Services.AddSingleton<TurnManager>();

// Setup turn management
builder.Services.AddSingleton<TurnResolver>();
builder.Services.AddSingleton<TurnBackupManager>();
builder.Services.AddHostedService<TurnSchedulerService>();

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
app.MapPost("/login", 
    (HttpContext context, 
    [FromBody] LoginRequest req, 
    [FromServices] SessionStore sessions,
    [FromServices] LoginRateLimiter limiter,
    [FromServices] GameConfig config ) => LoginHandler.Login(context, req, sessions, limiter, config));


// Getting content version (Game Version, Game Definition Version, etc)
app.MapGet("/server/version", ([FromServices] DefinitionLibrary definitionLibrary) => ServerInfo.Get(definitionLibrary));

// PRIVATE ROUTES:
// These require an authentication token
var authed = app.MapGroup("").AddEndpointFilter<RequireSessionFilter>();

// Getting game data:
authed.MapGet("/world/data", ([FromServices] DefinitionLibrary lib) => Results.Text(lib.GetAllGameDefinitionsAsJson(), "application/json"));
authed.MapGet("/gamestate/data", ([FromServices] EntityLibrary lib) => Results.Text(lib.GetAllEntitiesAsJson(), "application/json"));

// --- End routes

CertificateLoader.NotifyInConsole();

var test = app.Services.GetService<EntityLibrary>()?.GetAllEntitiesAsJson();


app.Run();

