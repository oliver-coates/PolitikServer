namespace TinyPolitik.Core;

public record LoginRequest(string Password, string PlayerId, string PlayerName);
public record LoginResponse(string Token, string PlayerId, string PlayerName, string ExistingNationId);

public static class LoginHandler
{
    public static IResult Login(HttpContext context, LoginRequest request, SessionStore sessions, LoginRateLimiter limiter)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        if (limiter.IsLocked(ip))
        {
            return Results.StatusCode(StatusCodes.Status429TooManyRequests);
        }

        if (string.IsNullOrWhiteSpace(request.Password) || PasswordHasher.Verify(request.Password, "123", "456") == false)
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
        string playerName = "testificate";
        string playerId = "999";
        string? nationId = null;

        var token = sessions.CreateSession(playerId, playerName);

        return Results.Json(new LoginResponse(token, playerId, playerName, nationId));
    }
}