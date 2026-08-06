using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace PolitikServer.Core;


public class EntityLibrary
{

    private static Dictionary<Type, Dictionary<string, GameEntity>> ContentLib = []; 
    
    public string VersionHash { get; private set; }

    public EntityLibrary()
    {        
        Reload();

        VersionHash = GetVersionHash();
    }

    private string GetVersionHash()
    {
        return this.GetHashCode().ToString();
    }

    private void Reload()
    {
        // StrategicResources = LoadContentOfType<StrategicResourceDefinition>("strategic resources");
    }

    #region Public Methods
        
    public static void AddEntity(GameEntity entity)
    {
        if (!ContentLib.ContainsKey(entity.GetType()))
        {
            ContentLib.Add(entity.GetType(), new());
        }

        if (ContentLib[entity.GetType()].ContainsKey(entity.UniqueIdentifier))
        {
            throw new Exception($"Attempting to add entity '' of type '', but an entity of this type and UID already exists.");
        }

        ContentLib[entity.GetType()].Add(entity.UniqueIdentifier, entity);
        Console.WriteLine($"Added game entity:   {entity}");
    }

    public static void RemoveEntity<T>(string uid) where T : GameEntity
    {
        if (!ContentLib[typeof(T)].Remove(uid))
        {
            throw new Exception($"Attempting to remove entity '{uid}', of type '{typeof(T)}', but it does not exist within the Content Library.");
        }
    }

    /// <summary>
    /// Gets a game definition of the passed type, at the provided unique identifier.
    /// </summary>
    public static T GetEntity<T>(string uid) where T : GameEntity
    {
        return (T) ContentLib[typeof(T)][uid];
    }

    public static T[] GetAllEntitiesOfType<T>() where T : GameEntity
    {
        return (T[]) ContentLib[typeof(T)].Values.ToArray();
    }

    public static List<T> GetAllEntitiesByUids<T>(IEnumerable<string> uids) where T : GameEntity
    {
        Dictionary<string, GameEntity> dict = ContentLib[typeof(T)];
        
        List<T> result = new();
        foreach (string uid in uids)
        {
            result.Add((T) dict[uid]);
        }

        return result;
    }

    /// <summary>
    /// Takes all the game entities and adds them to a json array.
    /// </summary>
    public string GetAllEntitiesAsJson()
    {
        // This solution will work, but the alterative solution below might work - and is WAY easier for us. Give it a go:
        #if false
        var root = new JsonObject();

        foreach (KeyValuePair<Type, Dictionary<string, GameEntity>> gameEntityBundle in ContentLib)
        {
            Type type = gameEntityBundle.Key;
            GameEntity[] content = gameEntityBundle.Value.Values.ToArray();

            string allJson = Newtonsoft.Json.JsonConvert.SerializeObject(content, Newtonsoft.Json.Formatting.None);
            root.Add(type.Name, allJson);
        }

        return root.ToString();
        #endif
        
        // Test this!:
        return Newtonsoft.Json.JsonConvert.SerializeObject(ContentLib, Newtonsoft.Json.Formatting.None);
    }
    #endregion
}