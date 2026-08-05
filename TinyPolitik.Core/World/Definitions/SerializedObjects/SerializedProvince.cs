namespace PolitikServer.Core.Serialization;


public class SerializedProvince : SerializedGameDefinition
{
    public string name = "";
    public WorldPoint centre;
    public string[] adjacentProvinceUIDs = [];
    public string biome = "";
    public string terrain = "";
    public string[] features = [];


    public override GameDefinition Deserialize()
    {
        return new Province(_uniqueIdentifier, name, centre, adjacentProvinceUIDs);
    }
}

