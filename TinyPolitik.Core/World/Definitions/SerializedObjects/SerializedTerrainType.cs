namespace PolitikServer.Core.Serialization;

public class SerializedTerrainType : SerializedGameDefinition
{
    public override GameDefinition Deserialize()
    {
        return new TerrainType(_uniqueIdentifier);
    }
}