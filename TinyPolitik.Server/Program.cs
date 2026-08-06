using Microsoft.AspNetCore.Mvc;
using PolitikServer.Core;

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

builder.Services.AddSingleton(gameConfig);

builder.Services.AddSingleton(new LoginRateLimiter());
builder.Services.AddSingleton(new SessionStore());
builder.Services.AddSingleton(new EntityLibrary());
builder.Services.AddSingleton(new GameDefinitionLibrary(contentRoot));

ContentLoader.Setup(builder);
CertificateLoader.Setup(builder, gameConfig);

var app = builder.Build();

// PUBLIC ROUTES:
app.MapPost("/login", 
    (HttpContext context, 
    [FromBody] LoginRequest req, 
    [FromServices] SessionStore sessions,
    [FromServices] LoginRateLimiter limiter,
    [FromServices] GameConfig config ) => LoginHandler.Login(context, req, sessions, limiter, config));


// Temporarily removing this because it might not be necessary:
// Getting content version (I.e Game Version)
// app.MapGet("/content/version", ([FromServices] EntityLibrary contentLib, [FromServices] GameDefinitionLibrary defLib) => ServerInfo.Get(contentLib, defLib));

// Getting the world version (Map/Balacing updates)
app.MapGet("/world/version", ([FromServices] GameDefinitionLibrary lib) => Results.Json(new {worldVersion = lib.VersionHash}));

// PRIVATE ROUTES:
// These require an authentication token
var authed = app.MapGroup("").AddEndpointFilter<RequireSessionFilter>();

// Getting game data:
authed.MapGet("/world/data", ([FromServices] GameDefinitionLibrary lib) => Results.Text(lib.GetAllGameDefinitionsAsJson(), "application/json"));
authed.MapGet("/gamestate/data", ([FromServices] EntityLibrary lib) => Results.Text(lib.GetAllEntitiesAsJson(), "application/json"));

// --- End routes

CertificateLoader.NotifyInConsole();

app.Run();

