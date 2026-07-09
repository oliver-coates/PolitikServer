using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace TinyPolitik.Core;

public class SessionInfo
{
    public string PlayerId;
    public string PlayerName;
    public DateTime ExpiresAtUtc;

    public SessionInfo(string id, string name, DateTime expires)
    {
        PlayerId = id;
        PlayerName = name;
        ExpiresAtUtc = expires;
    }
}

public class SessionStore
{
    private readonly ConcurrentDictionary<string, SessionInfo> _sessions = new();

    public string CreateSession(string playerId, string playerName)
    {
        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

        _sessions[token] = new SessionInfo(playerId, playerName, DateTime.UtcNow.AddDays(7));

        return token;
    }

    public SessionInfo? Validate(string token)
    {
        if (_sessions.TryGetValue(token, out var s) && s.ExpiresAtUtc > DateTime.UtcNow)
        {
            s.ExpiresAtUtc = DateTime.UtcNow.AddDays(7);
            return s;
        }

        return null;
    }
}