using System.Collections.Concurrent;

namespace TinyPolitik.Core;

public class LoginRateLimiter
{
    private const int MAXIMUM_LOGIN_ATTEMPTS = 5;
    private const int LOCKOUT_TIME_SECONDS = 60;

    private class Attempt
    {
        public int Count;
        public DateTime LockedUntilUtc;
    }

    private readonly ConcurrentDictionary<string, Attempt> _attempts = new();

    public bool IsLocked(string ip)
    {
        return _attempts.TryGetValue(ip, out var a) && a.LockedUntilUtc > DateTime.UtcNow; 
    }

    public void RecordFailure(string ip)
    {
        Attempt a = _attempts.GetOrAdd(ip, new Attempt());
        a.Count ++;

        if (a.Count >= MAXIMUM_LOGIN_ATTEMPTS)
        {
            a.LockedUntilUtc = DateTime.UtcNow.AddSeconds(LOCKOUT_TIME_SECONDS);
            a.Count = 0;
        }
    }

    public void RecordSuccess(string ip)
    {
        _attempts.TryRemove(ip, out _);
    }
}