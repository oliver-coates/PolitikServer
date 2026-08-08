namespace PolitikServer.Core.Serialization;

public static class SerializationSchema
{
    /// <summary>
    /// Maps the serialized definition type, to where it is stored under the gamewrold directory.
    /// </summary>
    public static readonly Dictionary<Type, string> TypeAtPath = new()
    {
        {typeof(SerializedGameWorld),                       ""},
        {typeof(SerializedWorldConfig),                     "config"},
        {typeof(SerializedProvince),                        Path.Join("world", "provinces")},
        {typeof(SerializedBiome),                           Path.Join("world", "biome")},
        {typeof(SerializedProvinceDevelopmentLevel),        Path.Join("world", "developmentLevel")},
        {typeof(SerializedProvinceFeature),                 Path.Join("world", "features")},
        {typeof(SerializedProvinceModifier),                Path.Join("world", "provinceModifiers")},
        {typeof(SerializedStrategicResource),               Path.Join("world", "strategicResources")},
        {typeof(SerializedTerrainType),                     Path.Join("world", "terrain")},
        {typeof(SerializedBuildingType),                    Path.Join("world", "buildings")},
        {typeof(SerializedProductionMode),                  Path.Join("world", "productionModes")},
    };

    // Order in which to deserialize our objects.
    public static readonly List<Type> DeserializationLoadOrder = new List<Type>()
    {
        // Always config first:
        typeof(SerializedWorldConfig),

        // Economy:
        typeof(SerializedProvinceDevelopmentLevel),
        typeof(SerializedStrategicResource),
        typeof(SerializedProductionMode), // Depends on strategic resources
        typeof(SerializedBuildingType), // Depends on production modes
        
        // Provinces:
        typeof(SerializedProvinceModifier),
        typeof(SerializedBiome),
        typeof(SerializedTerrainType),
        typeof(SerializedProvinceFeature),
        typeof(SerializedProvince), // Depends on biomes, terrains and features

        // Finish with the world:
        typeof(SerializedGameWorld)  // Depends on literally everything
    };

    // Maps serialized types to deserialized types
    public static readonly Dictionary<Type, Type> Deserialization = new Dictionary<Type, Type>()
    {
        {typeof(SerializedGameWorld),                       typeof(GameWorld)},
        {typeof(SerializedWorldConfig),                     typeof(WorldConfig)},
        {typeof(SerializedProvince),                        typeof(Province)},
        {typeof(SerializedBiome),                           typeof(BiomeType)},
        {typeof(SerializedProvinceDevelopmentLevel),        typeof(ProvinceDevelopmentLevel)},
        {typeof(SerializedProvinceFeature),                 typeof(ProvinceFeature)},
        {typeof(SerializedProvinceModifier),                typeof(ProvinceModifier)},
        {typeof(SerializedStrategicResource),               typeof(StrategicResource)},
        {typeof(SerializedTerrainType),                     typeof(TerrainType)},
        {typeof(SerializedBuildingType),                    typeof(BuildingType)},
        {typeof(SerializedProductionMode),                  typeof(ProductionMode)},
    };
}