namespace PolitikServer.Core;

public class NationClaimManager
{
    public enum ClaimAttemptResult
    {
        Success,
        NotFound,
        AlreadyClaimed,         
        ThisPlayerAlreadyHasNation
    }

    private EntityLibrary _entityLib;

    private readonly object _claimAttemptLock = new(); // Lock object to prevent two players from claiming the exact same nation.

    public NationClaimManager(EntityLibrary entityLib)
    {
        _entityLib = entityLib;
    }

    public string[] GetAllUnclaimedNationIds()
    {
        // Gets all nation IDs without a player id assigned.
        return EntityLibrary.GetAllEntitiesOfType<Nation>().Where(n => n.playerId == null).Select(n => n.UniqueIdentifier).ToArray();
    }

    public ClaimAttemptResult TryClaimNation(string claimingPlayerId, string nationId)
    {

        lock (_claimAttemptLock)
        {
            Nation? nation = _entityLib.TryGetEntity<Nation>(nationId); 
            
            if (nation == null)
            {
                return ClaimAttemptResult.NotFound;
            }

            if (!string.IsNullOrEmpty(nation.playerId))
            {
                return ClaimAttemptResult.AlreadyClaimed;
            }

            bool playerAlreadyHasNation = EntityLibrary.GetAllEntitiesOfType<Nation>().Any(n => n.playerId == claimingPlayerId);
            if (playerAlreadyHasNation)
            {
                return ClaimAttemptResult.ThisPlayerAlreadyHasNation;
            }

            nation.playerId = claimingPlayerId;
            return ClaimAttemptResult.Success;
        }
    }

    public string GetRandomCountryLeaderTitle()
    {
        return RandomUtil.Pick([
            "Supreme Leader", 
            "President", 
            "Prime Minister",
            "Your Highness",
            "Supreme Chairman"]);
    }
}
