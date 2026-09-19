namespace PolitikServer.Core;

public class ProvinceFeature : GameDefinition
{
    public readonly SerializedList<StrategicResource> StrategicResources;

    public ProvinceFeature(string UniqueIdentifier, SerializedList<StrategicResource> StrategicResources) : base(UniqueIdentifier)
    {
        this.StrategicResources = StrategicResources;
    }

    public override string GetReadableName()
    {
        return $"Province Feature";
    }

    protected override string GetFullName()
    {
        return $"Province Feature. Provides resources: [{string.Join(',', StrategicResources.Get().Select(x => x.GetReadableName()))}]";
    }

    internal override void Deserialize()
    {
    }
}