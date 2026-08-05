namespace PolitikServer.Core;

public abstract class GameDefinition
{
    public readonly string UniqueIdentifier;

    public GameDefinition(string UniqueIdentifier)
    {
        this.UniqueIdentifier = UniqueIdentifier;
    }
   
    public virtual void LateDeserialize() {}
}