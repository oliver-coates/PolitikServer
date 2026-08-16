namespace PolitikServer.Core;




public class RequireSessionFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;
        var sessions = httpContext.RequestServices.GetRequiredService<SessionStore>();

        var authHeader = httpContext.Request.Headers.Authorization.ToString();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return Results.Unauthorized();
        }

        var token = authHeader["Bearer ".Length..].Trim();
        var session = sessions.Validate(token);
        if (session is null)
        {
            return Results.Unauthorized();
        }

        httpContext.Items["Session"] = session;
        return await next(context);
    }
}

public static class HttpContextSessionExtensions
{
    public static SessionInfo GetSession(this HttpContext ctx)
    {
        return ctx.Items["Session"] as SessionInfo ?? throw new InvalidOperationException("No session on context - endpoint is missing RequireSessionFilter");
    }
}