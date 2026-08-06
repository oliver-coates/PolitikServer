namespace PolitikServer.Core;

public class Province : GameDefinition
{
    public string Name { get; private set; }
    public WorldPoint Centre { get; private set; }

    private readonly string[] ConnectedUids;
    public Province[] ConnectedProvinces { get; private set; }
    
    public Province(string uid, string name, WorldPoint centre, string[] connectedProvinces) : base(uid)
    {
        Name = name;

        Centre = centre;
    
        ConnectedUids = connectedProvinces;
        ConnectedProvinces = []; // Will be gathered in LateDeserialize
    }

    public override void LateDeserialize()
    {
        // Conver the strings into UIDs
        List<Province> adjacent = [];
        foreach (string adjacentProvinceUid in ConnectedUids)
        {
            adjacent.Add(DefinitionLibrary.GetDefinition<Province>(adjacentProvinceUid));
        }

        ConnectedProvinces = [.. adjacent];
    }

    public override string ToString()
    {
        return $"[{UniqueIdentifier}] Province '{Name}', center {Centre}, Connected to: {String.Join(',', ConnectedProvinces.Select(p => p.UniqueIdentifier))}";
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