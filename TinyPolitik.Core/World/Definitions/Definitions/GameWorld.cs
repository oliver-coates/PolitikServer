namespace PolitikServer.Core;

public class GameWorld : GameDefinition
{

    public string WorldName { get; private set; }
    public string WorldAuthor {get; private set; }
    public DateTime LastUpdated { get; private set; }

    public GameWorld(string WorldName, string WorldAuthor, DateTime LastUpated) : base("world")
    {
        this.WorldName = WorldName;
        this.WorldAuthor = WorldAuthor;
        this.LastUpdated = LastUpated;
    }

    internal override void Deserialize() {}


    protected override string GetFullName()
    {
        return $"World '{WorldName}' by '{WorldAuthor}'. Last Updated: {LastUpdated}";
    }

    public override string GetReadableName()
    {
        return "Game World";
    }
}