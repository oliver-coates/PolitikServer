using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace PolitikServer.Core;

public class DefinitionLibrary : GameLibrary<GameDefinition>
{
    public string VersionHash { get => ComputeHash(); }
    
    public string ComputeHash()
    {
        string asJson = ToJson();

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(asJson));

        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    // These methods are used by SerializedFields when loading in,
    // They should not be called by regular classes, who should be relying on DI
    public static new GameDefinition[] Get(IReadOnlyList<string> uids)
    {
        return Instance?.Get(uids) ?? throw new Exception("Defintion library is null.");
    }

    public static new GameDefinition Get(string uid)
    {
        return Instance?.Get(uid) ?? throw new Exception("Defintion library is null.");
    } 
}
