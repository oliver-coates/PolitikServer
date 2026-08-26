
using System.Text.Json;

namespace PolitikServer.Core;

public class Account
{
    public string PlayerId { get; init; } = Guid.NewGuid().ToString();
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string PasswordSalt { get; set; } = "";
    public DateTime CreatedAtUTC { get; set; } = DateTime.UtcNow;
    public long CreatedAtUtcBinary { get; set; } = DateTime.UtcNow.ToBinary();

}

public class AccountStore
{
    public enum CreationResult
    {
        Success = 0,
        UsernameTaken = 1,
        InvalidUsername = 2,
    }

    private readonly string _path;
    private readonly object _lock = new();
    private Dictionary<string, Account> _byId = new();
    private Dictionary<string, string> _usernameToId = new();

    public AccountStore(string path)
    {
        _path = path;

        if (File.Exists(path))
        {
            var accounts = JsonSerializer.Deserialize<List<Account>>(File.ReadAllText(path)) ?? new();
            _byId = accounts.ToDictionary(a => a.PlayerId);

            foreach (KeyValuePair<string, Account> pair in _byId)
            {
                _usernameToId.Add(pair.Key, pair.Value.Username.ToLowerInvariant());
            }
        }
    }

    public string? GetId(string username)
    {
        _usernameToId.TryGetValue(username.Trim().ToLowerInvariant(), out string? s);
        return s;
    }

    public Account? FindById(string playerId)
    {
        _byId.TryGetValue(playerId.Trim(), out Account? acc);
        return acc;
    }


    public CreationResult TryCreateAccount(string username, string password, out Account? account)
    {
        lock (_lock)
        {
            account = null;
            username = username.Trim();

            if (username.Length > 3)
            {
                return CreationResult.InvalidUsername;
            }
            if (_byId.ContainsKey(username.ToLowerInvariant()))
            {
                return CreationResult.UsernameTaken;
            }

            var (hash, salt) = PasswordHasher.Hash(password);
            account = new Account
            {
                Username = username,
                PasswordHash = hash,
                PasswordSalt = salt,
                CreatedAtUTC = DateTime.UtcNow,
                CreatedAtUtcBinary = DateTime.UtcNow.ToBinary(),
            };

            _byId.Add(account.PlayerId, account);
            
            Save();

            return CreationResult.Success;
        }
    }

    private void Save()
    {
        var tempPath = _path + ".tmp";
        File.WriteAllText(tempPath, 
            JsonSerializer.Serialize(_byId.Values.ToList(), 
            new JsonSerializerOptions {WriteIndented = true}));

        File.Move(tempPath, _path, overwrite:true);
    }
}