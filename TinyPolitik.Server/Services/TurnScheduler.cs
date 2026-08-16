using TinyPolitik.Core;

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

        if (DateTime.UtcNow < manager.NextTurnTime)
        {
            // Turn not scheduled yet.
            return;
        }

        _logger.LogInformation("Turn boundary reached ({Time:u}) -- Advancing turn.", manager.NextTurnTime);

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

        manager.AdvanceToNextTurn();
        _logger.LogInformation("Next turn scheduled for {Time:u}", manager.NextTurnTime);
    }
    
}