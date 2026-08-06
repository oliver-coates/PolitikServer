namespace PolitikServer.Core;

public static class RandomCountryNameGenerator
{
    private static readonly Random Random;
    static RandomCountryNameGenerator()
    {
        Random = new Random();
    }


    private static List<string> NationPrefixes = new()
    {
        "Kingdom of",
        "State of",
        "Greater",
        "United States of",
        "Republic of",
        "Sultanate of",
        "Liberated",
        "Free State of",
        "Commune of",
    };
    
    private static List<string> NationPostfixes = new()
    {
        "Union",
        "Commune",
        "Republic",
        "State",
        "Confederacy",
        "Federation"
    };

    private static List<string> NameStart = new()
    {
        "Am",
        "Mor",
        "Bel",
        "Bav",
        "Sult",
        "New Z",
        "Germ",
        "Port",
        "Tur",
        "Russ",
        "Burg"
    };

    public static Dictionary<string,string> NameEnd = new()
    {
        {"avia", "avian"},
        {"erica", "erican"},
        {"istan", "istani"},
        {"burg", "burgian"},
        {"any", "anian"},
        {"ugal", "ugese"},
        {"key", "kish"},
        {"ia", "ian"},
        {"ania", "anian"},
        {"undy", "undian"},
        {"land", "landian"},
        {"uguay", "uguayan"}
    };

    public static RandomlyGeneratedName Generate()
    {
        string start = NameStart[Random.Next(0, NameStart.Count)];
        
        List<KeyValuePair<string, string>> ends = NameEnd.ToList();
        string end = ends[Random.Next(0, ends.Count)].Key;
        
        string verbEnd = NameEnd[end];

        string verb = $"{start}{verbEnd}";
        string shortName = $"{start}{end}";
        string longName;

        if (Random.Next(0, 2) == 1)
        {
            string prefix = NationPrefixes[Random.Next(0, NationPrefixes.Count)];
            longName = $"{prefix} {shortName}";
        }
        else
        {
            string postfix = NationPostfixes[Random.Next(0, NationPostfixes.Count)];
            longName = $"{shortName} {postfix}";
        }

        return new RandomlyGeneratedName()
        {
            shortName = shortName,
            longName = longName,
            verb = verb
        };
    }

}

public struct RandomlyGeneratedName()
{
    public required string longName;
    public required string shortName;
    public required string verb;

    public override string ToString()
    {
        return $"Long: '{longName}', Shortened: '{shortName}', Verb: '{verb}'";
    }
}