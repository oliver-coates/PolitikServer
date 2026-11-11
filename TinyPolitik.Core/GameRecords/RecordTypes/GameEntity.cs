namespace PolitikServer.Core;

public abstract class GameEntity : GameRecord
{
    protected GameEntity(string UniqueIdentifier) : base(UniqueIdentifier) {}
}
