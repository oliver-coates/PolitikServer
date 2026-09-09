namespace PolitikServer.Core;

public abstract class GameDefinition : GameRecord
{
    protected GameDefinition(string UniqueIdentifier) : base(UniqueIdentifier) {}
}