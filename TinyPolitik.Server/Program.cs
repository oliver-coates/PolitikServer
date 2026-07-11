using Microsoft.AspNetCore.Mvc;
using TinyPolitik.Core;

var builder = WebApplication.CreateBuilder(args);

var dataRoot = Path.Combine(builder.Environment.ContentRootPath, "gamedata");
Directory.CreateDirectory(dataRoot);

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

ContentLoader.Setup(builder);
CertificateLoader.Setup(builder, gameConfig);

var app = builder.Build();

app.MapPost("/login", 
    (HttpContext context, 
    [FromBody] LoginRequest req, 
    [FromServices] SessionStore sessions,
    [FromServices] LoginRateLimiter limiter,
    [FromServices] GameConfig config ) => LoginHandler.Login(context, req, sessions, limiter, config));

 
app.MapGet("/content/version", (ContentLibrary lib) => ServerInfo.Get(lib));


app.MapGet("/content/definitions/strategicResources", (ContentLibrary lib) => Results.Json(lib.StrategicResources.Values));

CertificateLoader.NotifyInConsole();

app.Run();
