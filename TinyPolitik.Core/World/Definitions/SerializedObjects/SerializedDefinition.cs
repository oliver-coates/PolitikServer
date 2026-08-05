namespace PolitikServer.Core.Serialization;

public abstract class SerializedGameDefinition
{
    public string _uniqueIdentifier = "";

    public abstract GameDefinition Deserialize();
}