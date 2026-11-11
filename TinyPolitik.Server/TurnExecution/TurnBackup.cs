using Microsoft.Extensions.Logging;

namespace PolitikServer.Core;

public class TurnBackupManager
{
    public const string DEFINITIONS_FILE_PREFIX = "world";
    public const string ENTITIES_FILE_PREFIX = "entities-turn-";

    private ILogger _logger;
    private string _backupRoot;
    private GameBackup[] _backups;

    public TurnBackupManager(ILogger<TurnBackupManager> logger)
    {
        _logger = logger;
        _backups = [];
        _backupRoot = ApplicationPaths.BackupsRoot;

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
        string path = Path.Combine(_backupRoot, $"{DEFINITIONS_FILE_PREFIX}.json");

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        File.WriteAllText(path, json);
        
        _logger.LogInformation("Successfully saved world snapshot to '{path}'", path);
    }

    public void MakeTurnBackup(int turnNumber, string json)
    {
        string path = Path.Combine(_backupRoot, $"{ENTITIES_FILE_PREFIX}{turnNumber}.json");

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        File.WriteAllText(path, json);

        _logger.LogInformation("Successfully saved snapshot to '{path}'", path);
    }

    private void LoadBackups()
    {
        if (!Directory.Exists(_backupRoot))
        {
            throw new Exception("The backup directory does not exist");
        }

        List<GameBackup> allBackups = [];

        string[] allJsons = Directory.GetFiles(_backupRoot, "*.json");

        // Entity backup files all conform to the 'snapshot-turn-0.json' filter.
        foreach (string entityBackupFilePath in allJsons.Where(j => j.Contains(ENTITIES_FILE_PREFIX)))
        {
            // Splits it so we have 'snapshot-turn-XXX.json'
            string pathRemoved = entityBackupFilePath.Split(Path.DirectorySeparatorChar)[^1];
            
            // Get the substring, shearing off the 'snapshot-turn-' and the '.json', leaving us just with the 'XXX'
            int length = pathRemoved.Length - (ENTITIES_FILE_PREFIX.Length + ".json".Length);  
            string numStr = pathRemoved.Substring(ENTITIES_FILE_PREFIX.Length, length);
            
            allBackups.Add( new GameBackup()
            {
                path = entityBackupFilePath,
                number = int.Parse(numStr),
                type = GameBackup.Type.Entities
            });
        }
        
        _backups = [.. allBackups];
    }

    /// <summary>
    /// Attempts to pull the last (latest) gamestate backup from the backups directory.
    /// Returns the backup as a JSON string.
    /// </summary>
    public bool TryGetLastGamestateBackup(out string json)
    {
        try
        {
            LoadBackups();

            GameBackup latestBackup = _backups.Where(j => j.type == GameBackup.Type.Entities).OrderBy(j => j.number).Last();

            json = File.ReadAllText(latestBackup.path);
            return true;
        }
        catch (Exception e)
        {
            json = e.ToString();
            return false;
        }
    }   

    private class GameBackup
    {
        public enum Type
        {
            Definitions,
            Entities
        }
        public required string path;
        public required int number;
        public required Type type;
        
    }
}
