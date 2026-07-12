namespace TinyPolitik.Core;

public record LoginRequest(string Password, string PlayerId);
public record LoginResponse(string Token, string PlayerId, string ExistingNationId);

public static class LoginHandler
{
    public static IResult Login(HttpContext context, LoginRequest request, SessionStore sessions, LoginRateLimiter limiter, GameConfig config)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        if (limiter.IsLocked(ip))
        {
            return Results.StatusCode(StatusCodes.Status429TooManyRequests);
        }

        if (string.IsNullOrWhiteSpace(request.Password) || PasswordHasher.Verify(request.Password, config.PasswordHash ?? "", config.PasswordSalt ?? "") == false)
        {
            limiter.RecordFailure(ip);
            return Results.Unauthorized();
        }
        else
        {
            limiter.RecordSuccess(ip);
        }

        if (string.IsNullOrWhiteSpace(request.PlayerId))
        {
            return Results.BadRequest("Missing Player ID");
        }

        // var player = // Create player here and save into world state
        // For now create dummy player:
        string playerId = "999";
        string? nationId = "not yet set";

        var token = sessions.CreateSession(playerId);

        return Results.Json(new LoginResponse(token, playerId, nationId));
    }
}