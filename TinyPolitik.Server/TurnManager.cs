
namespace PolitikServer.Core;

public class TurnManager
{
    private int turnNumber;
    /// <summary>
    /// UTC turn times, in the format [Hour : Minute].
    /// </summary>
    private Tuple<int, int>[] turnTimes;
    /// <summary>
    /// The UTC time when the next turn is scheduled.
    /// </summary>
    public DateTime NextTurnTime { get; private set; }
    /// <summary>
    /// Metadata for the current turn.
    /// </summary>
    public TurnMetaData TurnMetaData { get; private set; }

    public TurnManager(GameConfig config)
    {
        // Convert turn times from string format e.g. "12:30" into int tuples of hour/minute
        turnTimes = new Tuple<int, int>[config.TurnTimesLocal.Length];
        
        int index = 0;
        foreach (string timeCode in config.TurnTimesLocal)
        {
            string[] split = timeCode.Split(':');
            try
            {
                int hour = int.Parse(split[0]);
                int minute = int.Parse(split[1]);                

                turnTimes[index] = new Tuple<int, int>(hour, minute);
            }
            catch (Exception e)
            {
                throw new Exception($"Exception while parsing turn time '{timeCode}': {e}");
            }
            index++;
        }

        TurnMetaData = new(); // This will be overwritten by either the Initialise method or by loading the saved turn metadata
    }

    public void Initialise()
    {
        // Initialise with turn meta data
        TurnMetaData = GenerateMetaData();
    }

    /// <summary>
    /// Gets the next scheduled turn time - updates the NextTurnTime field. 
    /// </summary>
    public void DetermineNextScheduledTurnTime()
    {
        // Try for today
        foreach (Tuple<int, int> time in turnTimes)
        {
            DateTime proposedTime = DateTime.Today;

            proposedTime = proposedTime.AddHours(time.Item1);
            proposedTime = proposedTime.AddMinutes(time.Item2);

            // If the proposed time is after now, it is good for our turn time
            if (proposedTime > DateTime.Now)
            {
                NextTurnTime = proposedTime;
                return;
            }
        }
    
        // If that doesn't work, try for tomorrow:
        foreach (Tuple<int, int> time in turnTimes)
        {
            DateTime proposedTime = DateTime.Today;
            proposedTime = proposedTime.AddDays(1);

            proposedTime = proposedTime.AddHours(time.Item1);
            proposedTime = proposedTime.AddMinutes(time.Item2);

            // If the proposed time is after now, it is good for our turn time
            if (proposedTime > DateTime.Now)
            {
                NextTurnTime = proposedTime;
                return;
            }
        }

        throw new Exception($"Error while finding next turn time. Could not find a valid turn time for today or tomorrow with the time codes: {string.Join(',', turnTimes.Select(t => $"{t.Item1:00}:{t.Item2:00}"))}");
    }
    
    /// <summary>
    /// Generates the metadata for turn 0 - the inital turn that is called when a server is started for the first time.
    /// </summary>
    public void InitialiseForStartingTurn()
    {
        turnNumber = 0;
        
        DetermineNextScheduledTurnTime();
        
        TurnMetaData = GenerateMetaData();
    }

    /// <summary>
    /// Advances the next turn.
    /// Called at the end of the turn resolution process.
    /// </summary>
    public void AdvanceToNextTurn()
    {
        turnNumber += 1;

        DetermineNextScheduledTurnTime();
     
        TurnMetaData = GenerateMetaData();
    }

    private TurnMetaData GenerateMetaData()
    {
        return new TurnMetaData()
        {
            turnNumber = turnNumber,
            timeOccuredUtc = DateTime.UtcNow,
            timeOccuredUtcBinary = DateTime.UtcNow.ToBinary(),
            nextTurnTimeScheduled = NextTurnTime,
            nextTurnTimeScheduledBinary = NextTurnTime.ToBinary(),
        };
    }
}

public class TurnMetaData
{
    public int turnNumber = 0;

    public DateTime timeOccuredUtc;
    public long timeOccuredUtcBinary;

    public DateTime nextTurnTimeScheduled;
    public long nextTurnTimeScheduledBinary;
}