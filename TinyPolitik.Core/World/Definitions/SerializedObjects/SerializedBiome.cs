namespace PolitikServer.Core.Serialization;

public class SerializedBiome : SerializedGameDefinition
{
    public override GameDefinition Deserialize()
    {
        return new BiomeType(_uniqueIdentifier);
    }
}