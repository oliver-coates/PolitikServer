namespace PolitikServer.Core;

public static class RandomCountryNameGenerator
{
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

    private static Dictionary<string,string> NameEnd = new()
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
        string start = NameStart[RandomUtil.Range(0, NameStart.Count)];
        
        List<KeyValuePair<string, string>> ends = NameEnd.ToList();
        string end = ends[RandomUtil.Range(0, ends.Count)].Key;
        
        string verbEnd = NameEnd[end];

        string verb = $"{start}{verbEnd}";
        string shortName = $"{start}{end}";
        string longName;

        if (RandomUtil.Range(0, 1) == 1)
        {
            string prefix = NationPrefixes[RandomUtil.Range(0, NationPrefixes.Count)];
            longName = $"{prefix} {shortName}";
        }
        else
        {
            string postfix = NationPostfixes[RandomUtil.Range(0, NationPostfixes.Count)];
            longName = $"{shortName} {postfix}";
        }

        return new RandomlyGeneratedName()
        {
            shortName = shortName,
            longName = longName,
            noun = verb
        };
    }

}

public struct RandomlyGeneratedName()
{
    public required string longName;
    public required string shortName;
    public required string noun;

    public override string ToString()
    {
        return $"Long: '{longName}', Shortened: '{shortName}', Verb: '{noun}'";
    }
}