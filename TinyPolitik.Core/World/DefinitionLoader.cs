namespace PolitikServer.Core.Serialization;

/// <summary>
/// Loads all game definitions from the game's content root, initialises libraries
/// </summary>
public class DefinitionLoader
{
    private DefinitionLibrary _definitionLibrary;
    private Dictionary<Type, List<GameDefinition>> _allDefinitions = new();
    
    public DefinitionLoader(DefinitionLibrary lib)
    {
        _definitionLibrary = lib;
    }

    public void Load()
    {
        // First load all definition files into memory
        LoadAllDefinitions();

        // Then populate the definition library with the definitions
        PopulateAllDefinitions();

        // Finally initialise all definitions
        DeserializeAllDefinitions();
    }

    private void LoadAllDefinitions()
    {
        // Load the individual files first:
        foreach (Type definitionType in SerializationSchema.DefinitionFiles.Keys)
        {
            string path = Path.Combine(ApplicationPaths.ContentRoot, SerializationSchema.DefinitionFiles[definitionType]);
            
            GameDefinition definition = LoadDefinition(path, definitionType);        
            _allDefinitions.Add(definitionType, new List<GameDefinition>() {definition});
        }

        // Then load all the directories
        foreach (Type definitionType in SerializationSchema.DefinitionDirectories.Keys)
        {
            _allDefinitions.Add(definitionType, new List<GameDefinition>());

            string dir = Path.Combine(ApplicationPaths.ContentRoot, SerializationSchema.DefinitionDirectories[definitionType]);

            foreach (string path in Directory.GetFiles(dir))
            {
                GameDefinition definition = LoadDefinition(path, definitionType);
                _allDefinitions[definitionType].Add(definition);
            }
        }
    }

    private GameDefinition LoadDefinition(string path, Type definitionType)
    {
        string content = File.ReadAllText(path);
        GameDefinition definition = (GameDefinition) (Newtonsoft.Json.JsonConvert.DeserializeObject(content, definitionType) ?? throw new Exception($"Could not deserialize game definition at: {path}"));
    
        return definition;
    }

    private void DeserializeAllDefinitions()
    {
        foreach (List<GameDefinition> definitions in _allDefinitions.Values)
        {
            foreach (GameDefinition definition in definitions)
            {
                try
                {
                    definition.Deserialize();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception occured while deserializing a game definition: {e}");
                }

            }
        }
    }

    private void PopulateAllDefinitions()
    {
        foreach (KeyValuePair<Type, List<GameDefinition>> pair in _allDefinitions)
        {
            _definitionLibrary.Populate(pair.Value, pair.Key);
        }
    }

}