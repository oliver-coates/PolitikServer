namespace PolitikServer.Core.Serialization;

public class SerializedGameWorld : SerializedGameDefinition
{
    public string worldName = "";
    public string worldAuthor = "";
    public long lastUpdated = 0;

    public override GameDefinition Deserialize()
    {
        return new GameWorld(
            worldName,
            worldAuthor,
            DateTime.FromBinary(lastUpdated));
    }
}