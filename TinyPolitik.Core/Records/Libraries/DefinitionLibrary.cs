using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace PolitikServer.Core;

public class DefinitionLibrary : GameLibrary<GameDefinition>, IGameRecordLibrary
{
    public string VersionHash { get; private set; } = "";
    
    public string ComputeHash()
    {
        var sb = new StringBuilder();
        Dictionary<string, string[]> allAsjson = ToJson();

        // Concatenate all world data:
        foreach (KeyValuePair<string, string[]> contentDict in allAsjson)
        {
            foreach (string json in contentDict.Value)
            {
                sb.Append(json);
            }
        }

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(sb.ToString()));

        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    // public static new GameDefinition Get(string uniqueIdentifier)
    // {
    //     return Instance.Get(uniqueIdentifier);
    // }

    // public static new T Get<T>(string uniqueIdentifier) where T : GameDefinition
    // {
    //     return Instance.Get<T>(uniqueIdentifier);
    // }

    // public static T Get<T>() where T : GameDefinition
    // {
    //     return Instance.GetAll<T>()[0];
    // }

    
    // public static new T[] Get<T>(IReadOnlyList<string> uids) where T : GameDefinition
    // {
    //     return Instance.Get<T>(uids);
    // }

    // public static new GameDefinition[] Get(IReadOnlyList<string> uids)
    // {
    //     return Instance.Get(uids);
    // }


    // public static new T[] GetAll<T>() where T : GameDefinition
    // {
    //     return Instance.GetAll<T>();
    // }          

    // public static new GameDefinition[] GetAll(Type requestedType)
    // {
    //     return Instance.GetAll(requestedType);
    // }

    // public static void Add<T>(T record) where T : GameDefinition
    // {
    //     Instance.Add<T>(record);
    // }

    // public static new void Add(GameDefinition record, Type t)
    // {
    //     Instance.Add(record, t);
    // }

    // public static void Remove<T>(T record) where T : GameDefinition
    // {
    //     Instance.Remove<T>(record);
    // }

    // public static new void Remove(GameDefinition record, Type t)
    // {
    //     Instance.Remove(record, t);
    // }
}
