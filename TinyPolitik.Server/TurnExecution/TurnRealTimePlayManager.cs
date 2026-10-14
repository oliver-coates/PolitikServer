
namespace PolitikServer.Core;

public class TurnRealTimePlayManager
{
    private ILogger<TurnRealTimePlayManager> _logger;

    public TurnRealTimePlayManager(ILogger<TurnRealTimePlayManager> logger)
    {
        _logger = logger;
    }

    public async Task<IResult> ReadyNation(HttpContext ctx, EntityLibrary entities, ServerConfig config, TurnProcessor turnProcessor)
    {
        var session = ctx.GetSession();
        
        if (!config.AllowRealTimePlay)
        {
            return Results.BadRequest("This server does not allow real-time-play.");
        }

        Nation? nation = entities.TryGet<Nation>(session.PlayerId);
        
        if (nation is null)
        {
            return Results.BadRequest($"No nation exists for this player (UID: {session.PlayerId}).");
        }
        
        nation.isReady = true;
        _logger.LogInformation("Player '{player}' ({nation}) is ready!", nation.playerId, nation.nameLong);        


        if (AreAllNationsReady(entities))
        {
            _logger.LogInformation("All players are ready, processing turn...");        

            await turnProcessor.ProcessTurnAsync();

            _logger.LogInformation("Real time play is enabled! The turn will process once all players are ready.");            
        }

        return Results.Ok();
    }

    private bool AreAllNationsReady(EntityLibrary entities)
    {
        int playerNationCount = 0;

        foreach (Nation nation in entities.GetAll<Nation>())
        {
            // Ensure all player run countries are ready
            if (nation.playerId != null)
            {
                playerNationCount++;
                if (!nation.isReady)
                {
                    return false;
                }
            }
        }

        // If there aren't any nations which have players, don't allow a turn advance - we should be waiting for players to join and ready up.
        if (playerNationCount == 0)
        {
            return false;
        }

        return true;
    }

    public IResult GetReadiness(EntityLibrary entities)
    {
        var nations = entities.GetAll<Nation>();
        
        int numNations = 0;
        int numNationsReady = 0;
        foreach (Nation nation in nations)
        {
            if (nation.playerId != null)
            {
                numNations += 1;
                if (nation.isReady)
                {
                    numNationsReady += 1;
                }
            }
        }

        return Results.Json(new
        {
            totalCount = numNations,
            readyCount = numNationsReady,
            nations = nations.Where(n => n.playerId != null).Select(n => new {id = n.UniqueIdentifier, name = n.nameShort, ready = n.isReady})
        });
    }
}