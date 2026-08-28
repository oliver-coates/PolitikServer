namespace PolitikServer.Core;

public class ProvinceDevelopmentLevel : GameDefinition
{
    public readonly int populationThreshold;   
    public readonly int resourcesProvided;
    public readonly int buildingSlots;
    public readonly int powerProduced;
    public readonly float popGrowthMulitplier;

    public ProvinceDevelopmentLevel(string UniqueIdentifier, int pop, int resources, int slots, int power, float growth) : base(UniqueIdentifier)
    {
        populationThreshold = pop;
        resourcesProvided = resources;
        buildingSlots = slots;
        powerProduced = power;
        popGrowthMulitplier = growth;
    }

    public override string ToString()
    {
        return $"[{UniqueIdentifier}] Development Level. Starts above {populationThreshold} population, resources: {resourcesProvided}, slots: {buildingSlots}, power: {powerProduced}, multiplier: x{popGrowthMulitplier}";
    }

    public static ProvinceDevelopmentLevel Evaluate(ProvinceDevelopmentLevel[] levels, int population)
    {
        // Order development levels by the ones with the highest population thresholds first.
        ProvinceDevelopmentLevel[] orderedLevels = levels.OrderBy(s => s.populationThreshold).ToArray();
        foreach (ProvinceDevelopmentLevel level in orderedLevels)
        {
            // If the population exceeds this threshold, this is the correct development level
            if (population >= level.populationThreshold)
            {
                return level;
            }
        }

        // If all else fails, return the last (lowest threshold) level.
        return orderedLevels[^1];
    }
}