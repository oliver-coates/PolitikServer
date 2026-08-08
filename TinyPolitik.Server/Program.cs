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

// Getting the world version (Changes as the Map/defintions change)
app.MapGet("/world/version", ([FromServices] DefinitionLibrary lib) => Results.Json(new {worldVersion = lib.VersionHash}));

// PRIVATE ROUTES:
// These require an authentication token
var authed = app.MapGroup("").AddEndpointFilter<RequireSessionFilter>();

// Getting game data:
authed.MapGet("/world/data", ([FromServices] DefinitionLibrary lib) => Results.Text(lib.GetAllGameDefinitionsAsJson(), "application/json"));
authed.MapGet("/gamestate/data", ([FromServices] EntityLibrary lib) => Results.Text(lib.GetAllEntitiesAsJson(), "application/json"));

// --- End routes

CertificateLoader.NotifyInConsole();

for (int i = 0; i < 50; i++)
{
    Console.WriteLine(RandomCountryNameGenerator.Generate());
}

app.Run();

