using PolitikServer.Core.Serialization;

namespace PolitikServer.Core;

/// <summary>
/// Static helper class which manages event calls for the initialisation of the server.
/// </summary>
public static class ServerInitialiser
{

    /// <summary>
    /// Primary Initialisation occurs prior to the game world being initialised.
    /// </summary>
    public static event Action<ServerInitialisationMode>? OnPrimarlyInitialise;

    /// <summary>
    /// Secondary initialisation occurs after the game world has been initialised.
    /// </summary>
    public static event Action<ServerInitialisationMode>? OnSecondaryInitialise;


    public static ServerInitialisationMode InitialisePrimary(WebApplication app)
    {
        Validate(app);

        ServerInitialisationMode mode;

        var backupManager = app.Services.GetRequiredService<TurnBackupManager>(); 

        if (backupManager.DoBackupsExist() && backupManager.TryGetLastGamestateBackup(out string json))
        {
            mode = new InitialiseFromBackup()
            {
                backupJson = json
            };

        }
        else
        {
            mode = new InitialiseNewWorld();            
        }

        OnPrimarlyInitialise?.Invoke(mode);

        return mode;
    }

    public static void InitialiseSecondary(ServerInitialisationMode mode)
    {
        OnSecondaryInitialise?.Invoke(mode);
    }

    private static void Validate(WebApplication app)
    {
        try
        {
            app.Services.GetRequiredService<GameWorldValidator>().ValidateGameWorld();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Could not locate game world file. Ensure you have a valid TinyPolitik game world located at '{ApplicationPaths.ContentRoot}'.");
            Console.WriteLine(e);

            Environment.Exit(0);
        }
    }
}

