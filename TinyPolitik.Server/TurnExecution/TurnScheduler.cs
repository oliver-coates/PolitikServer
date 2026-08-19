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

        if (DateTime.UtcNow < manager.NextTurnTime)
        {
            // Turn not scheduled yet.
            return;
        }

        _logger.LogInformation("Turn boundary reached ({Time:u}) -- Advancing turn.", manager.NextTurnTime);

        await scope.ServiceProvider.GetRequiredService<TurnProcessor>().ProcessTurnAsync();

        _logger.LogInformation("Next turn scheduled for {Time:u}", manager.NextTurnTime);        
    
    }    
}