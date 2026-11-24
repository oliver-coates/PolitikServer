namespace PolitikServer.Core;


public abstract class ServerInitialisationMode
{
    
}

public class InitialiseNewWorld : ServerInitialisationMode
{
    
}

public class InitialiseFromBackup : ServerInitialisationMode
{
    public string backupJson = "";
}