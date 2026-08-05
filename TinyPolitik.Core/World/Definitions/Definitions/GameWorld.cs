namespace PolitikServer.Core;

public class GameWorld : GameDefinition
{

    public string Name { get; private set; }
    public string Author {get; private set; }
    public DateTime TimeLastChanged { get; private set; }

    public GameWorld(string name, string author, DateTime timeChanged) : base("world")
    {
        Name = name;
        Author = author;
        TimeLastChanged = timeChanged;
    }

    public override string ToString()
    {
        return $"[{UniqueIdentifier}] '{Name}' by '{Author}'. Last Updated: {TimeLastChanged.ToString()}";
    }
}