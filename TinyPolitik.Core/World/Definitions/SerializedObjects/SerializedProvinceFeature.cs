namespace PolitikServer.Core.Serialization;

public class SerializedProvinceFeature : SerializedGameDefinition
{
    public string[] strategicResources = [];

    public override GameDefinition Deserialize()
    {
        StrategicResource[] resources = DefinitionLibrary.GetDefinitonsByUid<StrategicResource>(strategicResources).ToArray();

        return new ProvinceFeature(_uniqueIdentifier, resources);
    }
}