using TinyPolitik.Core;

var builder = WebApplication.CreateBuilder(args);

var contentRoot = Path.Combine(builder.Environment.ContentRootPath, "content");
Directory.CreateDirectory(Path.Combine(contentRoot, "strategic resources"));

builder.Services.AddSingleton(new ContentLibrary(contentRoot));

var app = builder.Build();

app.MapGet("/content/definitions/strategicResources", (ContentLibrary lib) => Results.Json(lib.StrategicResources.Values));

app.Run();
