using Microsoft.Extensions.Logging;

namespace PolitikServer.Core;

public class NationClaimManager
{
    public enum ClaimAttemptResult
    {
        Success = 0,
        NationNotFound = 1,
        PlayerNotFound = 2,
        AlreadyClaimed = 3,         
        ThisPlayerAlreadyHasNation = 4
    }

    public enum AbandonAttemptResult
    {
        Success = 0,
        NationNotFound = 1,
        PlayerNotFound = 2,
        DoesNotControlNation = 3   
    }

    private readonly EntityLibrary _entityLib;
    private readonly ILogger<NationClaimManager> _logger;
    private readonly AccountStore _accountStore;

    private readonly object _claimAttemptLock = new(); // Lock object to prevent two players from claiming the exact same nation.

    public NationClaimManager(EntityLibrary entityLib, ILogger<NationClaimManager> logger, AccountStore accountStore)
    {
        _entityLib = entityLib;
        _logger = logger;
        _accountStore = accountStore;
    }

    public string[] GetAllUnclaimedNationIds()
    {
        // Gets all nation IDs without a player id assigned.
        return _entityLib.GetAll<Nation>().Where(n => n.playerId == null).Select(n => n.UniqueIdentifier).ToArray();
    }

    public ClaimAttemptResult TryClaimNation(string claimingPlayerId, string nationId)
    {
        lock (_claimAttemptLock)
        {
            Nation? nation = _entityLib.TryGet<Nation>(nationId); 
            Account? account = _accountStore.FindById(claimingPlayerId);
            
            if (account == null)
            {
                return ClaimAttemptResult.PlayerNotFound;
            }

            if (nation == null)
            {
                return ClaimAttemptResult.NationNotFound;
            }

            if (!string.IsNullOrEmpty(nation.playerId))
            {
                return ClaimAttemptResult.AlreadyClaimed;
            }

            bool playerAlreadyHasNation = _entityLib.GetAll<Nation>().Any(n => n.playerId == claimingPlayerId);
            if (playerAlreadyHasNation)
            {
                return ClaimAttemptResult.ThisPlayerAlreadyHasNation;
            }

            nation.playerId = claimingPlayerId;
            _logger.LogInformation("Player {player} has claimed nation {nation}", account.Username, nation.GetReadableName());
            
            account.NationId = nation.UniqueIdentifier;
            _accountStore.Save();

            return ClaimAttemptResult.Success;
        }
    }

    public AbandonAttemptResult TryAbandonNation(string abandoningPlayerId, string nationId)
    {
        lock (_claimAttemptLock)
        {
            Nation? nation = _entityLib.TryGet<Nation>(nationId);
            Account? account = _accountStore.FindById(abandoningPlayerId);

            if (account == null)
            {
                return AbandonAttemptResult.PlayerNotFound;
            }

            if (nation == null)
            {
                return AbandonAttemptResult.NationNotFound;   
            }

            if (nation.playerId != abandoningPlayerId)
            {
                return AbandonAttemptResult.DoesNotControlNation;
            }

            _logger.LogInformation("Player '{player}' has abandoned nation '{nation}'", account.Username, nation.GetReadableName());
            
            nation.playerId = null;
            account.NationId = "null";

            _accountStore.Save();
            return AbandonAttemptResult.Success;
        }
    }

    public string GetRandomCountryLeaderTitle()
    {
        return RandomUtil.Pick([
            "Supreme Leader", 
            "President", 
            "Prime Minister",
            "Your Highness",
            "Chairman"]);
    }
}
