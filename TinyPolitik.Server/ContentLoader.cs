namespace TinyPolitik.Core;


public static class ContentLoader
{
    public static void Setup(WebApplicationBuilder builder)
    {
        var contentRoot = Path.Combine(builder.Environment.ContentRootPath, "content");
        Directory.CreateDirectory(Path.Combine(contentRoot, "strategic resources"));

        builder.Services.AddSingleton(new ContentLibrary(contentRoot));
    }

    public static void LoadAllContent()
    {
        
    }
}