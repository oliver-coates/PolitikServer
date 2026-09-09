namespace PolitikServer.Core.Serialization;

public class SerializedWorldConfig : SerializedGameDefinition
{
    public List<ConfigValue> configValues = [];

    public override GameDefinition Deserialize()
    {
        Dictionary<string, ConfigValue> dictMap = [];

        foreach(ConfigValue value in configValues)
        {
            dictMap.Add(value.name, value);
        }

        return new WorldConfig(dictMap);
    }
}