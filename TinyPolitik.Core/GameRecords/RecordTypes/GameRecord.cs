namespace PolitikServer.Core;

[Newtonsoft.Json.JsonObject]
public abstract class GameRecord
{
    [Newtonsoft.Json.JsonRequired] public required string UniqueIdentifier;

    internal abstract void Deserialize();

    public GameRecord(string UniqueIdentifier)
    {
        this.UniqueIdentifier = UniqueIdentifier;
    }

    public sealed override string ToString()
    {
        return $"({UniqueIdentifier}) {GetReadableName()}";
    }

    public abstract string GetReadableName();
}   