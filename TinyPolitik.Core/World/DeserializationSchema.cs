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

    // Order in which to deserialize our objects.
    public static readonly List<Type> DeserializationLoadOrder = new List<Type>()
    {
        typeof(SerializedProvince),
        typeof(SerializedGameWorld)
    };

    public static readonly Dictionary<Type, Type> Deserialization = new Dictionary<Type, Type>()
    {
        {typeof(SerializedGameWorld),           typeof(GameWorld)},
        {typeof(SerializedProvince),            typeof(Province)}
    };
}