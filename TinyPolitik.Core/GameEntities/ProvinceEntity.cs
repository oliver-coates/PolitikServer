namespace PolitikServer.Core;

public class ProvinceEntity : GameEntity
{
    // Core data:
    public required SerializedField<ProvinceDefinition> definition { get; init; }
    public SerializedList<ProvinceEntity> connectedProvinces = new();
    
    // Owner & Occupier:
    public required SerializedNullableField<Nation?> ownerNation;
    public required SerializedNullableField<Nation?> occupierNation;
    public required bool isOccupied;


    // Economy:
    public required int population;
    public required SerializedList<BuildingEntity> buildings; 
    public required SerializedField<ProvinceDevelopmentLevel> developmentLevel;


    [Newtonsoft.Json.JsonConstructor]
    public ProvinceEntity(string UniqueIdentifier, int population, SerializedField<ProvinceDefinition> province, 
        SerializedList<ProvinceEntity> connectedProvinces, SerializedNullableField<Nation?> ownerNation, 
        SerializedNullableField<Nation?> occupierNation, 
        SerializedField<ProvinceDevelopmentLevel> developmentLevel, bool isOccupied,
        SerializedList<BuildingEntity> buildings) : base(UniqueIdentifier)
    {
        this.definition = province;
        this.connectedProvinces = connectedProvinces;

        this.ownerNation = ownerNation;
        this.occupierNation = occupierNation;
        this.isOccupied = isOccupied;

        this.population = population;
        this.developmentLevel = developmentLevel;

        this.buildings = buildings;
    }

    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
    public ProvinceEntity(ProvinceDefinition provinceDefinition, IReadOnlyList<ProvinceEntity> connectedProvinces, 
                            Nation? ownerNation, Nation? occupierNation, int population, 
                            ProvinceDevelopmentLevel developmentLevel) : base(Guid.NewGuid().ToString())
    {
        this.definition = new SerializedField<ProvinceDefinition>(provinceDefinition);
        this.connectedProvinces = new SerializedList<ProvinceEntity>(connectedProvinces); 

        this.ownerNation = new SerializedNullableField<Nation?>(ownerNation);
        this.occupierNation = new SerializedNullableField<Nation?>(occupierNation);

        this.population = population;
        this.developmentLevel = new SerializedField<ProvinceDevelopmentLevel>(developmentLevel);
    
        this.buildings = new();
        
        isOccupied = false;
    }

    internal override void Deserialize()
    {
        definition.Load();
        connectedProvinces.Load();

        ownerNation.Load();
        occupierNation.Load();

        developmentLevel.Load();
    }

    public override string GetReadableName()
    {
        return $"Province Entity for '{definition.Get().UniqueIdentifier}' Pop: '{population}', Buildings: [{string.Join(',', buildings)}], Owned/Controlled: {(ownerNation.Get()?.nameShort) ?? "None"}/{(occupierNation.Get()?.nameShort) ?? "None"}";
    }
}