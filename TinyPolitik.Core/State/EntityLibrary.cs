using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using Newtonsoft.Json;

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

    public T? TryGetEntity<T>(string uid) where T : GameEntity
    {
        if (ContentLib.TryGetValue(typeof(T), out var subLib))
        {
            if (subLib.TryGetValue(uid, out GameEntity? value))
            {
                return (T)value;
            }             
        }

        return null;
    }

    public static T[] GetAllEntitiesOfType<T>() where T : GameEntity
    {
        return ContentLib[typeof(T)].Values.Select(e => (T) e).ToArray();
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
        var root = new JsonObject();

        foreach (KeyValuePair<Type, Dictionary<string, GameEntity>> entityBundle in ContentLib)
        {
            string typeName = entityBundle.Key.Name;
            List<GameEntity> entities = entityBundle.Value.Values.ToList();
            List<string> entitiesJson = entities.Select(JsonConvert.SerializeObject).ToList();

            JsonArray jsonArray = new(entitiesJson.Select(e => JsonNode.Parse(e)).ToArray());
            root.Add(typeName, jsonArray);
        }

        return root.ToString();
    }
    #endregion

}