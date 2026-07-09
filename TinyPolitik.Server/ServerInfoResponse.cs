namespace TinyPolitik.Core;


public record ServerInfoResponse(string Game, int ApiVersion, string ContentVersion, string ServerName);

public static class ServerInfo
{
    public static IResult Get(ContentLibrary lib)
    {
        return Results.Json(new ServerInfoResponse(
            Game: "TinyPolitik",
            ApiVersion: 1,
            ContentVersion: lib.VersionHash,
            ServerName: "Unnamed Server"
        ));
    }
}