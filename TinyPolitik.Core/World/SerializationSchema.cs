namespace PolitikServer.Core.Serialization;

/// <summary>
/// This schema defines where each game definition type is found within the content folder.
/// </summary>
public static class SerializationSchema
{
    public static Dictionary<Type, string> DefinitionFiles { get;} = new()
    {
        { typeof(GameWorld), "GameWorld.json" },
        { typeof(WorldConfig), Path.Join("config", "WorldConfig.json")},
    };

    public static Dictionary<Type, string> DefinitionDirectories { get; } = new()
    {
        { typeof(ProvinceDefinition), Path.Join("world", "provinces")},
        { typeof(BiomeType), Path.Join("world", "biome")},
        { typeof(BuildingType), Path.Join("world", "buildings")},
        { typeof(ProvinceDevelopmentLevel), Path.Join("world", "developmentLevel")},
        { typeof(ProvinceFeature), Path.Join("world", "features")},
        { typeof(ProductionMode), Path.Join("world", "productionModes")},
        { typeof(ProvinceModifier), Path.Join("world", "provinceModifiers")},
        { typeof(StrategicResource), Path.Join("world", "strategicResources")},
        { typeof(TerrainType), Path.Join("world", "terrain")},
    };
}