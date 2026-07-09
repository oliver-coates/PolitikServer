namespace TinyPolitik.Core;


public static class ContentLoader
{
    public static ContentLibrary Setup(WebApplicationBuilder builder)
    {
        var contentRoot = Path.Combine(builder.Environment.ContentRootPath, "content");
        Directory.CreateDirectory(Path.Combine(contentRoot, "strategic resources"));
        
        var lib = new ContentLibrary(contentRoot);
        builder.Services.AddSingleton(lib);
        
        return lib;
    }

    public static void LoadAllContent()
    {
        
    }
}