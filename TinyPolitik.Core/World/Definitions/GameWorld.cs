namespace PolitikServer.Core;

public class GameWorld : GameDefinition
{
    public string Name { get; private set; }
    public string Author {get; private set; }
    public DateTime TimeLastChanged { get; private set; }


    public GameWorld(string name, string author, long lastTimeChanged) : base("world")
    {
        Name = name;
        Author = author;
        TimeLastChanged = DateTime.FromBinary(lastTimeChanged);
    }
}