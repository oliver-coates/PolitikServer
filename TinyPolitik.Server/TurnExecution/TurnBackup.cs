using Microsoft.Extensions.Logging;

namespace PolitikServer.Core;

public class TurnBackupManager
{
    private ILogger _logger;
    private string _backupRoot;

    public TurnBackupManager(ILogger<TurnBackupManager> logger)
    {
        _logger = logger;
        _backupRoot = "Not Yet Initialised";
    }

    public void Initialise(string root)
    {
        _backupRoot = root;

        try
        {
            Directory.CreateDirectory(_backupRoot);
        }
        catch (Exception ex)
        {
            throw new Exception($"Could not initialise turn backups with provided path: {ex}");
        }
    }

    public void MakeWorldBackup(string json)
    {
        string path = Path.Combine(_backupRoot, "snapshot-world.json");

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        File.WriteAllText(path, json);
        
        _logger.LogInformation("Successfully saved world snapshot to '{path}'", path);
    }

    public void MakeTurnBackup(int turnNumber, string json)
    {
        string path = Path.Combine(_backupRoot, $"snapshop-turn-{turnNumber}.json");

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        File.WriteAllText(path, json);

        _logger.LogInformation("Successfully saved snapshot to '{path}'", path);
    }   
}