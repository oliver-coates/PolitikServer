using Microsoft.AspNetCore.Mvc;
using TinyPolitik.Core;

var builder = WebApplication.CreateBuilder(args);

ContentLibrary library = ContentLoader.Setup(builder);
CertificateLoader.Setup(builder);

builder.Services.AddSingleton(new LoginRateLimiter());
builder.Services.AddSingleton(new SessionStore());

var app = builder.Build();

app.MapGet("/content/version", () => ServerInfo.Get(library));


app.MapPost("/login", 
    (HttpContext context, 
    [FromBody] LoginRequest req, 
    [FromServices] SessionStore sessions,
    [FromServices] LoginRateLimiter limiter) => LoginHandler.Login(context, req, sessions, limiter));

 
app.MapGet("/content/definitions/strategicResources", (ContentLibrary lib) => Results.Json(lib.StrategicResources.Values));


CertificateLoader.NotifyInConsole();

app.Run();
