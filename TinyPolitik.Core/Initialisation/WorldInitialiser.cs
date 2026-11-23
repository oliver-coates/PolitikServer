namespace PolitikServer.Core;

/// <summary>
/// Static helper class which manages event calls for the initialisation of the game world.
/// </summary>
public static class WorldInitialiser
{
    public static event Action<ServerInitialisationMode>? OnInitialiseEarly;
    public static event Action<ServerInitialisationMode>? OnInitialiseMain;
    public static event Action<ServerInitialisationMode>? OnInitialiseLate;

    public static void Initialise(ServerInitialisationMode mode)
    {
        OnInitialiseEarly?.Invoke(mode);
        
        OnInitialiseMain?.Invoke(mode);
        
        OnInitialiseLate?.Invoke(mode);
    }
}