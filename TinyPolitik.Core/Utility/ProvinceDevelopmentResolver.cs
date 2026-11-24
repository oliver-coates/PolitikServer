namespace PolitikServer.Core;

public class ProvinceDevelopmentResolver
{
    private readonly DefinitionLibrary _definitions;

    private Dictionary<Tuple<int, int>, ProvinceDevelopmentLevel> _rangeDict;

    public ProvinceDevelopmentResolver(DefinitionLibrary definitions)
    {
        _definitions = definitions;
        _rangeDict = new();

        WorldInitialiser.OnInitialiseEarly += Initialise;
    }

    public void Initialise(ServerInitialisationMode mode)
    {
        _rangeDict = new();

        ProvinceDevelopmentLevel[] levels = _definitions.GetAll<ProvinceDevelopmentLevel>();

        // Order development levels by the ones with the lowest population thresholds first.
        ProvinceDevelopmentLevel[] orderedLevels = levels.OrderByDescending(s => s.PopulationThreshold).ToArray();

        // Assign tuples to the dictionary defining the population ranges of each development level.
        // I.e the first goes from pop 0 -> 9,999, and then next from 10,000 -> 99,999 etc
        for (int levelIndex = 0; levelIndex < orderedLevels.Length; levelIndex++)
        {
            var currentLevel = orderedLevels[levelIndex];
            int min = currentLevel.PopulationThreshold;

            int max;
            if (levelIndex == (orderedLevels.Length -1))
            {
                // The final level doesn't have a next development level, so just add it as the max possible value:
                max = int.MaxValue;        
            }
            else
            {
                // Maximum threshold here is the next level minus 1
                var nextLevel = orderedLevels[levelIndex+1];
                max = nextLevel.PopulationThreshold-1;
            }

            Console.WriteLine($"Level '{currentLevel.GetReadableName()}' between {min} and {max}");
            _rangeDict.Add(new Tuple<int, int>(min, max), currentLevel);
        }
        
    }

    public ProvinceDevelopmentLevel Evaluate(int population)
    {
        foreach (KeyValuePair<Tuple<int, int>, ProvinceDevelopmentLevel> keyPairs in _rangeDict)
        {
            var range = keyPairs.Key;
            var level = keyPairs.Value;

            if (population >= range.Item1 && population <= range.Item2)
            {
                return level;
            }
        }

        throw new Exception($"Could not resolve a province development level for the given population: '{population}'");
    }
}