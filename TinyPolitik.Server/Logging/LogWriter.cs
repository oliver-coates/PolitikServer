
namespace PolitikServer.Core;

/// <summary>
/// Writes each log event to a log file as each event comes in.
/// </summary>
public class LogWriter : IDisposable
{
    private readonly string _logsRoot;
    private readonly object _lock = new();
    private StreamWriter _writer;

    public LogWriter(string root)
    {
        _logsRoot = root;
        
        // Ensure dir exists:
        try
        {
            Directory.CreateDirectory(_logsRoot);        
        }
        catch (Exception ex)
        {
            throw new Exception($"Given log root directory of {root} is not valid: {ex}");
        }
    
        _writer = OpenCurrent();
    }
    
    // The 'temp path' that each incoming log is written to. Renamed to 'log-turn-{turnNumber}' once the turn occured
    private string GetCurrentPath()
    {
        return Path.Combine(_logsRoot, "current.log");
    }

    private StreamWriter OpenCurrent()
    {
        return new(new FileStream(GetCurrentPath(), FileMode.Append, FileAccess.Write, FileShare.Read))
        {
            AutoFlush = true,
        };
    }

    public void WriteLine(string line)
    {
        lock (_lock) { _writer.WriteLine(line); }
    }

    public void SaveTurn(int turnNumber)
    {
        lock (_lock)
        {
            _writer.Dispose();

            var finalPath = Path.Combine(_logsRoot, $"log-turn-{turnNumber}.log");
            if (File.Exists(finalPath))
            {
                File.Delete(finalPath);
            }
            File.Move(GetCurrentPath(), finalPath);

            _writer = OpenCurrent();
        }
    }

    public void Dispose()
    {
        _writer.Dispose();
    }
}