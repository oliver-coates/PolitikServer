namespace PolitikServer.Core;

public class ProvinceDefinition : GameDefinition
{
    public string Name { get; private set; }
    public GlobeArea Area { get; private set; }
    public SerializedList<ProvinceDefinition> ConnectedProvinces { get; private set; }
    public SerializedField<TerrainType> Terrain { get; private set; }
    public SerializedList<ProvinceFeature> Features { get; private set; }

    

    [Newtonsoft.Json.JsonConstructor]
    public ProvinceDefinition(string UniqueIdentifier, string Name, GlobeArea Area, SerializedList<ProvinceDefinition> ConnectedProvinces, SerializedField<TerrainType> Terrain, SerializedList<ProvinceFeature> Features) : base(UniqueIdentifier)
    {
        this.Name = Name;
        this.Area = Area;

        this.ConnectedProvinces = ConnectedProvinces;
        this.Terrain = Terrain;
        this.Features = Features;
    }


    internal override void Deserialize()
    {
        ConnectedProvinces.Load();
    }

    protected override string GetFullName()
    {
        return $"Province '{Name}', center {Area.centre}, Connected to: {String.Join(',', ConnectedProvinces.Get().Select(p => p.UniqueIdentifier))}";
    }

    public override string GetReadableName()
    {
        return "Province.";
    }
}

public class GlobeArea
{
    public required AreaBorder[] borders;
    public required WorldPoint centre;
}

public class AreaBorder
{
    public required WorldPoint[] points;
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

