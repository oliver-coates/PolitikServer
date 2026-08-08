using System.Runtime.InteropServices.Marshalling;

namespace PolitikServer.Core;

public class TurnManager
{
    private Tuple<int, int>[] turnTimes;

    public TurnManager(GameConfig config)
    {
        // Convert turn times from string format e.g. "12:30" into int tuples of hour/minute
        turnTimes = new Tuple<int, int>[config.TurnTimesUtc.Length];
        
        int index = 0;
        foreach (string timeCode in config.TurnTimesUtc)
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

    }

    public DateTime GetNextScheduledTurnTime()
    {
        // Try for today
        foreach (Tuple<int, int> time in turnTimes)
        {
            DateTime proposedTime = DateTime.Today;

            proposedTime = proposedTime.AddHours(time.Item1);
            proposedTime = proposedTime.AddMinutes(time.Item2);

            // If the proposed time is after now, it is good for our turn time
            if (proposedTime > DateTime.UtcNow)
            {
                return proposedTime;
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
            if (proposedTime > DateTime.UtcNow)
            {
                return proposedTime;
            }
        }

        throw new Exception($"Error while finding next turn time. Could not find a valid turn time for today or tomorrow with the time codes: {string.Join(',', turnTimes.Select(t => $"{t.Item1:00}:{t.Item2:00}"))}");
    }
    
    /// <summary>
    /// Generates the metadata for turn 0 - the inital turn that is called when a server is started for the first time.
    /// </summary>
    public TurnMetaData GetStartingTurnMetaData()
    {
        DateTime nextTurn = GetNextScheduledTurnTime();
        
        return new TurnMetaData()
        {
            turnNumber = 0,
            timeOccuredUtc = DateTime.UtcNow,
            timeOccuredUtcBinary = DateTime.UtcNow.ToBinary(),
            nextTurnTimeScheduled = nextTurn,
            nextTurnTimeScheduledBinary = nextTurn.ToBinary(),
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