namespace PolitikServer.Core;

public class ProvinceEntity : GameEntity
{
    // Core data:
    public required SerializedField<ProvinceDefinition> province { get; init; }
    public SerializedList<ProvinceEntity> connectedProvinces = new();
    
    // Owner & Occupier:
    public required SerializedNullableField<Nation?> ownerNation;
    public required SerializedNullableField<Nation?> occupierNation;


    // Economy:
    public required int population;
    public required List<string> buildings; // Not yet implemented...
    public required SerializedField<ProvinceDevelopmentLevel> developmentLevel;

    public ProvinceEntity(string UniqueIdentifier, int population, SerializedField<ProvinceDefinition> province, SerializedList<ProvinceEntity> connectedProvinces, SerializedNullableField<Nation?> ownerNation, SerializedNullableField<Nation?> occupierNation, SerializedField<ProvinceDevelopmentLevel> developmentLevel) : base(UniqueIdentifier)
    {
        this.province = province;
        this.connectedProvinces = connectedProvinces;

        this.ownerNation = ownerNation;
        this.occupierNation = occupierNation;

        this.population = population;
        this.developmentLevel = developmentLevel;
    }

    internal override void Deserialize()
    {
        province.Load();
        connectedProvinces.Load();

        ownerNation.Load();
        occupierNation.Load();

        developmentLevel.Load();
    }

    public override string GetReadableName()
    {
        return $"Province Entity for '{province.Get().GetReadableName()}' Pop: '{population}', Buildings: [{string.Join(',', buildings)}], Owned/Controlled: {(ownerNation.Get()?.nameShort) ?? "None"}/{(occupierNation.Get()?.nameShort) ?? "None"}";
    }
}