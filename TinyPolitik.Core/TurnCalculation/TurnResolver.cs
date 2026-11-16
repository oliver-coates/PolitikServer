using Microsoft.Extensions.Logging;

namespace PolitikServer.TurnResolution;

public class TurnResolver
{
    private readonly ILogger<TurnResolver> _logger;
    
    private readonly ProvinceTurnResolver _provinceResolver;
    private readonly EconomyResolver _economyResolver;


    public TurnResolver(ILogger<TurnResolver> logger, ProvinceTurnResolver provinceTurnResolver, EconomyResolver economyResolver)
    {
        _logger = logger;
        _provinceResolver = provinceTurnResolver;
        _economyResolver = economyResolver;
    }

    public void NextTurn()
    {
        _logger.LogInformation("Starting turn resolution...");

        _provinceResolver.TickOccupation();
        _provinceResolver.TickPopulation();
        _provinceResolver.TickProvinceLevel();

        _economyResolver.Tick();

        _logger.LogInformation("...Turn resolution finished.");
    }
}