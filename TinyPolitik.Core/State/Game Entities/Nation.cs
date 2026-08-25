namespace PolitikServer.Core;

public class Nation : GameEntity
{
    public required string? playerId;
    // Utilised in real-time-play to determine if this player controlled nation is ready to advance the turn
    public bool isReady = false; 


    public required string nameLong;
    public required string nameShort;
    public required string colorPrimary;
    public required string colorTertiary;
    public required string noun;


    public required SerializedList<ProvinceEntity> provincesControlled;
    public required SerializedField<ProvinceEntity> capitolProvince;


    public override string ToString()
    {
        return $"Nation [{UniqueIdentifier}], {nameLong}, Capitol: '{capitolProvince.Get().province.Get().UniqueIdentifier}'";
    }
}