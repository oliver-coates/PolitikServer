namespace PolitikServer.Core.Serialization;

public class SerializedProvinceDevelopmentLevel : SerializedGameDefinition
{
    public int populationThreshold;
    public int resourcesProduced;
    public int buildingSlots;
    public int powerProduced;
    public float populationGrowthMultiplier;

    public override GameDefinition Deserialize()
    {
        return new ProvinceDevelopmentLevel(_uniqueIdentifier, populationThreshold, resourcesProduced, buildingSlots, powerProduced, populationGrowthMultiplier);
    }
}