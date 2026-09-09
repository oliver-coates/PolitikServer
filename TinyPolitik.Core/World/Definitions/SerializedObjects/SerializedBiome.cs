namespace PolitikServer.Core.Serialization;

public class SerializedBiome : SerializedGameDefinition
{
    public override GameDefinitionOld Deserialize()
    {
        return new BiomeType(_uniqueIdentifier);
    }
}