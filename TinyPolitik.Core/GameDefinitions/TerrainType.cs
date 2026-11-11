namespace PolitikServer.Core;

public class TerrainType : GameDefinition
{
    public TerrainType(string UniqueIdentifier) : base(UniqueIdentifier)
    {
    }

    public override string GetReadableName()
    {
        return $"Terrain Type.";
    }

    protected override string GetFullName()
    {
        return $"Terrain Type.";
    }

    internal override void Deserialize()
    {
    }
}