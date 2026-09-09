namespace PolitikServer.Core;

using System;
using System.Collections.Generic;
using System.Linq;

public abstract class GameLibrary<LibType> : IGameRecordLibrary where LibType : GameRecord
{
    public static GameLibrary<LibType>? Instance { get; private set; }

    private Dictionary<string, LibType> _dict;
    private Dictionary<Type, List<LibType>> _typeDict;

    protected GameLibrary()
    {
        _dict = new();
        _typeDict = new();
        Instance = this;

        InitialiseInternal();
    }

    protected virtual void InitialiseInternal() {}

    /// <summary>
    /// Deserializes all records within this library.
    /// </summary>
    public void Deserialize()
    {
        foreach (LibType record in _dict.Values)
        {
            record.Deserialize();
        }
    }

    void IGameRecordLibrary.Populate(GameRecord[] records, Type t)
    {
        if (!_typeDict.ContainsKey(t)) { _typeDict.Add(t, new()); }
        
        foreach (GameRecord record in records)
        {
            LibType obj = (LibType) record;
            
            _dict.Add(record.UniqueIdentifier, obj);
            _typeDict[t].Add(obj);
        }
    }


    public virtual T Get<T>(string uid) where T : LibType
    {
        try
        {
            return (T) _dict[uid];
        }
        catch (KeyNotFoundException e)
        {
            throw new Exception($"Uid '{uid}' of type '{typeof(T)}' is not within the '{typeof(LibType)}' Library : {e}");
        }
    }
    public virtual LibType Get(string uid)
    {
        return _dict[uid];
    }


    public virtual T[] Get<T>(IReadOnlyList<string> uids) where T : LibType
    {
        return uids.Select(uid =>
        {
            try
            {
                return (T) _dict[uid];                
            }
            catch (Exception e)
            {
                throw new Exception($"Uid '{uid}' of type '{typeof(T)}' is not within the '{typeof(LibType)}' Library. {e}");
            }
        }).ToArray();
    }

    public virtual LibType[] Get(IReadOnlyList<string> uids)
    {
        return uids.Select(uid =>
        {
            try
            {
                return _dict[uid];                
            }
            catch
            {
                throw new Exception($"Uid '{uid}' is not within the '{typeof(LibType)}' Library.");
            }
        }).ToArray();
    }


    public virtual T[] GetAll<T>() where T : LibType
    {
        Type requestedType = typeof(T);
        return _dict.Values.Where(e => e.GetType() == requestedType).Select(e => (T) e).ToArray();
    }          

    public virtual LibType[] GetAll(Type requestedType)
    {
        return _dict.Values.Where(e => e.GetType() == requestedType).ToArray();
    }          

    public virtual Dictionary<Type, List<LibType>> GetAllByType()
    {
        return _typeDict;
    }
    
    public virtual void Add<T>(LibType item) where T : LibType
    {
        _dict.Add(item.UniqueIdentifier, item);
        
        if (!_typeDict.ContainsKey(typeof(T)))
        {
            _typeDict.Add(typeof(T), new List<LibType>());
        }
        _typeDict[typeof(T)].Add(item);
    } 

    public virtual void Add(LibType item, Type t)
    {
        _dict.Add(item.UniqueIdentifier, item);
        
        if (!_typeDict.ContainsKey(t))
        {
            _typeDict.Add(t, new List<LibType>());
        }
        _typeDict[t].Add(item);
    }

    public virtual void AddRange<T>(IReadOnlyList<LibType> items) where T : LibType
    {
        foreach (LibType i in items)
        {
            _dict.Add(i.UniqueIdentifier, i);
        }

        if (!_typeDict.ContainsKey(typeof(T)))
        {
            _typeDict.Add(typeof(T), new List<LibType>());
        }
        _typeDict[typeof(T)].AddRange(items);
    }

    public virtual void AddRange(IReadOnlyList<LibType> items, Type t)
    {
        foreach (LibType i in items)
        {
            _dict.Add(i.UniqueIdentifier, i);
        }

        if (!_typeDict.ContainsKey(t))
        {
            _typeDict.Add(t, new List<LibType>());
        }
        _typeDict[t].AddRange(items);
    }


    public virtual void Remove<T>(LibType item) where T : LibType
    {
        _dict.Remove(item.UniqueIdentifier);
        _typeDict[typeof(T)].Remove(item);
    } 

    public virtual void Remove(LibType item, Type t)
    {
        _dict.Remove(item.UniqueIdentifier);
        _typeDict[t].Remove(item);
    }
    
    /// <summary>
    /// Converts all records within this dictionary into JSON,
    /// Returns a Dictionary mapping the name of the type, to an array of values. 
    /// </summary>
    public virtual Dictionary<string, string[]> ToJson()
    {
        Dictionary<string, string[]> jsonDict = new();

        foreach (Type recordType in _typeDict.Keys.OrderBy(k => k.Name))
        {
            string typeName = recordType.Name;
            List<string> jsonList = new();

            foreach (LibType record in _typeDict[recordType].OrderBy(k => k.UniqueIdentifier))
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(record);
                jsonList.Add(json);
            }

            jsonDict.Add(typeName, jsonList.ToArray());
        }

        return jsonDict;   
    }
}

internal interface IGameRecordLibrary
{
    internal void Populate(GameRecord[] records, Type t);
}

