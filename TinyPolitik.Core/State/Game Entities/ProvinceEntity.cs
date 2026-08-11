namespace PolitikServer.Core;

public class ProvinceEntity : GameEntity
{
    public required Province province { get; init; }
    public List<ProvinceEntity> connectedProvinces = [];
    public required int population;
    public required List<string> buildings;
    public required Nation? ownerNation;
    public required Nation? occupierNation;

    public override string ToString()
    {
        return $"Province [{UniqueIdentifier}], Pop: {population}, Buildings: [{string.Join(',', buildings)}], Owned/Controlled: {(ownerNation?.nameShort) ?? "None"}/{(occupierNation?.nameShort) ?? "None"}";
    }
}