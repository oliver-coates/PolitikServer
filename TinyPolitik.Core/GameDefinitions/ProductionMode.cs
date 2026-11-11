namespace PolitikServer.Core;

public class ProductionMode : GameDefinition
{
    public readonly SerializedList<StrategicResource> ResourcesConsumed;
    public readonly SerializedList<StrategicResource> ResourcesProduced;
    public readonly int PowerDraw;
    public readonly int PowerGenerated;

    public ProductionMode(string UniqueIdentifier, SerializedList<StrategicResource> ResourcesConsumed, SerializedList<StrategicResource> ResourcesProduced, int PowerDraw, int PowerGenerated) : base(UniqueIdentifier)
    {
        this.ResourcesConsumed = ResourcesConsumed;
        this.ResourcesProduced = ResourcesProduced;
        this.PowerDraw = PowerDraw;
        this.PowerGenerated = PowerGenerated;
    }

    public override string GetReadableName()
    {
        return "Production Mode";
    }

    protected override string GetFullName()
    {
        return $"Production Mode. Inputs: [{string.Join(',', ResourcesConsumed.Get().Select(o => o.UniqueIdentifier))}], Outputs: [{string.Join(',', ResourcesProduced.Get().Select(o => o.UniqueIdentifier))}]. Power: -{PowerGenerated - PowerDraw}";
    }

    internal override void Deserialize()
    {
        ResourcesConsumed.Load();
        ResourcesProduced.Load();
    }
}