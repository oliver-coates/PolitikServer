namespace PolitikServer.Core;

public class StrategicResource : GameDefinition
{

    public StrategicResource(string UniqueIdentifier) : base(UniqueIdentifier)
    {
    }

    public override string GetReadableName()
    {
        return "Strategic Resource.";
    }

    protected override string GetFullName()
    {
        return "Strategic Resource.";
    }

    internal override void Deserialize()
    {
    }
}