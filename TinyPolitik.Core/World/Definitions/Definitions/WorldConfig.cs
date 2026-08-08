namespace PolitikServer.Core;

public class WorldConfig : GameDefinition
{
    private static Dictionary<string, ConfigValue> Values = [];

    public WorldConfig(Dictionary<string, ConfigValue> values) : base("worldConfig")
    {
        Values = values;
    }

    public override string ToString()
    {
        return "$[World Config]";
    }
    
    public static string Get(string name)
    {
        return Values[name].value;
    }
}

public class ConfigValue
{
    public required string name;
    public required string value;
    public required string description;
}