namespace PolitikServer.Core;

public class ProvinceDevelopmentResolver
{
    private readonly DefinitionLibrary _definitions;

    public ProvinceDevelopmentResolver(DefinitionLibrary definitions)
    {
        _definitions = definitions;

        WorldInitialiser.OnInitialiseEarly += Initialise;
    }

    public void Initialise(ServerInitialisationMode mode)
    {
        
    }
}