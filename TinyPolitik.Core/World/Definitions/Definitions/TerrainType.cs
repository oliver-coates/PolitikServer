namespace PolitikServer.Core;

public class TerrainType : GameDefinition
{
    public TerrainType(string UniqueIdentifier) : base(UniqueIdentifier)
    {
    }

    public override string ToString()
    {
        return $"[{UniqueIdentifier}] Terrain Type.";
    }
}