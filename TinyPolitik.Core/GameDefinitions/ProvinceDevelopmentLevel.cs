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

    
}