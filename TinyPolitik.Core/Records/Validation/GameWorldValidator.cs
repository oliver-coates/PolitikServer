namespace PolitikServer.Core.Serialization;

/// <summary>
/// Validates the existence of the gameworld.
/// </summary>
public class GameWorldValidator
{
    public bool ValidateGameWorld()
    {
        string gameWorldPath = Path.Combine(ApplicationPaths.ContentRoot, "GameWorld.json"); 
        
        if (!File.Exists(gameWorldPath))
        {
            throw new Exception($"Could not find game world at: '{gameWorldPath}'");
        }

        return true;
    }
}