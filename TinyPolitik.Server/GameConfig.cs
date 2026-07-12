using System.Text.Json;

namespace TinyPolitik.Core;

public class GameConfig
{
    public const string DEFAULT_PASSWORD = "change me";


    public string ServerName { get; set; }
    public int MaxPlayers { get; set; }


    // -- Turn Times:
    public string[] TurnTimesLocal { get; set; } // Local turn times, consumed and converted into UTC at startup  
    public string[] TurnTimesUtc { get; set; }


    // -- Password:
    public string? Password { get; set;}
    public string? PasswordHash { get; set; }
    public string? PasswordSalt { get; set; }

    // -- Port:
    public int Port { get; set; }

    /// <summary>
    /// This constructor sets up the default game configuration settings
    /// </summary>
    public GameConfig()
    {
        ServerName = "Unnamed Server";
        MaxPlayers = 32;

        TurnTimesLocal = ["06:00", "12:00", "18:00"];
        TurnTimesUtc = [];

        Port = 2000;

        // Turn times UTC & password hash and salt are left blank.
    }
}

public static class GameConfigLoader
{
    private static readonly JsonSerializerOptions jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        RespectNullableAnnotations = false
    };

    public static GameConfig LoadOrCreate(string path)
    {
        // Get the user to setup their config:
        if (!File.Exists(path))
        {
            CreateNewConfigFile(path);

            Console.WriteLine("\nWelcome to Tiny Politik!");
            Console.WriteLine($"A config file has been created at: '{path}'.");
            Console.WriteLine("Open this file and set the name, password & turn times, then reopen the server.");
            Environment.Exit(0);
        }

        // Read and validate the current game config
        GameConfig config = JsonSerializer.Deserialize<GameConfig>(File.ReadAllText(path))
        ?? throw new InvalidDataException($"Failed to parse config file '{path}'.");
        
        Validate(config);
        
        bool passwordHashAndSaltExists = !string.IsNullOrEmpty(config.PasswordHash) && !string.IsNullOrEmpty(config.PasswordSalt);
        bool passwordExists = !string.IsNullOrEmpty(config.Password);

        if (!passwordHashAndSaltExists && passwordExists)
        {
            var (hash, salt) = PasswordHasher.Hash(config.Password ?? "");

            config.PasswordHash = hash;
            config.PasswordSalt = salt;
            config.Password = null;

            SaveConfigFile(path, config);
            return config;
        }
        else if (passwordHashAndSaltExists && !passwordExists)
        {
            // The password hash and salt is set, while the plaintext password does not exist.
            // Good.
            return config;
        }
        else
        {
            throw new InvalidDataException("Passwords could not be parsed.");
        }
    }

    private static void SaveConfigFile(string path, GameConfig config)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        File.WriteAllText(path, JsonSerializer.Serialize(config, jsonOptions));
    }

    private static void CreateNewConfigFile(string path)
    {
        GameConfig config = new GameConfig()
        {
            Password = GameConfig.DEFAULT_PASSWORD
        };

        File.WriteAllText(path, JsonSerializer.Serialize(config, jsonOptions));
    }

    private static void Validate(GameConfig config)
    {
        if (config.MaxPlayers <= 0)
        {
            throw new InvalidDataException("Maximum player count cannot be less than 1");
        }
        else if ((string.IsNullOrEmpty(config.PasswordHash) && string.IsNullOrEmpty(config.PasswordSalt)) && (string.IsNullOrEmpty(config.Password) || config.Password == GameConfig.DEFAULT_PASSWORD))
        {
            throw new InvalidDataException("Not password has been configured - Set 'password' in the server config file and restart");
        }
        else if (config.TurnTimesLocal is null || config.TurnTimesLocal.Length == 0)
        {
            throw new InvalidDataException("Turns times local must specify at least one daily turn time (in local time)");
        }
        else
        {
            foreach (string timeLocal in config.TurnTimesLocal)
            {
                if (!TimeOnly.TryParse(timeLocal, out _))
                {
                    throw new InvalidDataException($"Could not parse turn time '{timeLocal }' - expected in a HH:mm format");
                }
            }
        }
    }

}