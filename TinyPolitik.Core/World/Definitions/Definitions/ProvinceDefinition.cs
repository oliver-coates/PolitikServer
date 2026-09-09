namespace PolitikServer.Core;

public class ProvinceDefinition : GameDefinition
{
    public string Name { get; private set; }
    public WorldPoint Centre { get; private set; }
    public SerializedList<ProvinceDefinition> ConnectedProvinces { get; private set; }
    
    public ProvinceDefinition(string uid, string name, WorldPoint centre, SerializedList<ProvinceDefinition> ConnectedProvinces) : base(uid)
    {
        Name = name;

        Centre = centre;
        this.ConnectedProvinces = ConnectedProvinces;
    }

    public override string ToString()
    {
        return $"[{UniqueIdentifier}] Province '{Name}', center {Centre}, Connected to: {String.Join(',', ConnectedProvinces.Get().Select(p => p.UniqueIdentifier))}";
    }

    internal override void Deserialize()
    {
        ConnectedProvinces.Load();
    }
}


/// <summary>
/// Point in the world (X, Y).
/// Do note that this struct is serializable, and so is used across SerializedDefinitions and normal definitions.
/// </summary>
public struct WorldPoint
{
    public float x = 0.0f;
    public float y = 0.0f;

    public WorldPoint(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return $"World Point ({x:0.000},{y:0.000})";
    }
}