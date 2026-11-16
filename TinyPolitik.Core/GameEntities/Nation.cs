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
    public required SerializedList<ProvinceEntity> provincesOwned;
    public required SerializedField<ProvinceEntity> capitalProvince;

    public readonly SerializedField<NationalEconomy> economy;


    [Newtonsoft.Json.JsonConstructor]
    public Nation(string UniqueIdentifier, string? playerId, string nameLong,
        string nameShort, string colorPrimary, string colorTertiary,
        string noun, SerializedList<ProvinceEntity> provincesControlled,
        SerializedList<ProvinceEntity> provincesOwned, SerializedField<ProvinceEntity> captialProvince,
        SerializedField<NationalEconomy> economy) : base(UniqueIdentifier)
    {
        this.playerId = playerId;
        this.nameLong = nameLong;
        this.nameShort = nameShort;
        this.colorPrimary = colorPrimary;
        this.colorTertiary = colorTertiary;
        this.noun = noun;
    
        this.provincesControlled = provincesControlled;
        this.provincesOwned = provincesOwned;
        this.capitalProvince = captialProvince;
        
        this.economy = economy;
    }

    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
    public Nation(string? playerId, Decoration decoration, IReadOnlyList<ProvinceEntity> provinces, ProvinceEntity captialProvince) : base(Guid.NewGuid().ToString())
    {
        this.playerId = playerId;
        this.nameLong = decoration.nameLong;
        this.nameShort = decoration.nameShort;
        this.colorPrimary = decoration.colorPrimary;
        this.colorTertiary = decoration.colorTertiary;
        this.noun = decoration.noun;
    
        this.provincesControlled = new SerializedList<ProvinceEntity>(provinces);
        this.provincesOwned = new SerializedList<ProvinceEntity>(provinces);
        this.capitalProvince = new SerializedField<ProvinceEntity>(captialProvince);
        
        var economy = new NationalEconomy();
        this.economy = new SerializedField<NationalEconomy>(economy);
    }

    public override string GetReadableName()
    {
        return $"Nation '{nameLong}', Capitol: '{capitalProvince.Get().definition.Get().GetReadableName()}";
    }


    internal override void Deserialize()
    {
        provincesControlled.Load();
        capitalProvince.Load();
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