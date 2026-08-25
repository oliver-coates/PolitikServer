namespace PolitikServer.Core;

public class NationClaimManager
{
    private EntityLibrary _entityLib;

    public NationClaimManager(EntityLibrary entityLib)
    {
        _entityLib = entityLib;
    }

    public string[] GetAllUnclaimedNationIds()
    {
        // Gets all nation IDs without a player id assigned.
        return EntityLibrary.GetAllEntitiesOfType<Nation>().Where(n => n.playerId == null).Select(n => n.UniqueIdentifier).ToArray();
    }
}
