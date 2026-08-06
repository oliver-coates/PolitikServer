namespace PolitikServer.Core.Serialization;

public class SerializedBuildingType : SerializedGameDefinition
{
    public int baseBuildCost;
    public int upkeepCost;
    public int powerDrawBase;
    public int maxLevelBase;
    public string[] productionModes = [];

    public override GameDefinition Deserialize()
    {
        ProductionMode[] modes = DefinitionLibrary.GetDefinitonsByUid<ProductionMode>(productionModes).ToArray();

        return new BuildingType(_uniqueIdentifier, baseBuildCost, upkeepCost, powerDrawBase, maxLevelBase, modes);
    }
}