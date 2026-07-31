namespace PolitikServer.Core;

public abstract class GameDefinition
{
    public required string UniqueIdentifier;
}

public interface IRequiresLateDeserialization
{
    public void LateDeserialize();
}