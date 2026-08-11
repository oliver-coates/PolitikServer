namespace PolitikServer.Core;

public abstract class GameDefinition : ISerializableObject
{
    public string UniqueIdentifier { get; private set; }

    public GameDefinition(string UniqueIdentifier)
    {
        this.UniqueIdentifier = UniqueIdentifier;
    }

    public virtual void LateDeserialize() {}
}