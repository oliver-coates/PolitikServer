namespace PolitikServer.Core;

/// <summary>
/// Responsible for creating all game entities at server intialisation.
/// </summary>
public class GameStateInitialiser
{
    public GameStateInitialiser()
    {
        SetupProvinces();

        SetupNations();
    }

    private void SetupProvinces()
    {
        Province[] provinces = DefinitionLibrary.GetAllDefinitionsOfType<Province>();

        foreach (Province p in provinces)
        {
            ProvinceEntity province = new()
            {
                UniqueIdentifier = $"province_{p.UniqueIdentifier}",
                province = p,
                population = WorldConfig.GetInt("province_starting_population"),
                buildings = [],
                ownerNation = null,
                occupierNation = null
            };

            EntityLibrary.AddEntity(province);
        }
    }

    private void SetupNations()
    {
        
    }
}