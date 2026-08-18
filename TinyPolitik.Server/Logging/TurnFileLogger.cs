namespace PolitikServer.Core;

public class TurnFileLogger : ILogger
{
    private readonly string _category;
    private readonly LogWriter _writer;

    public TurnFileLogger(string category, LogWriter writer)
    {
        _category = category;
        _writer = writer;
    }
 
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;
        
        var line = $"{DateTime.UtcNow:u} [{logLevel}] {_category}: {formatter(state, exception)}";
        
        if (exception != null)
        {
            line += $"{Environment.NewLine}[EXCEPTION] {exception}";  
        } 
        
        _writer.WriteLine(line);
    }
}

public class TurnFileLoggerProvider : ILoggerProvider
{
    private readonly LogWriter _writer;
    public TurnFileLoggerProvider(LogWriter writer)
    {
        _writer = writer;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new TurnFileLogger(categoryName, _writer);
    }

    public void Dispose()
    {
        _writer.Dispose();
    }
}