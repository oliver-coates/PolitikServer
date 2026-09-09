namespace PolitikServer.Core.Serialization;

public class SerializedTerrainType : SerializedGameDefinition
{
    public override GameDefinitionOld Deserialize()
    {
        return new TerrainType(_uniqueIdentifier);
    }
}