using System.Collections;
using PolitikServer.Core;
using Newtonsoft.Json;

namespace PolitikServer;

public interface ISerializableObject
{
    public string UniqueIdentifier { get; }

    public static string GetTypeName<T>()
    {
        if (typeof(T).IsSubclassOf(typeof(GameDefinition)))
        {
            return "GameDefinition";
        }
        else if (typeof(T).IsSubclassOf(typeof(GameEntity)))
        {
            return "GameEntity";
        }
        else
        {
            return "Unknown";
        }
    }

}

public class SerializedField<T> where T : ISerializableObject
{
    [JsonIgnore] public T Value { get; private set; }
    [JsonProperty] private string SerializedValue;
    [JsonProperty] private string TypeName;
    [JsonProperty] private string Type;

    public SerializedField(T value)
    {
        Value = value;
        SerializedValue = value.UniqueIdentifier;
        TypeName = typeof(T).Name;
        Type = ISerializableObject.GetTypeName<T>();
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
}

public class SerializedNullableField<T> where T : ISerializableObject?
{
    [JsonIgnore] public T? Value { get; private set; }
    [JsonProperty] private string? SerializedValue;
    [JsonProperty] private string TypeName;
    [JsonProperty] private string Type;

    public SerializedNullableField(T? value)
    {
        Value = value;
        SerializedValue = value?.UniqueIdentifier ?? "";
        TypeName = typeof(T).Name;
        Type = ISerializableObject.GetTypeName<T>();
    }

    public SerializedNullableField()
    {
        Value = default(T);
        SerializedValue = "";
        TypeName = typeof(T).Name;
        Type = ISerializableObject.GetTypeName<T>();
    }

    public T? Get()
    {
        return Value;
    }

    public void Set(T? newValue)
    {
        Value = newValue;
        SerializedValue = newValue?.UniqueIdentifier ?? "";
    }
}

public class SerializedList<T>  where T : ISerializableObject 
{
    [JsonIgnore] public List<T> Values {get; private set; }
    [JsonProperty] private List<string> SerializedValues;
    [JsonProperty] private string TypeName;
    [JsonProperty] private string Type;

    public SerializedList(IList<T> values)
    {
        Values = new List<T>(values);
        SerializedValues = GetSerialized();
        TypeName = typeof(T).Name;
        Type = ISerializableObject.GetTypeName<T>();
    }

    public SerializedList()
    {
        Values = new List<T>();
        SerializedValues = [];
        TypeName = typeof(T).Name;
        Type = ISerializableObject.GetTypeName<T>();
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
            SerializedValues = GetSerialized();
        }
    }

    private List<string> GetSerialized()
    {
        return Values.Select(e => e.UniqueIdentifier).ToList();
    }

    public IEnumerator<T> GetEnumerator()
    {
        return Values.GetEnumerator();
    }
}