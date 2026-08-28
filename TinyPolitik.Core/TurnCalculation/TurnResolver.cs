using Microsoft.Extensions.Logging;

namespace PolitikServer.Core;

public class TurnResolver
{
    private readonly ILogger<TurnResolver> _logger;
    
    private readonly ProvinceTurnResolver _provinceTurnResolver;


    public TurnResolver(ILogger<TurnResolver> logger, ProvinceTurnResolver provinceTurnResolver)
    {
        _logger = logger;
        _provinceTurnResolver = provinceTurnResolver;

    }

    public void NextTurn()
    {
        _logger.LogInformation("Starting turn resolution...");

        _provinceTurnResolver.TickPopulation();
        _provinceTurnResolver.TickProvinceLevel();

        _logger.LogInformation("...Turn resolution finished.");
    }
}