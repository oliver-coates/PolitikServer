namespace PolitikServer.Core;


public record ServerInfoResponse(string Game, int ApiVersion, string ContentVersion, string ServerName);

public static class ServerInfo
{
    public static IResult Get(EntityLibrary contentLib, GameDefinitionLibrary defLib)
    {
        return Results.Json(new ServerInfoResponse(
            Game: "TinyPolitik",
            ApiVersion: 1,
            ContentVersion: defLib.VersionHash,
            ServerName: "Unnamed Server"
        ));
    }
}