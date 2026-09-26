namespace PolitikServer.Core;

public class Nation : GameEntity
{
    public required string? playerId;
    
    // Utilised in real-time-play to determine if this player controlled nation is ready to advance the turn
    [Newtonsoft.Json.JsonIgnore] public bool isReady = false; 


    public required string nameLong;
    public required string nameShort;
    public required string colorPrimary;
    public required string colorTertiary;
    public required string noun;


    public required SerializedList<ProvinceEntity> provincesControlled;
    public required SerializedField<ProvinceEntity> captialProvince;

    public Nation(string UniqueIdentifier, string? playerId, string nameLong, string nameShort, string colorPrimary, string colorTertiary, string noun, SerializedList<ProvinceEntity> provincesControlled, SerializedField<ProvinceEntity> captialProvince) : base(UniqueIdentifier)
    {
        this.playerId = playerId;
        this.nameLong = nameLong;
        this.nameShort = nameShort;
        this.colorPrimary = colorPrimary;
        this.colorTertiary = colorTertiary;
        this.noun = noun;
    
        this.provincesControlled = provincesControlled;
        this.captialProvince = captialProvince;
    }

    public override string GetReadableName()
    {
        return $"Nation {nameLong}, Capitol: '{captialProvince.Get().province.Get().UniqueIdentifier}";
    }


    internal override void Deserialize()
    {
        provincesControlled.Load();
        captialProvince.Load();
    }
}