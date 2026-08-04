namespace PolitikServer.Core.Serialization;

public static class DeserializationSchema
{
    /// <summary>
    /// Maps the serialized definition type, to where it is stored under the gamewrold directory.
    /// </summary>
    public static readonly Dictionary<Type, string> TypeAtPath = new()
    {
        {typeof(SerializedGameWorld),                           ""},
        {typeof(SerializedProvince),           Path.Join("world", "provinces")},
        // {"Biome Types",                     Path.Join("world", "biome")},
        // {"Province Development Level",      Path.Join("world", "developmentLevel")},
        // {"Province Features",               Path.Join("world", "features")},
        // {"Province Modifiers",              Path.Join("world", "provinceModifiers")},
        // {"Strategic Resources",             Path.Join("world", "strategicResources")},
        // {"Terrain Types",                   Path.Join("world", "terrain")},
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