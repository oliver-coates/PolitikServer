namespace PolitikServer.Core.Serialization;

public class SerializedStrategicResource : SerializedGameDefinition
{
    public override GameDefinition Deserialize()
    {
        return new StrategicResource(_uniqueIdentifier);
    }
}