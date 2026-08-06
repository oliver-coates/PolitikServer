namespace PolitikServer.Core;


public static class ContentLoader
{
    public static EntityLibrary Setup(WebApplicationBuilder builder)
    {
        var contentRoot = Path.Combine(builder.Environment.ContentRootPath, "content");
        Directory.CreateDirectory(Path.Combine(contentRoot, "strategic resources"));
        
        var lib = new EntityLibrary();
        builder.Services.AddSingleton(lib);
        
        return lib;
    }

    public static void LoadAllContent()
    {
        
    }
}