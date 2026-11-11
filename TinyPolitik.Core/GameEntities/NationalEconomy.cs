namespace PolitikServer.Core;

/// <summary>
/// Game Entity representing the economy of a nation.
/// </summary>
public class NationalEconomy : GameEntity
{
    public NationalEconomy(string UniqueIdentifier) : base(UniqueIdentifier)
    {
    }

    public override string GetReadableName()
    {
        return "Not yet implemented";
    }

    internal override void Deserialize()
    {
        
    }
}