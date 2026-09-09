using System.Collections;
using PolitikServer.Core;
using Newtonsoft.Json;

namespace PolitikServer.Core;



public class SerializedField<T> where T : GameRecord
{
    [JsonIgnore] public T Value { get; private set; }
    [JsonProperty] private string SerializedValue;

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
}