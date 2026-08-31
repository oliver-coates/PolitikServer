namespace PolitikServer.Core;

public record AccountRegisterRequest(string InvitePassword, string Username, string Password);
public record AccountLoginRequest(string Username, string Password);
public record AccountAuthenticationResponse(string Token, string PlayerId, string Username, string? ControlledNationId);

public class AccountManager
{
    private readonly SessionStore _sessionStore;
    private readonly GameConfig _config;
    private readonly AccountStore _accounts;
    private readonly LoginRateLimiter _loginLimiter;
    private readonly EntityLibrary _entityLib;

    public AccountManager(SessionStore store, GameConfig config, AccountStore accounts, LoginRateLimiter limiter, EntityLibrary entities)
    {
        _sessionStore = store;
        _config = config;
        _accounts = accounts;
        _loginLimiter = limiter;
        _entityLib = entities;
    }

    public IResult RegisterAccount(HttpContext ctx, AccountRegisterRequest request)
    {
        var ip = ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        
        // Deny locked ips
        if (_loginLimiter.IsLocked(ip))
        {
            return Results.StatusCode(StatusCodes.Status429TooManyRequests);  
        } 

        // Verify password
        if (string.IsNullOrWhiteSpace(request.InvitePassword) || !PasswordHasher.Verify(request.InvitePassword, _config.ServerPasswordHash!, _config.ServerPasswordSalt!))
        {
            _loginLimiter.RecordFailure(ip);
            return Results.Unauthorized();
        }

        var result = _accounts.TryCreateAccount(request.Username, request.Password, out var account);

        // Handle failure resutls:
        switch (result)
        {
            case AccountStore.CreationResult.InvalidUsername:
                return Results.BadRequest("Invalid Username");
            
            case AccountStore.CreationResult.InvalidPassword:
                return Results.BadRequest("Invalid Password");
            
            case AccountStore.CreationResult.UsernameTaken:
                return Results.Conflict("This username has been taken.");

            default:
                break;
        }
        // Else we create an account:
        _loginLimiter.RecordSuccess(ip);
        var token = _sessionStore.CreateSession(account!.PlayerId);

        Console.WriteLine($"Player {account.Username} has created an account.");

        return Results.Json(
            new AccountAuthenticationResponse(
                token,
                account.PlayerId,
                account.Username,
                null
            )
        );
    }

    public IResult Login(HttpContext ctx, AccountLoginRequest request)
    {
        var ip = ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        
        // Deny locked ips
        if (_loginLimiter.IsLocked(ip))
        {
            return Results.StatusCode(StatusCodes.Status429TooManyRequests);  
        }

        Account? account =  _accounts.FindByUsername(request.Username);
        if (account is null)
        {
            _loginLimiter.RecordFailure(ip);
            return Results.NotFound($"No record of a player with name {request.Username} exists.");
        }
        
        if (!PasswordHasher.Verify(request.Password, account.PasswordHash, account.PasswordSalt))
        {
            _loginLimiter.RecordFailure(ip);
            return Results.Unauthorized();
        }

        _loginLimiter.RecordSuccess(ip);

        var token = _sessionStore.CreateSession(account.PlayerId);
        string? existingNationId = EntityLibrary.GetAllEntitiesOfType<Nation>().FirstOrDefault(n => n.playerId == account.PlayerId)?.playerId;
        
        Console.WriteLine($"Player {account.Username} has logged in.");

        return Results.Json(
            new AccountAuthenticationResponse(
                token,
                account.PlayerId,
                account.Username,
                existingNationId
            )
        );
    }
}