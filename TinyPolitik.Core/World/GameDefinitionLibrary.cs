using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using PolitikServer.Core.Serialization;

namespace PolitikServer.Core;

/// <summary>
/// Deserialize and store all game definitions.
/// </summary>
public class GameDefinitionLibrary
{
    private readonly string _worldRoot;
    public string VersionHash { get; private set; } = "";

    // Dictionary mapping serialized game definition types to a Dictionary which maps the UID of each game definition to their raw JSON text.
    private static Dictionary<Type, Dictionary<string, string>> JsonDefinitions = [];
    
    // Dictionary mapping defintion types to their uid and definition
    public static Dictionary<Type, Dictionary<string, GameDefinition>> Content { get; private set; } = []; 

    
    #region Initialisation

    public GameDefinitionLibrary(string worldRoot)
    {
        _worldRoot = worldRoot;
        
        Reload();
    }

    private void Reload()
    {
        // Load all the definitions
        LoadAllDefinitions();
        
        // Finally get the hash
        VersionHash = ComputeHash();
    }

    private string ComputeHash()
    {
        var sb = new StringBuilder();

        // Concatenate all world data:
        foreach (IReadOnlyDictionary<string, string> contentDict in JsonDefinitions.Values)
        {
            foreach (KeyValuePair<string, string> pair in contentDict.OrderBy(k => k.Key))
            {
                sb.Append(pair.Key).Append(pair.Value);
            }
        }

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(sb.ToString()));

        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    #endregion
    

    #region Loading From Disk

    /// <summary>
    /// Loads all definitions from the contentpath
    /// </summary>
    private void LoadAllDefinitions()
    {
        // Load all content
        JsonDefinitions = new Dictionary<Type, Dictionary<string, string>>();
        foreach (KeyValuePair<Type, string> contentPath in DeserializationSchema.TypeAtPath)
        {
            Type contentType = contentPath.Key;
            string folderPath = Path.Join(_worldRoot, contentPath.Value);

            var content = LoadFolder(folderPath, false);
            if (content != null)
            {
                JsonDefinitions.Add(contentType, new Dictionary<string, string>());
                foreach (string json in content)
                {
                    JsonNode n = JsonNode.Parse(json) ?? throw new Exception($"Could not parse Json Node from json content: {json}");
                    string uid = ((string?) n["_uniqueIdentifier"]) ?? throw new Exception($"Could not read a Unique Identifier from json content: {json}.");

                    JsonDefinitions[contentType].Add(uid, json);                
                }

                Console.WriteLine($"Loaded {content.Count} of {contentType.Name}");
            }
            else
            {
                Console.WriteLine($"Warning: Could not find path '{folderPath}' while loading world file.");
            }
        }
    }

    /// <summary>
    /// Loads all .json objects in a directory.
    /// </summary>
    /// <param name="folderPath">The folder path.</param>
    /// <returns>A list of all json objects in that directory. Returns null if the directory does not exist.</returns>
    private static List<string>? LoadFolder(string folderPath, bool searchSubDirectories = false)
    {        
        // Ensure the path exists
        if (!Directory.Exists(folderPath))
        {
            return null;
        }

        SearchOption searchOption;
        if (searchSubDirectories) searchOption = SearchOption.AllDirectories;
        else searchOption = SearchOption.TopDirectoryOnly;

        List<string> results = new();
        foreach (string file in Directory.GetFiles(folderPath, "*.json", searchOption))
        {
            results.Add(File.ReadAllText(file));
        }

        return results;
    }


    #endregion


    #region Deserialization
    public static Dictionary<string, GameDefinition> GetDefintionDict<T>() where T : GameDefinition
    {
        return Content[typeof(T)];
    }

    public void DeserializeWorld()
    {
        foreach (Type defintionType in DeserializationSchema.DeserializationLoadOrder)
        {
            string defName = DeserializationSchema.DefinitionTypeDict[defintionType];
            Dictionary<string, string> jsonDict = JsonDefinitions[defintionType];
        
            foreach (KeyValuePair<string, string> jsonDefinition in jsonDict)
            {
                DeserializeDefinition(defintionType, jsonDefinition.Value);
            }
        }
    }

    public void DeserializeDefinition(Type definitionType, string json)
    {
        GameDefinition def;
        JsonNode j = JsonNode.Parse(json) ?? throw new Exception($"Could not parse json node: {json}");
        
        switch (definitionType)
        {
            case Type _ when definitionType == typeof(GameWorld):
                // def = DeserializeGameWorld(j);
                break;

            case Type _ when definitionType == typeof(Province):
                // def = DeserializeProvince(j);        
                break; 


            default:
                throw new Exception($"Could not deserialize object of type '{definitionType.Name}'");
        }

        // Content[definitionType].Add(def.UniqueIdentifier, def);
    }

    // private GameWorld DeserializeGameWorld(JsonNode j)
    // {
    //     string name = ((string?) j["worldName"]) ?? "";
    //     string author = ((string?) j["worldAuthor"]) ?? "";
    //     long dateTimeChanged = ((long?) j["timeLastUpdated"]) ?? 0;
        
    //     return new GameWorld(name, author, dateTimeChanged);
    // }

    // private Province DeserializeProvince(JsonNode j)
    // {
    //     string uid = ((string?) j["_uniqueIdentifier"])  ?? "";
    //     string name = ((string?) j["name"]) ?? "";
        
    //     JsonNode n = j["adjacentProvinceUIDs"] ?? throw new Exception();
    //     JsonArray arr = JsonArray.Parse(n);


    //         string[] connectedProvicnes = ((string[]?) j["adjacentProvinceUIDs"]) ?? [];

    //     return new Province(uid, name, )
        
    // }

    #endregion


    #region Public Methods
    
    /// <summary>
    /// Takes all the game definitions and adds them to a json array.
    /// </summary>
    public string GetAllGameDefinitionsAsJson()
    {
        var root = new JsonObject();

        foreach (KeyValuePair<Type, Dictionary<string, string>> definitonBundle in JsonDefinitions)
        {
            Type type = definitonBundle.Key;
            Dictionary<string, string> content = definitonBundle.Value;

            JsonArray jsonArray = new(content.OrderBy(d => d.Key).Select(d => JsonNode.Parse(d.Value)).ToArray());
            root.Add(type.Name, jsonArray);
        }

        return root.ToString();
    }
    
    /// <summary>
    /// Gets a game definition of the passed type, at the provided unique identifier.
    /// </summary>
    public static T GetDefinition<T>(string uid) where T : GameDefinition
    {
        return (T) Content[typeof(T)][uid];
    }

    /// <returns> A list of all game definitions of the provided type</returns>
    public static T[] GetAllDefinitionsOfType<T>() where T : GameDefinition
    {
        return (T[]) Content[typeof(T)].Values.ToArray();
    }


    #endregion
}