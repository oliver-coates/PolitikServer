using System.Collections;
using PolitikServer.Core;
using Newtonsoft.Json;

namespace PolitikServer.Core;



public class SerializedField<T> where T : GameRecord
{
    [JsonIgnore] public T Value { get; private set; }
    [JsonRequired] private string SerializedValue;

    public SerializedField(T value)
    {
        Value = value;
        SerializedValue = value.UniqueIdentifier;
    }

    public T Get()
    {
        return Value;
    }

    public void Set(T newValue)
    {
        Value = newValue;
        SerializedValue = newValue.UniqueIdentifier;
    }

    public void Load()
    {
        if (string.IsNullOrEmpty(SerializedValue))
        {
            throw new Exception("No serialized value found for SerializedField. Consider using the NullableSerializedField class");
        }
        else if (typeof(T).IsSubclassOf(typeof(GameDefinition)))
        {
            GameRecord definition = DefinitionLibrary.Get(SerializedValue);
            Value = (T) definition;
        }
        else if (typeof(T).IsSubclassOf(typeof(GameEntity)))
        {
            GameRecord entity = EntityLibrary.Get(SerializedValue); 
            Value = (T) entity;
        }
        else
        {
            throw new NullReferenceException($"Could not deserialize field of type {typeof(T)}.");
        }
    }
}

public class SerializedNullableField<T> where T : GameRecord?
{
    [JsonIgnore] public T? Value { get; private set; }
    [JsonProperty] private string? SerializedValue;

    public SerializedNullableField(T? value)
    {
        Value = value;
        SerializedValue = value?.UniqueIdentifier ?? "null";
    }

    public SerializedNullableField()
    {
        Value = default(T);
        SerializedValue = "null";
    }

    public T? Get()
    {
        return Value;
    }

    public void Set(T? newValue)
    {
        Value = newValue;
        SerializedValue = newValue?.UniqueIdentifier ?? "null";
    }

    public void Load()
    {
        if (string.IsNullOrEmpty(SerializedValue) || SerializedValue == "null")
        {
            Value = null;
        }
        else if (typeof(T).IsSubclassOf(typeof(GameDefinition)))
        {
            GameRecord definition = DefinitionLibrary.Get(SerializedValue);
            Value = (T) definition;
        }
        else if (typeof(T).IsSubclassOf(typeof(GameEntity)))
        {
            GameRecord entity = EntityLibrary.Get(SerializedValue); 
            Value = (T) entity;
        }
        else
        {
            throw new NullReferenceException($"Could not deserialize field of type {typeof(T)}.");
        }
    }
}

public class SerializedList<T>  where T : GameRecord 
{
    [JsonIgnore] public List<T> Values {get; private set; }
    [JsonProperty] private List<string> SerializedValues;

    public SerializedList(IList<T> values)
    {
        Values = new List<T>(values);
        SerializedValues = Values.Select(v => v.UniqueIdentifier).ToList();
    }

    public SerializedList()
    {
        Values = new List<T>();
        SerializedValues = [];
    }

    public List<T> Get()
    {
        return Values;
    }
   
    public T this[int i]
    {
        get 
        {
            return Values[i];
        }
        set 
        {
            Values[i] = value;
            SerializedValues = Values.Select(v => v.UniqueIdentifier).ToList();
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        return Values.GetEnumerator();
    }

    public void Load()
    {
        if (SerializedValues == null || SerializedValues.Count == 0)
        {
            Values = new List<T>();
        }
        else if (typeof(T).IsSubclassOf(typeof(GameDefinition)))
        {
            GameRecord[] definitions = DefinitionLibrary.Get(SerializedValues);
            Values = definitions.Select(e => (T) e).ToList();
        }
        else if (typeof(T).IsSubclassOf(typeof(GameEntity)))
        {
            GameRecord[] entities = EntityLibrary.Get(SerializedValues); 
            Values = entities.Select(e => (T) e).ToList();
        }
        else
        {
            throw new NullReferenceException($"Could not deserialize list of type '{typeof(T)}'.");
        }
    }
}