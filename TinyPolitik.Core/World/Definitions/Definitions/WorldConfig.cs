namespace PolitikServer.Core;

public class WorldConfig : GameDefinition
{
    private static WorldConfig? _Instance;
    private Dictionary<string, ConfigValue> ValuesDict = [];
    private List<ConfigValue> Values;

    public WorldConfig(List<ConfigValue> Values) : base("WorldConfig")
    {
        _Instance = this;
        this.Values = Values;

        ValuesDict = new();
        foreach (ConfigValue configValue in Values)
        {
            ValuesDict.Add(configValue.name, configValue);   
        }
    }

    public override string ToString()
    {
        return "$[World Config]";
    }
    
    public static string Get(string name)
    {
        return _Instance?.ValuesDict[name].value ?? throw new Exception("World Config has not been initialised");
    }

    public static int GetInt(string name)
    {
        return int.Parse(_Instance?.ValuesDict[name].value ?? throw new Exception("World Config has not been initialised"));
    }

    public static float GetFloat(string name)
    {
        return float.Parse(_Instance?.ValuesDict[name].value ?? throw new Exception("World Config has not been initialised"));
    }

    internal override void Deserialize()
    {
    }
}

public class ConfigValue
{
    public required string name;
    public required string value;
    public required string description;
}