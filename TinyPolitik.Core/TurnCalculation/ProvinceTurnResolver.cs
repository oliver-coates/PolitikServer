using Microsoft.Extensions.Logging;

namespace PolitikServer.Core;

public class ProvinceTurnResolver
{
    private readonly ILogger<ProvinceTurnResolver> _logger;

    private readonly DefinitionLibrary _definitions;
    private readonly EntityLibrary _entities;

    public ProvinceTurnResolver(ILogger<ProvinceTurnResolver> logger, DefinitionLibrary definitions, EntityLibrary entityLibrary)
    {
        _logger = logger;
        _definitions = definitions;
        _entities = entityLibrary;

    }

    /// <summary>
    /// Grows/Shrinks the the population of each province based on the modifiers.
    /// </summary>
    public void TickPopulation()
    {
        ProvinceEntity[] provinces = EntityLibrary.GetAllEntitiesOfType<ProvinceEntity>();
        float baseGrowth = WorldConfig.GetFloat("province_growth_base");
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
                growthRateFinal = growthRateBase * developmentLevel.popGrowthMulitplier;
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
        ProvinceDevelopmentLevel[] developmentLevels = DefinitionLibrary.GetAllDefinitionsOfType<ProvinceDevelopmentLevel>();
        ProvinceEntity[] provinces = EntityLibrary.GetAllEntitiesOfType<ProvinceEntity>();

        foreach (ProvinceEntity province in provinces)
        {
            ProvinceDevelopmentLevel currentLevel = province.developmentLevel.Get();
            ProvinceDevelopmentLevel correctLevel = ProvinceDevelopmentLevel.Evaluate(developmentLevels, province.population);

            if (currentLevel == correctLevel)
            {
                continue;
            }

            province.developmentLevel.Set(correctLevel);
            string provinceUid = province.province.Get().UniqueIdentifier;

            _logger.LogInformation($"Province '{provinceUid}`' development level has changed (old: '{currentLevel.UniqueIdentifier}', new: '{correctLevel.UniqueIdentifier}')");
        }
    }
}