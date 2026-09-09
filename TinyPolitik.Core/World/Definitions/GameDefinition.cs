namespace PolitikServer.Core;

public abstract class GameDefinitionOld : ISerializableObject
{
    public string UniqueIdentifier { get; private set; }

    public GameDefinitionOld(string UniqueIdentifier)
    {
        this.UniqueIdentifier = UniqueIdentifier;
    }

    public virtual void LateDeserialize() {}
}