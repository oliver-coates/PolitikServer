namespace PolitikServer.Core;

public static class ApplicationPaths
{
    /// <summary>
    /// The root directory for all gamedata.
    /// </summary>
    public static string DataRoot       { get; private set; } = "";
    
    /// <summary>
    /// Directory for all game logs to be stored in.
    /// </summary>
    public static string LogsRoot       { get; private set; } = "";

    /// <summary>
    /// Root Directory for all gameworld content (all defintions: provinces, resources, factories).
    /// </summary>
    public static string ContentRoot    { get; private set; } = "";

    /// <summary>
    /// Directory for all backups to be stored in.
    /// </summary>
    public static string BackupsRoot    { get; private set; } = "";

    /// <summary>
    /// The path to the accounts file, containing all the information regarding the game accounts.
    /// </summary>
    public static string AccountsPath   { get; private set; } = "";

    /// <summary>
    /// The path to the ServerConfig file, defining the port and ip and password etc
    /// </summary>
    public static string ServerConfigPath { get; private set; } = "";

    public static void InitialiseDirectories(string contentRootPath)
    {
        // Paths:
        DataRoot = Path.Combine(contentRootPath, "gamedata");
        
        LogsRoot = Path.Combine(contentRootPath, "gamedata", "logs");
        ContentRoot = Path.Combine(contentRootPath, "gamedata", "gameworld");
        BackupsRoot = Path.Combine(contentRootPath, "gamedata", "backups");
        
        AccountsPath = Path.Combine(contentRootPath, "gamedata", "accounts.json");
        ServerConfigPath = Path.Combine(DataRoot, "config.json");

        // Initialising Directories:
        Directory.CreateDirectory(DataRoot);

        Directory.CreateDirectory(ContentRoot);
        Directory.CreateDirectory(LogsRoot);
    }

}