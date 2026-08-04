namespace PolitikServer.Core;

public abstract class GameDefinition
{
    public readonly string UniqueIdentifier;

    public GameDefinition(string UniqueIdentifier)
    {
        this.UniqueIdentifier = UniqueIdentifier;
    }
}

public interface IRequiresLateDeserialization
{
    public void LateDeserialize();
}