namespace PolitikServer.Core.Serialization;

/// <summary>
/// Loads all records (Game Definitions, Game Entities, Game Requests, Game Notifications) from the game's content root, initialises libraries
/// </summary>
public class RecordLoader
{
    public enum LoadMethod
    {
        DefinitionsOnly, // Only attempts to load game definitions.
        AllRecords // Attempts to load all records, including entities.
    }

    private readonly DefinitionLibrary _definitionLibrary;
    private readonly EntityLibrary _entityLibrary;

    private string _entitiesJson = String.Empty;

    public RecordLoader(DefinitionLibrary definitions, EntityLibrary entities)
    {
        _definitionLibrary = definitions;
        _entityLibrary = entities;
    }

    public void Load(LoadMethod method)
    {
        try
        {
            // Ensure all libraries are clear
            FlushAllLibraries();

            // Load all definition files into memory
            LoadAllDefinitions();

            if (method == LoadMethod.AllRecords)
            {
                LoadAllEntities();
            }

            // Finally initialise all definitions
            DeserializeAllRecords();    
        }
        catch (Exception e)
        {
            Console.WriteLine("Error while loading game world:");
            Console.WriteLine(e);

            Environment.Exit(0);
        }
    }

    private void LoadAllDefinitions()
    {
        // Load the individual files first:
        foreach (Type definitionType in SerializationSchema.DefinitionFiles.Keys)
        {
            string path = Path.Combine(ApplicationPaths.ContentRoot, SerializationSchema.DefinitionFiles[definitionType]);
            
            LoadDefinition(path, definitionType);        
        }

        // Then load all the directories
        foreach (Type definitionType in SerializationSchema.DefinitionDirectories.Keys)
        {
            string dir = Path.Combine(ApplicationPaths.ContentRoot, SerializationSchema.DefinitionDirectories[definitionType]);

            foreach (string path in Directory.GetFiles(dir))
            {
                LoadDefinition(path, definitionType);
            }
        }
    }

    private void LoadAllEntities()
    {
        if (_entitiesJson == String.Empty)
        {
            throw new Exception("Entities are attempting to be loaded, but no entity data has been declared.");
        }

        // TODO: Implement this
        Console.WriteLine("Todo: Implement entity backup loading here");
    }

    public void SetEntityData(string json)
    {
        _entitiesJson = json;
    }

    private void LoadDefinition(string path, Type definitionType)
    {
        string content = File.ReadAllText(path);
        GameDefinition definition = (GameDefinition) (Newtonsoft.Json.JsonConvert.DeserializeObject(content, definitionType) ?? throw new Exception($"Could not deserialize game definition at: {path}"));
    
        _definitionLibrary.Populate(definition, definitionType);
    }

    private void FlushAllLibraries()
    {
        _definitionLibrary.Flush();
        _entityLibrary.Flush();
    }

    private void DeserializeAllRecords()
    {
        _definitionLibrary.Deserialize();
        _entityLibrary.Deserialize();
    }


}
