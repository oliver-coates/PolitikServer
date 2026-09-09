namespace PolitikServer.Core.Serialization;

public class SerializedStrategicResource : SerializedGameDefinition
{
    public override GameDefinitionOld Deserialize()
    {
        return new StrategicResource(_uniqueIdentifier);
    }
}