
namespace PolitikServer.Core;

/// <summary>
/// Responsible for initiating a turn & controlling turn order execution logic.
/// </summary>
public class TurnProcessor
{
    private readonly IServiceProvider _services;
    private readonly ILogger<TurnProcessor> _logger;
    private readonly LogWriter _writer;
    private readonly GameConfig _config;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public TurnProcessor(IServiceProvider services, ILogger<TurnProcessor> logger, LogWriter writer, GameConfig config)
    {
        _services = services;
        _logger = logger;
        _writer = writer;
        _config = config;
    }

    public async Task ProcessTurnAsync()
    {
        if (!await _lock.WaitAsync(0))
        {
            _logger.LogWarning("Turn is already being processed - ingoring the concurrent trigger");
            return;
        }

        using var scope = _services.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<TurnManager>();
        var resolver = scope.ServiceProvider.GetRequiredService<TurnResolver>();
        var backup = scope.ServiceProvider.GetRequiredService<TurnBackupManager>();
        var entities = scope.ServiceProvider.GetRequiredService<EntityLibrary>();
        var logWriter = scope.ServiceProvider.GetRequiredService<LogWriter>();


        // TODO: Shutoff request handling here.

        // Calculate next turn:
        try
        {
            resolver.NextTurn();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Turn resolution failed.");
        }

        // Make backup:
        try
        {
            string json = entities.GetAllEntitiesAsJson();
            backup.MakeTurnBackup(manager.turnNumber, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Turn backup failed.");
        }
        
        // File all the logging info into its own decidated log file (e.g. turn-log-12 for turn 12) - 
        // all new events will now be saved under 'current.log' until we process the next turn 
        logWriter.SaveTurn(manager.turnNumber);

        // Tick the turn manager over to the next turn - all events have been processed and logged!
        manager.AdvanceToNextTurn();

        if (_config.AllowRealTimePlay)
        {
            ResetNationReadiness(entities);
        }

        // TODO: Reopen requests here

        _lock.Release();
    }

    private void ResetNationReadiness(EntityLibrary entities)
    {
        foreach (Nation nation in EntityLibrary.GetAllEntitiesOfType<Nation>())
        {
            nation.isReady = false;
        }
    }
}
