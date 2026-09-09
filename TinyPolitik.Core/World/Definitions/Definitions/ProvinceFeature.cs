namespace PolitikServer.Core;

public class ProvinceFeature : GameDefinitionOld
{
    public readonly StrategicResource[] resources;

    public ProvinceFeature(string UniqueIdentifier, StrategicResource[] resources) : base(UniqueIdentifier)
    {
        this.resources = resources;
    }

    public override string ToString()
    {
        return $"[{UniqueIdentifier}] Province Feature.";
    }
}