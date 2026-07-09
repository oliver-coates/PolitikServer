using TinyPolitik.Core;

var builder = WebApplication.CreateBuilder(args);

ContentLoader.Setup(builder);

CertificateLoader.Setup(builder);

var app = builder.Build();

app.MapGet("/content/definitions/strategicResources", (ContentLibrary lib) => Results.Json(lib.StrategicResources.Values));

CertificateLoader.NotifyInConsole();

app.Run();
