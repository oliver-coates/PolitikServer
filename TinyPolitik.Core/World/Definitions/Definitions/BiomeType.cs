namespace PolitikServer.Core;

public class BiomeType : GameDefinition
{
    public BiomeType(string UniqueIdentifier) : base(UniqueIdentifier)
    {
    }

    protected override string GetFullName()
    {
        return "Biome Type";
    }

    public override string GetReadableName()
    {
        return $"Biome Type";
    }

    internal override void Deserialize()
    {
    }
}