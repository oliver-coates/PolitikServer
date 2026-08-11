namespace PolitikServer.Core;

public class ProvinceEntity : GameEntity
{
    public required SerializedField<Province> province { get; init; }
    public SerializedList<ProvinceEntity> connectedProvinces = new();
    public required int population;
    public required List<string> buildings;
    public required SerializedNullableField<Nation?> ownerNation;
    public required SerializedNullableField<Nation?> occupierNation;

    public override string ToString()
    {
        return $"Province [{UniqueIdentifier}], Pop: {population}, Buildings: [{string.Join(',', buildings)}], Owned/Controlled: {(ownerNation.Get()?.nameShort) ?? "None"}/{(occupierNation.Get()?.nameShort) ?? "None"}";
    }
}