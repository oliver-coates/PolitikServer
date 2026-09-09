namespace PolitikServer.Core;

public class ProductionMode : GameDefinition
{
    public readonly StrategicResource[] consumed;
    public readonly StrategicResource[] produced;
    public readonly int powerDraw;
    public readonly int powerGain;

    public ProductionMode(string UniqueIdentifier, StrategicResource[] consumed, StrategicResource[] produced, int draw, int gain) : base(UniqueIdentifier)
    {
        this.consumed = consumed;
        this.produced = produced;
        powerDraw = draw;
        powerGain = gain;
    }

    public override string ToString()
    {
        return $"[{UniqueIdentifier}] Production Mode. Inputs: [{string.Join(',', consumed.Select(o => o.UniqueIdentifier))}], Outputs: [{string.Join(',', produced.Select(o => o.UniqueIdentifier))}]. Power: -{powerDraw}/+{powerGain}";
    }
}