namespace PolitikServer.Core;

using System;
using System.Collections.Generic;
using System.Linq;

public abstract class GameLibrary<LibType> where LibType : GameRecord
{
    public static GameLibrary<LibType>? Instance { get; private set; }

    /// <summary>
    /// Dictionary relating a UID to a gamerecord that this library is storing
    /// </summary>
    private Dictionary<string, LibType> _idDict;
    /// <summary>
    /// Dictionary relating a type to a list of all gamerecord of this type that this library is storing
    /// </summary>
    private Dictionary<Type, List<LibType>> _typeDict;

    protected GameLibrary()
    {
        _idDict = new();
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
        foreach (LibType record in _idDict.Values)
        {
            record.Deserialize();
        }
    }

    public void Populate(IReadOnlyList<GameRecord> records, Type t)
    {
        if (!_typeDict.ContainsKey(t)) { _typeDict.Add(t, new()); }
        
        foreach (GameRecord record in records)
        {
            LibType obj = (LibType) record;
            
            _idDict.Add(record.UniqueIdentifier, obj);
            _typeDict[t].Add(obj);
        }
    }


    public virtual T Get<T>(string uid) where T : LibType
    {
        try
        {
            return (T) _idDict[uid];
        }
        catch (KeyNotFoundException e)
        {
            throw new Exception($"Uid '{uid}' of type '{typeof(T)}' is not within the '{typeof(LibType)}' Library : {e}");
        }
    }
    public virtual LibType Get(string uid)
    {
        return _idDict[uid];
    }


    public virtual T[] Get<T>(IReadOnlyList<string> uids) where T : LibType
    {
        return uids.Select(uid =>
        {
            try
            {
                return (T) _idDict[uid];                
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
                return _idDict[uid];                
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
        return _idDict.Values.Where(e => e.GetType() == requestedType).Select(e => (T) e).ToArray();
    }          

    public virtual LibType[] GetAll(Type requestedType)
    {
        return _idDict.Values.Where(e => e.GetType() == requestedType).ToArray();
    }          

    public virtual Dictionary<Type, List<LibType>> GetAllByType()
    {
        return _typeDict;
    }
    
    public virtual void Add<T>(LibType item) where T : LibType
    {
        _idDict.Add(item.UniqueIdentifier, item);
        
        if (!_typeDict.ContainsKey(typeof(T)))
        {
            _typeDict.Add(typeof(T), new List<LibType>());
        }
        _typeDict[typeof(T)].Add(item);
    } 

    public virtual void Add(LibType item, Type t)
    {
        _idDict.Add(item.UniqueIdentifier, item);
        
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
            _idDict.Add(i.UniqueIdentifier, i);
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
            _idDict.Add(i.UniqueIdentifier, i);
        }

        if (!_typeDict.ContainsKey(t))
        {
            _typeDict.Add(t, new List<LibType>());
        }
        _typeDict[t].AddRange(items);
    }


    public virtual void Remove<T>(LibType item) where T : LibType
    {
        _idDict.Remove(item.UniqueIdentifier);
        _typeDict[typeof(T)].Remove(item);
    } 

    public virtual void Remove(LibType item, Type t)
    {
        _idDict.Remove(item.UniqueIdentifier);
        _typeDict[t].Remove(item);
    }
    
    public virtual string ToJson()
    {
        // Libtype is GameDefinition here
        Dictionary<string, LibType[]> dict = new();

        foreach ((Type t, List<LibType> objects) in _typeDict)
        {
            dict.Add(t.Name, objects.ToArray());
        }

        return Newtonsoft.Json.JsonConvert.SerializeObject(dict); 
    }

    public virtual T? TryGet<T>(string uid) where T : LibType
    {
        if (_idDict.ContainsKey(uid))
        {
            return (T) _idDict[uid];
        }
        else
        {
            return null;
        }
    }
}


