namespace PolitikServer.Core;

public class WorldConfig : GameDefinition
{
    [Newtonsoft.Json.JsonIgnore] private static Dictionary<string, ConfigValue> ValuesDict = [];
    [Newtonsoft.Json.JsonRequired] private List<ConfigValue> Values;

    public WorldConfig(List<ConfigValue> Values) : base("WorldConfig")
    {
        this.Values = Values;

        ValuesDict = new();
        foreach (ConfigValue configValue in Values)
        {
            ValuesDict.Add(configValue.name, configValue);   
        }
    }

    internal override void Deserialize()
    {
    }

    protected override string GetFullName()
    {
        return "";
    }

    public override string GetReadableName()
    {
        return "";
    }


    public static string Get(string name)
    {
        try
        {
            return ValuesDict[name].value;
        }
        catch (KeyNotFoundException)
        {
            throw new Exception($"Could not find string value of '{name}' in world config.");
        }
    }

    public static int GetInt(string name)
    {
        try
        {
            return int.Parse(ValuesDict[name].value);
        }
        catch (KeyNotFoundException)
        {
            throw new Exception($"Could not find '{name}' in world config.");
        }
        catch (FormatException)
        {
            throw new Exception($"Could not parse '{ValuesDict[name]}' to integer in world config.");
        }
        
    }

    public static float GetFloat(string name)
    {
        try
        {
            return float.Parse(ValuesDict[name].value);
        }
        catch (KeyNotFoundException)
        {
            throw new Exception($"Could not find value'{name}' in world config.");
        }
        catch (FormatException)
        {
            throw new Exception($"Could not parse '{ValuesDict[name]}' to float in world config.");
        }
    }

    
}

public class ConfigValue
{
    public required string name;
    public required string value;
    public required string description;
}