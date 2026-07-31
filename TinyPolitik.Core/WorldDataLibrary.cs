using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;

namespace TinyPolitik.Core;

public class WorldDataLibrary
{
    private readonly string _worldRoot;

    public readonly Dictionary<string, string> ContentPathSchema = new Dictionary<string, string>()
    {
        {"World",                           ""},
        {"Biome Types",                     Path.Join("world", "biome")},
        {"Province Development Level",      Path.Join("world", "developmentLevel")},
        {"Province Features",               Path.Join("world", "features")},
        {"Province Modifiers",              Path.Join("world", "provinceModifiers")},
        {"Provinces",                       Path.Join("world", "provinces")},
        {"Strategic Resources",             Path.Join("world", "strategicResources")},
        {"Terrain Types",                   Path.Join("world", "terrain")},
    };


    public string VersionHash { get; private set; } = "";

    public Dictionary<string, IReadOnlyDictionary<string, string>> ContentJson {get; private set; } = new Dictionary<string, IReadOnlyDictionary<string, string>>();

    public WorldDataLibrary(string worldRoot)
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
        foreach (IReadOnlyDictionary<string, string> contentDict in ContentJson.Values)
        {
            foreach (KeyValuePair<string, string> pair in contentDict.OrderBy(k => k.Key))
            {
                sb.Append(pair.Key).Append(pair.Value);
            }
        }

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(sb.ToString()));

        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private void LoadAllDefinitions()
    {
        // Load all content
        ContentJson = new Dictionary<string, IReadOnlyDictionary<string, string>>();
        foreach (KeyValuePair<string, string> contentPath in ContentPathSchema)
        {
            string contentType = contentPath.Key;
            string folderPath = Path.Join(_worldRoot, contentPath.Value);

            var content = LoadFolder(folderPath);
            if (content != null)
            {
                ContentJson[contentType] = content;
                Console.WriteLine($"Loaded {content.Count} of {contentType}");
            }
            else
            {
                Console.WriteLine($"Warning: Could not find path '{folderPath}' while loading world file.");
            }

        }
    }

    private static Dictionary<string, string>? LoadFolder(string folderPath)
    {        
        // Ensure the path exists
        if (!Directory.Exists(folderPath))
        {
            return null;
        }

        Dictionary<string, string> result = new();
        foreach (string file in Directory.GetFiles(folderPath))
        {
            result[Path.GetFileNameWithoutExtension(file)] = File.ReadAllText(file);
        }

        return result;
    }

    public string GetWorldDataAsString()
    {
        var root = new JsonObject();

        foreach (KeyValuePair<string, IReadOnlyDictionary<string, string>> definitions in ContentJson)
        {
            string type = definitions.Key;
            JsonArray jsonArray = new(definitions.Value.OrderBy(k => k.Key).Select(kv => JsonNode.Parse(kv.Value)).ToArray());
            root.Add(type, jsonArray);
        }

        return root.ToString();
    }
}