namespace PolitikServer.Core.Serialization;

/// <summary>
/// Loads all game definitions from the game's content root, initialises libraries
/// </summary>
public class DefinitionLoader()
{
    public void LoadDefinitions()
    {
        // Load the individual files first:
        foreach (Type definitionType in SerializationSchema.DefinitionFiles.Keys)
        {
            string path = Path.Combine(ApplicationPaths.ContentRoot, SerializationSchema.DefinitionFiles[definitionType]);

            Console.WriteLine($">>> ({definitionType}) {path}");
        }

        // Then load all the directories
        foreach (Type definitionType in SerializationSchema.DefinitionDirectories.Keys)
        {
            string dir = Path.Combine(ApplicationPaths.ContentRoot, SerializationSchema.DefinitionDirectories[definitionType]);

            foreach (string path in Directory.GetFiles(dir))
            {
                Console.WriteLine($"> ({definitionType}) {path}");
            }
        }
    }
}