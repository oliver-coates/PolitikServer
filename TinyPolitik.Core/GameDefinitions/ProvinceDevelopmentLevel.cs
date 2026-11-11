namespace PolitikServer.Core;

public class ProvinceDevelopmentLevel : GameDefinition
{
    public readonly int PopulationThreshold;   
    public readonly int ResourcesProvided;
    public readonly int BuildingSlots;
    public readonly int PowerProduced;
    public readonly float PopulationGrowthMultiplier;

    public ProvinceDevelopmentLevel(string UniqueIdentifier, int PopulationThreshold, int ResourcesProvided, int BuildingSlots, int PowerProduced, float PopulationGrowthMultiplier) : base(UniqueIdentifier)
    {
        this.PopulationThreshold = PopulationThreshold;
        this.ResourcesProvided = ResourcesProvided;
        this.BuildingSlots = BuildingSlots;
        this.PowerProduced = PowerProduced;
        this.PopulationGrowthMultiplier = PopulationGrowthMultiplier;
    }

    internal override void Deserialize()
    {}

    protected override string GetFullName()
    {
        return $"Development Level. Starts above {PopulationThreshold} population, resources: {ResourcesProvided}, slots: {BuildingSlots}, power: {PowerProduced}, multiplier: x{PopulationGrowthMultiplier}";
    }


    public override string GetReadableName()
    {
        return "Development Level.";
    }

    /// <summary>
    /// Takes an array of province development levels and a population and returnss the development level that this population is at.
    /// </summary>
    public static ProvinceDevelopmentLevel Evaluate(ProvinceDevelopmentLevel[] levels, int population)
    {
        // Order development levels by the ones with the highest population thresholds first.
        ProvinceDevelopmentLevel[] orderedLevels = levels.OrderBy(s => s.PopulationThreshold).ToArray();
        
        foreach (ProvinceDevelopmentLevel level in orderedLevels)
        {
            // If the population exceeds this threshold, this is the correct development level
            if (population >= level.PopulationThreshold)
            {
                return level;
            }
        }

        // If all else fails, return the last (lowest threshold) level.
        return orderedLevels[^1];
    }

    
}