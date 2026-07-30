using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
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



builder.Services.AddSingleton(gameConfig);

builder.Services.AddSingleton(new LoginRateLimiter());
builder.Services.AddSingleton(new SessionStore());
builder.Services.AddSingleton(new ContentLibrary(contentRoot));
builder.Services.AddSingleton(new WorldDataLibrary(contentRoot));

ContentLoader.Setup(builder);
CertificateLoader.Setup(builder, gameConfig);

var app = builder.Build();

app.MapPost("/login", 
    (HttpContext context, 
    [FromBody] LoginRequest req, 
    [FromServices] SessionStore sessions,
    [FromServices] LoginRateLimiter limiter,
    [FromServices] GameConfig config ) => LoginHandler.Login(context, req, sessions, limiter, config));


// Getting content version (I.e Game Version)
app.MapGet("/content/version", ([FromServices] ContentLibrary lib) => ServerInfo.Get(lib));
// Getting the world version (Map/Balacing updates)
app.MapGet("/world/version", ([FromServices] WorldDataLibrary lib) => Results.Json(new {worldVersion = lib.VersionHash}));
// Getting the entire world:
app.MapGet("/world/data", ([FromServices] WorldDataLibrary lib) =>
{
    var root = new JsonObject
    {
        ["worldVersion"] = lib.VersionHash,
        ["worldMeta"] = JsonNode.Parse(lib.WorldJson)
    };
    
    foreach (KeyValuePair<string, IReadOnlyDictionary<string, string>> definitions in lib.ContentJson)
    {
        string type = definitions.Key;
        JsonArray jsonArray = new (definitions.Value.OrderBy(k => k.Key).Select(kv => JsonNode.Parse(kv.Value)).ToArray());
        root.Add(type, jsonArray);
    }

    return Results.Text(root.ToJsonString(), "application/json");
});

CertificateLoader.NotifyInConsole();

app.Run();
