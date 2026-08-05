namespace PolitikServer.Core;

public class BuildingType : GameDefinition
{
    public readonly int baseBuildCost;
    public readonly int upkeepCost;

    public readonly int powerDrawBase;
    
    public readonly int maxLevelBase;

    public readonly ProductionMode[] productionModes;

    public BuildingType(string UniqueIdentifier, int buildCost, int upkeepCost, int power, int maxLevel, ProductionMode[] modes) : base(UniqueIdentifier)
    {
        baseBuildCost = buildCost;
        this.upkeepCost = upkeepCost;
        powerDrawBase = power;
        maxLevelBase = maxLevel;
        productionModes = modes;
    }

    public override string ToString()
    {
        return $"[{UniqueIdentifier}] Building Type. Build Cost: {baseBuildCost}, Upkeep: {upkeepCost}, Powerdraw: {powerDrawBase}, Max Level (before tech) {maxLevelBase}, Production Modes: {string.Join(',',productionModes.Select(p => p.UniqueIdentifier))} ";
    }
}