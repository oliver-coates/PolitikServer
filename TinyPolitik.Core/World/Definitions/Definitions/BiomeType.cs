namespace PolitikServer.Core;

public class BiomeType : GameDefinitionOld
{
    public BiomeType(string UniqueIdentifier) : base(UniqueIdentifier)
    {
    }

    public override string ToString()
    {
        return $"[{UniqueIdentifier}] Biome Type";
    }
}