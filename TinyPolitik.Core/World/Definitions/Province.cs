namespace PolitikServer.Core;

public class Province : GameDefinition, IRequiresLateDeserialization
{
    public string Name { get; private set; }
    public ProvinceBorder[] Borders { get; private set; }
    public WorldPoint Centre { get; private set; }

    private readonly string[] ConnectedUids;
    public Province[] ConnectedProvinces { get; private set; }
    
    public Province(string uid, string name, ProvinceBorder[] borders, WorldPoint centre, string[] connectedProvinces)
    {
        UniqueIdentifier = uid;
        Name = name;

        Borders = borders;
        Centre = centre;
    
        ConnectedUids = connectedProvinces;
        ConnectedProvinces = []; // Will be gathered in LateDeserialize
    }

    public void LateDeserialize()
    {
        // Conver the strings into UIDs
        List<Province> adjacent = [];
        foreach (string adjacentProvinceUid in ConnectedUids)
        {
            adjacent.Add(WorldDataLibrary.GetDefinition<Province>(adjacentProvinceUid));
        }

        ConnectedProvinces = [.. adjacent];
    }
}

public class ProvinceBorder
{
    public WorldPoint[] points = [];
}

public struct WorldPoint
{
    public float x = 0.0f;
    public float y = 0.0f;

    public WorldPoint(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
}