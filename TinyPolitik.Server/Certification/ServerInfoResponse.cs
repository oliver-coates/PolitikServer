namespace PolitikServer.Core;


public record ServerInfoResponse(string Game, int ApiVersion, string WorldVersion, string ServerName, bool RealTimePlayEnabled);

public static class ServerInfo
{
    public static IResult Get(DefinitionLibrary defLib, GameConfig config)
    {
        return Results.Json(new ServerInfoResponse(
            Game: "TinyPolitik",
            ApiVersion: 1,
            WorldVersion: defLib.VersionHash,
            ServerName: "Unnamed Server",
            config.AllowRealTimePlay
        ));
    }
}