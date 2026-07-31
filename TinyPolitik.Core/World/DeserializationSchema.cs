namespace PolitikServer.Core;

public static class DeserializationSchema
{
    // Maps the JSON header to the content path it is pulled from
    public static readonly Dictionary<string, string> ContentPath = new()
    {
        {"World",                           ""},
        {"Biome Types",                     Path.Join("world", "biome")},
        {"Province Development Level",      Path.Join("world", "developmentLevel")},
        {"Province Features",               Path.Join("world", "features")},
        {"Province Modifiers",              Path.Join("world", "provinceModifiers")},
        {"Provinces",                       Path.Join("world", "provinces")},
        {"Strategic Resources",             Path.Join("world", "strategicResources")},
        {"Terrain Types",                   Path.Join("world", "terrain")},
    };

    public static readonly Dictionary<Type, string> DefinitionTypeDict = new()
    {
        {typeof(GameWorld), "World"},
        {typeof(Province), "Provinces"},
    };

    // Order in which to deserialize our objects.
    public static readonly List<Type> DeserializationLoadOrder = new List<Type>()
    {
        typeof(Province),
        typeof(GameWorld)
    };
}