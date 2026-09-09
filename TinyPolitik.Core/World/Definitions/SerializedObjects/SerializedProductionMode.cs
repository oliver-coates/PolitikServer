namespace PolitikServer.Core.Serialization;

public class SerializedProductionMode : SerializedGameDefinition
{
    public string[] produced = [];
    public string[] consumed = [];
    public int powerDraw;
    public int powerGain;

    public override GameDefinition Deserialize()
    {
        StrategicResource[] resourcesConsumed = DefinitionLibrary.GetDefinitonsByUid<StrategicResource>(consumed).ToArray();
        StrategicResource[] resourcesProduced = DefinitionLibrary.GetDefinitonsByUid<StrategicResource>(produced).ToArray();
    
        return new ProductionMode(_uniqueIdentifier, resourcesConsumed, resourcesProduced, powerDraw, powerGain);
    }
}