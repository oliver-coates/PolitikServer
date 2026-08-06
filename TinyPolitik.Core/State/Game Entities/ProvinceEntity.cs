namespace PolitikServer.Core;

public class ProvinceEntity : GameEntity
{
    public required Province province { get; init; }
    public required int population;
    public required List<string> buildings;
    public required string? ownerNation;
    public required string? occupierNation;

    public override string ToString()
    {
        return $"[{UniqueIdentifier}], Pop: {population}, Buildings: [{string.Join(',', buildings)}], Owned/Controlled: {(ownerNation) ?? "None"}/{(occupierNation) ?? "None"}";
    }
}