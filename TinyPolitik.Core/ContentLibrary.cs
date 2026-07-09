using System.Text.Json;

namespace TinyPolitik.Core;


public class ContentLibrary
{
    // Path to the root of the content files
    private readonly string _contentRoot;

    private static readonly JsonSerializerOptions JsonOptions = new () { PropertyNameCaseInsensitive = true };

    public IReadOnlyDictionary<string, StrategicResourceDefinition> StrategicResources {get; private set; } = new Dictionary<string, StrategicResourceDefinition>();

    public ContentLibrary(string contentRootPath)
    {
        _contentRoot = contentRootPath;
        
        Reload();
    }

    private void Reload()
    {
        StrategicResources = LoadContentOfType<StrategicResourceDefinition>("strategic resources");
    }

    /// <summary>
    /// Loads all objects of provided type within the subfolder name into the provided dictionary
    /// </summary>
    private Dictionary<string, T> LoadContentOfType<T>(string subfolderName) where T : IGameDefintion
    {
        var path = Path.Combine(_contentRoot, subfolderName);
        var dict = new Dictionary<string, T>();

        // Ensure directory exists
        if (Directory.Exists(path) == false)
        {
            throw new Exception($"Could not find path '{path}' when loading content type '{typeof(T)}'.");
        }

        // Get all json files, even in subdirectories
        foreach (var file in Directory.GetFiles(path, "*.json", SearchOption.AllDirectories))
        {
            T? definition = JsonSerializer.Deserialize<T>(File.ReadAllText(file), JsonOptions);

            // Ensure it is not null
            if (definition == null)
            {
                throw new Exception($"Could not parse '{file}'.");  
            }

            // Ensure the definition id is unique
            string defintionId = definition.Id;
            if (dict.ContainsKey(defintionId))
            {
                throw new Exception($"Definition Id '{defintionId}' is non-unique.");
            }

            dict.Add(defintionId, definition);
        }   
    
        return dict;
    }
}