using System.Collections;
using System.Text.Json.Serialization;
using PolitikServer.Core;

namespace PolitikServer;

public interface ISerializableObject
{
    public string UniqueIdentifier { get; }

    public static string GetTypeName<T>()
    {
        if (typeof(T).IsAssignableTo(typeof(GameDefinition)))
        {
            return "GameDefintion";
        }
        else if (typeof(T).IsAssignableTo(typeof(GameEntity)))
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
    public T Value { get; private set; }
    private string SerializedValue;
    private string TypeName;
    private string Type;

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
    public T? Value { get; private set; }
    private string? SerializedValue;
    private string TypeName;
    private string Type;

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

public class SerializedList<T> : IEnumerable<T> where T : ISerializableObject 
{
    public List<T> Values {get; private set; }
    private List<string> SerializedValues;
    private string TypeName;
    private string Type;

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

    #region Implementation of IEnumerable
    public IEnumerator<T> GetEnumerator()
    {
        return Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    #endregion
}