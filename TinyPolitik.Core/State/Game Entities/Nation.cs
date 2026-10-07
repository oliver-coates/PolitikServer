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

    [Newtonsoft.Json.JsonConstructor]
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

    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
    public Nation(string UniqueIdentifier, string? playerId, Decoration decoration, IReadOnlyList<ProvinceEntity> provincesControlled, ProvinceEntity captialProvince) : base(UniqueIdentifier)
    {
        this.UniqueIdentifier = UniqueIdentifier;
        this.playerId = playerId;
        this.nameLong = decoration.nameLong;
        this.nameShort = decoration.nameShort;
        this.colorPrimary = decoration.colorPrimary;
        this.colorTertiary = decoration.colorTertiary;
        this.noun = decoration.noun;
    
        this.provincesControlled = new SerializedList<ProvinceEntity>(provincesControlled);
        this.captialProvince = new SerializedField<ProvinceEntity>(captialProvince);
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

    public class Decoration
    {
        public required string nameLong;
        public required string nameShort;
        public required string colorPrimary;
        public required string colorTertiary;
        public required string noun;

        public override string ToString()
    {
        return $"[Nation Decoration] Long: '{nameLong}', Shortened: '{nameShort}', Verb: '{noun}', Colors ({colorPrimary}/{colorTertiary})";
    }
    }
}