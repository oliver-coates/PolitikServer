namespace PolitikServer.Core;

public class TurnSchedulerService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<TurnSchedulerService> _logger;
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(15);
        
    public TurnSchedulerService(IServiceProvider services, ILogger<TurnSchedulerService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(PollInterval);

        // Instantly check for the next turn, this should instantly fire off a turn if the server has just been initialised.
        await CheckForNextTurnAsync();

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await CheckForNextTurnAsync();
        }
    }

    private async Task CheckForNextTurnAsync()
    {
        using var scope = _services.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<TurnManager>();
        var resolver = scope.ServiceProvider.GetRequiredService<TurnResolver>();
        var backup = scope.ServiceProvider.GetRequiredService<TurnBackupManager>();
        var entities = scope.ServiceProvider.GetRequiredService<EntityLibrary>();
        var logWriter = scope.ServiceProvider.GetRequiredService<LogWriter>();


        if (DateTime.UtcNow < manager.NextTurnTime)
        {
            // Turn not scheduled yet.
            return;
        }

        _logger.LogInformation("Turn boundary reached ({Time:u}) -- Advancing turn.", manager.NextTurnTime);

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
        _logger.LogInformation("Next turn scheduled for {Time:u}", manager.NextTurnTime);        
    
        // TODO: Reopen requests here
    }    
}