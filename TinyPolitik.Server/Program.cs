using Microsoft.AspNetCore.Mvc;
using PolitikServer.Core;
using TinyPolitik.Core;

var builder = WebApplication.CreateBuilder(args);

var dataRoot = Path.Combine(builder.Environment.ContentRootPath, "gamedata");
Directory.CreateDirectory(dataRoot);

var contentRoot = Path.Combine(builder.Environment.ContentRootPath, "gameworld");
Directory.CreateDirectory(contentRoot);

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

// Setup networking:
builder.Services.AddSingleton(gameConfig);
builder.Services.AddSingleton(new LoginRateLimiter());
builder.Services.AddSingleton(new SessionStore());
ContentLoader.Setup(builder);
CertificateLoader.Setup(builder, gameConfig);

// Setup world:
builder.Services.AddSingleton(new DefinitionLibrary(contentRoot));
builder.Services.AddSingleton(new EntityLibrary());
builder.Services.AddSingleton(new GameStateInitialiser());
builder.Services.AddSingleton(new TurnManager(gameConfig));

// Setup turn management
builder.Services.AddSingleton(new TurnResolver());
builder.Services.AddSingleton(new TurnBackup());
builder.Services.AddHostedService<TurnSchedulerService>(); // Turn scheduler

var app = builder.Build();

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

