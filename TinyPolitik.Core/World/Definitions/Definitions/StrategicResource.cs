namespace PolitikServer.Core;

public class StrategicResource : GameDefinition
{
    public StrategicResource(string UniqueIdentifier) : base(UniqueIdentifier)
    {
    }

    public override string ToString()
    {
        return $"[{UniqueIdentifier}] Strategic Resource.";
    }
}