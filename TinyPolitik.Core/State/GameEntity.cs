namespace PolitikServer.Core;

public abstract class GameEntity : ISerializableObject
{
    public string UniqueIdentifier { get; init; } = "";
}
