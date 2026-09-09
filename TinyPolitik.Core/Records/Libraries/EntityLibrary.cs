namespace PolitikServer.Core;

public class EntityLibrary : GameLibrary<GameEntity>
{
    // This method is used by SerializedFields when loading in,
    // This should not be called by regular classes, who should be relying on DI
    public static new GameEntity[] Get(IReadOnlyList<string> uids)
    {
        return Instance?.Get(uids) ?? throw new Exception("Entity library is null.");
    }
    
    public static new GameEntity Get(string uid)
    {
        return Instance?.Get(uid) ?? throw new Exception("Entity library is null.");
    }
}