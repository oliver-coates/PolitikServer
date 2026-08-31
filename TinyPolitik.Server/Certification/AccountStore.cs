
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
    public const int USERNAME_MIN_CHARS = 3;
    public const int USERNAME_MAX_CHARS = 18;
    public const int PASSWORD_MIN_CHARS = 3;
    public const int PASSWORD_MAX_CHARS = 30;

    public enum CreationResult
    {
        Success = 0,
        UsernameTaken = 1,
        InvalidUsername = 2,
        InvalidPassword = 3,
    }

    private readonly string _path;
    private readonly object _lock = new();
    private Dictionary<string, Account> _accountDictionary = new();

    public AccountStore(string path)
    {
        _path = path;

        if (File.Exists(path))
        {
            var accounts = JsonSerializer.Deserialize<List<Account>>(File.ReadAllText(path)) ?? new();
            _accountDictionary = accounts.ToDictionary(a => a.PlayerId);
        }
    }

    public Account? FindById(string playerId)
    {
        _accountDictionary.TryGetValue(playerId.Trim(), out Account? acc);
        return acc;
    }

    public Account? FindByUsername(string username)
    {
        return _accountDictionary.Values.FirstOrDefault(a => a.Username == username);
    }


    public CreationResult TryCreateAccount(string username, string password, out Account? account)
    {
        lock (_lock)
        {
            account = null;
            username = username.Trim();

            if (!ValidateUsername(username))
            {
                return CreationResult.InvalidUsername;
            }
            if (!ValidatePassword(password))
            {
                return CreationResult.InvalidPassword;
            }
            if (_accountDictionary.Values.FirstOrDefault(a => a.Username == username) != null)
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

            _accountDictionary.Add(account.PlayerId, account);
            
            Save();

            return CreationResult.Success;
        }
    }

    private void Save()
    {
        var tempPath = _path + ".tmp";
        File.WriteAllText(tempPath, 
            JsonSerializer.Serialize(_accountDictionary.Values.ToList(), 
            new JsonSerializerOptions {WriteIndented = true}));

        File.Move(tempPath, _path, overwrite:true);
    }

    private bool ValidateUsername(string username)
    {
        if (username.Length < USERNAME_MIN_CHARS || username.Length > USERNAME_MAX_CHARS)
        {
            return false;
        }

        if (username.Any(ch => !char.IsLetterOrDigit(ch)))
        {
            return false;
        }

        return true;
    }

    private bool ValidatePassword(string password)
    {
        if (password.Length < PASSWORD_MIN_CHARS || password.Length > PASSWORD_MAX_CHARS)
        {
            return false;
        }       

        return true;
    }
}