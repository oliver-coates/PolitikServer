using System.Collections.ObjectModel;

namespace PolitikServer.Core;

/// <summary>
/// Game Entity representing the economy of a nation.
/// </summary>
public class NationalEconomy : GameEntity
{
    public required int money;

    // -- Power:
    public required int powerGeneration;
    public required int powerConsumption;

    /// <summary>
    /// What percentage of this nation's power requirement has been fulfilled.
    /// Between 0.0 = 0%, 1.0 = 100%.
    /// </summary>
    [Newtonsoft.Json.JsonIgnore] public float powerFulfillment;

    
    /// <summary>
    /// Revenue gained passively from controlled provinces.
    /// </summary>
    public required int passiveMoney;

    /// <summary>
    /// Power gained passively from controlled provinces.
    /// </summary>
    public required int passivePower;

    /// <summary>
    /// Dictionary of all strategic resources in the game and how many we producted this turn
    /// </summary>
    [Newtonsoft.Json.JsonIgnore] public required Dictionary<StrategicResource, int> producedResources;


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
        producedResources = [];
    }


    public override string GetReadableName()
    {
        return "National Economy";
    }

    internal override void Deserialize()
    {
        
    }
}