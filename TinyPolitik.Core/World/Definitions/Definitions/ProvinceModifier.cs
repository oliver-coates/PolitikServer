namespace PolitikServer.Core;

public class ProvinceModifier : GameDefinition
{
    public readonly int DurationBase;
    public readonly ProvinceModifierEffect[] Effects;
    
    public SerializedNullableField<ProvinceModifier> FollowOnModifier;

    public ProvinceModifier(string UniqueIdentifier, int DurationBase, ProvinceModifierEffect[] Effects, SerializedNullableField<ProvinceModifier> FollowOnModifier) : base(UniqueIdentifier)
    {
        this.DurationBase = DurationBase;
        this.Effects = Effects;
        this.FollowOnModifier = FollowOnModifier;
    }

    protected override string GetFullName()
    {
        return $"Province Modifier. Duration: {DurationBase}, Follows on: {((FollowOnModifier != null) ? FollowOnModifier : "None")}, Effects: [{string.Join(",", Effects.Select(o => $"{o.type}:{o.value}"))}]";
    }
    
    public override string GetReadableName()
    {
        return "Province Modifier.";
    }

    internal override void Deserialize()
    {
        FollowOnModifier.Load();
    }

}

public class ProvinceModifierEffect
{
    public string type = "";
    public string value = "";

    public ProvinceModifierEffect(string type, string value)
    {
        this.type = type;
        this.value = value;
    }
}