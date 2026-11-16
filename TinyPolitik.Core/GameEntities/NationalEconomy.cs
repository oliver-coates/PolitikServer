namespace PolitikServer.Core;

/// <summary>
/// Game Entity representing the economy of a nation.
/// </summary>
public class NationalEconomy : GameEntity
{
    public required int money;

    public required int powerGeneration;
    public required int powerConsumption;


    [Newtonsoft.Json.JsonConstructor]
    public NationalEconomy(string UniqueIdentifier, int money, int powerGeneration, int powerConsumption) : base(UniqueIdentifier)
    {
        this.money = money;

        this.powerGeneration = powerGeneration;
        this.powerConsumption = powerConsumption;
    }


    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
    public NationalEconomy() : base(Guid.NewGuid().ToString())
    {
        money = 0;

        powerGeneration = 0;
        powerConsumption = 0;
    }


    public override string GetReadableName()
    {
        return "National Economy";
    }

    internal override void Deserialize()
    {
        
    }
}