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
        var backup = scope.ServiceProvider.GetRequiredService<TurnBackup>();
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
            _logger.LogError(ex, "Turn resolution failed at {Time:u}.", manager.NextTurnTime);
        }

        // Make backup:
        try
        {
            backup.MakeBackup();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Turn backup failed at {Time:u}.", manager.NextTurnTime);
        }
        
        // File all the logging info into its own decidated log file (e.g. turn-log-12 for turn 12) - 
        // all new events will now be saved under 'current.log' until we process the next turn 
        logWriter.SaveTurn(manager.TurnMetaData.turnNumber);
        
        // Tick the turn manager over to the next turn - all events have been processed and logged!
        manager.AdvanceToNextTurn();
        _logger.LogInformation("Next turn scheduled for {Time:u}", manager.NextTurnTime);        
    
        // TODO: Reopen requests here
    }
    
}