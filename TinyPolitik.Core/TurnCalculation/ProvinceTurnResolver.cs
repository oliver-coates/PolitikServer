using Microsoft.Extensions.Logging;
using PolitikServer.Core;


namespace PolitikServer.TurnResolution;

public class ProvinceTurnResolver
{
    private readonly ILogger<ProvinceTurnResolver> _logger;

    private readonly EntityLibrary _entities;

    private readonly ProvinceDevelopmentResolver _provinceDevelopmentResolver;

    public ProvinceTurnResolver(ILogger<ProvinceTurnResolver> logger, EntityLibrary entityLibrary, ProvinceDevelopmentResolver provinceDevelopmentResolver)
    {
        _logger = logger;
        _entities = entityLibrary;

        _provinceDevelopmentResolver = provinceDevelopmentResolver;
    }

    /// <summary>
    /// Grows/Shrinks the the population of each province based on the modifiers.
    /// </summary>
    public void TickPopulation()
    {
        ProvinceEntity[] provinces = _entities.GetAll<ProvinceEntity>();
        float baseGrowth = WorldConfig.GetFloat("province_population_growth_base");
        int minPop = WorldConfig.GetInt("province_min_population");
        int maxPop = WorldConfig.GetInt("province_max_population");

        foreach (var province in provinces)
        {
            var developmentLevel = province.developmentLevel.Get();

            float growthRateBase = 1f + baseGrowth; // Healthcare, province modifiers, etc will go here.

            // Only apply the development level boost if we are gaining population.
            float growthRateFinal;
            if (growthRateBase > 0)
            {
                growthRateFinal = growthRateBase * developmentLevel.PopulationGrowthMultiplier;
            } 
            else
            {
                growthRateFinal = growthRateBase;
            }

            int newPopulation = (int) (province.population * growthRateFinal);

            if (maxPop > minPop)
            {
                newPopulation = int.Clamp(newPopulation, minPop, maxPop);
            }
            else
            {
                newPopulation = int.Clamp(newPopulation, minPop, int.MaxValue);   
            }

            province.population = newPopulation;
        }
    }

    /// <summary>
    /// Updates the province development level of each province based on its population.
    /// </summary>
    public void TickProvinceLevel()
    {
        ProvinceEntity[] provinces =_entities.GetAll<ProvinceEntity>();

        foreach (ProvinceEntity province in provinces)
        {
            ProvinceDevelopmentLevel currentLevel = province.developmentLevel.Get();
            ProvinceDevelopmentLevel correctLevel = _provinceDevelopmentResolver.Evaluate(province.population);

            if (currentLevel == correctLevel)
            {
                continue;
            }

            province.developmentLevel.Set(correctLevel);
            string provinceUid = province.definition.Get().UniqueIdentifier;

            _logger.LogInformation($"Province '{provinceUid}' development level has changed (old: '{currentLevel.UniqueIdentifier}', new: '{correctLevel.UniqueIdentifier}')");
        }
    }

    /// <summary>
    /// Updates the province's occupation bool to match whether it is currently occupied. 
    /// </summary>
    public void TickOccupation()
    {
        foreach (ProvinceEntity province in _entities.GetAll<ProvinceEntity>())
        {
            province.isOccupied = province.ownerNation.Get() != province.occupierNation.Get();
        }
    }
}