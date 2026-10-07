using Microsoft.Extensions.Logging;

namespace PolitikServer.Core;

/// <summary>
/// Responsible for creating all game entities at server intialisation.
/// </summary>
public class GameStateInitialiser
{
    private readonly ILogger<GameStateInitialiser> _logger;
    private readonly DefinitionLibrary _definitions;
    private readonly EntityLibrary _entities;

    public GameStateInitialiser(ILogger<GameStateInitialiser> logger, DefinitionLibrary definitions, EntityLibrary entities)
    {
        _logger = logger;
        _definitions = definitions;
        _entities = entities;
    }

    public void Initialise()
    {
        SetupProvinces();

        SetupNations();
    }

    private void SetupProvinces()
    {
        ProvinceDefinition[] provinceDefinitions = _definitions.GetAll<ProvinceDefinition>();
        Dictionary<ProvinceDefinition, ProvinceEntity> provinceDict = new(); 

        foreach (ProvinceDefinition pDef in provinceDefinitions)
        {
            ProvinceEntity newProvince = CreateNewProvince(pDef);
            _entities.Add<ProvinceEntity>(newProvince);
            provinceDict.Add(pDef, newProvince);
        }

        // Iterate back across each newly created province entity and set up their connected provinces
        foreach (KeyValuePair<ProvinceDefinition, ProvinceEntity> pair in provinceDict)
        {
            List<ProvinceEntity> connectedProvinces = new();
            foreach (ProvinceDefinition bordered in pair.Key.ConnectedProvinces)
            {
                connectedProvinces.Add(provinceDict[bordered]);
            }
            pair.Value.connectedProvinces = new SerializedList<ProvinceEntity>(connectedProvinces);
        }
    }

    private ProvinceEntity CreateNewProvince(ProvinceDefinition definition)
    {
        float popVariance = WorldConfig.GetFloat("province_starting_population_variance");
        int popBase = WorldConfig.GetInt("province_starting_population");
        int pop = (int) RandomUtil.ApplyVariance(popBase, popVariance, RandomUtil.VarianceMethod.Multiplicative);
        var level = ProvinceDevelopmentLevel.Evaluate(_definitions.GetAll<ProvinceDevelopmentLevel>(), pop);
        ProvinceEntity[] connectedProvinces = []; // TODO: Implement this.

        ProvinceEntity newProvince = new(
            Guid.NewGuid().ToString(), 
            definition, 
            connectedProvinces, 
            null, 
            null, 
            pop, 
            level
        );

        _logger.LogInformation("Initialised: {newProvince}", newProvince);

        return newProvince;
    }

    /// <summary>
    /// Populates the world with AI nations.
    /// </summary>
    private void SetupNations()
    {
        ProvinceEntity[] provinces = _entities.GetAll<ProvinceEntity>();  

        // How many nations to spawn?
        int numProvinces = provinces.Length;
        float nationPercentage = WorldConfig.GetFloat("starting_nations_world_percentage");
        int numNationsToSpawn = (int)(numProvinces * nationPercentage);

        // Spawn each nation
        for (int nationIndex = 0; nationIndex < numNationsToSpawn; nationIndex++)
        {
            // Attempt to randomly pick a province:
            ProvinceEntity? startingProvince = GetRandomNationStartingProvince();

            if (startingProvince == null)
            {
                // We couldn't get a starting province for this nation.
                // TODO: Log some kind of warning here.
                Console.WriteLine("Warning: Could not find a starting province when creating nations");
                break;
            }
            
            Nation newNation = GenerateNation([startingProvince]);
            _entities.Add<Nation>(newNation);
        }
        
    }

    private ProvinceEntity? GetRandomNationStartingProvince()
    {
        string doAllowBorderSpawns = WorldConfig.Get("do_allow_ai_nation_spawn_at_border");
        ProvinceEntity[] allProvinces = [.. _entities.GetAll<ProvinceEntity>().Shuffle()];

        switch (doAllowBorderSpawns)
        {
            case "true":
                // Iterate across all until we find a non-occupied province.
                for (int index = 0; index < allProvinces.Length; index++)
                {
                    ProvinceEntity prov = allProvinces[index];
                
                    // Ensure it isn't occupied
                    if (prov.ownerNation.Get() != null) { continue; }

                    return prov;
                }
                // Could not find a non-owned province.
                return null;
                        
            case "false":
                // Iterate across all until we find a non-occupied province that is non connected to an owned one.
                for (int index = 0; index < allProvinces.Length; index++)
                {
                    ProvinceEntity prov = allProvinces[index];
                    
                    // Ensure that this province isn't owned
                    if (prov.ownerNation.Get() != null) { continue; }

                    // Ensure none of the adjacent provinces are occupied
                    bool areConnectedOccupied = false;
                    foreach (var connectedProv in prov.connectedProvinces)
                    {
                        if (connectedProv.ownerNation.Get() != null) 
                        { 
                            areConnectedOccupied = true;
                            break;
                        }                        
                    } 
                    // Only accept if none of the adjacent provinces are occupied.
                    if (areConnectedOccupied) { continue; }
                    
                    return prov; 
                }
                // Could not find a non-owned province.
                return null;
            
            case "avoid":
                ProvinceEntity? unownedProvince = null;
                // First, iterate across all and try to find a non-bordered province that is non connected to an owned one.
                for (int index = 0; index < allProvinces.Length; index++)
                {
                    ProvinceEntity prov = allProvinces[index];
                    
                    // Ensure that this province isn't owned
                    if (prov.ownerNation.Get() != null) { continue; }
                    unownedProvince = prov; // < Save any unoccupied provinces incase we have to fallback to them

                    // Ensure none of the adjacent provinces are occupied
                    bool areConnectedOccupied = false;
                    foreach (var connectedProv in prov.connectedProvinces)
                    {
                        if (connectedProv.ownerNation.Get() != null) 
                        { 
                            areConnectedOccupied = true;
                            break;
                        }                        
                    } 
                    // Only accept if none of the adjacent provinces are occupied.
                    if (areConnectedOccupied) { continue; }
                    
                    return prov; 
                }
                // Second, since we couldn't find a non-bordered province, try to fall back to just a bordered unoccpied province.
                if (unownedProvince != null)
                {
                    // Fall back to this
                    return unownedProvince;
                }
                // We couldn't fallback to a bordered, unoccupied province, so return null.
                return null;
            
            default:
                throw new Exception($"Unhandled 'doAllowBorderSpawns' value of '{doAllowBorderSpawns}'. It must be either 'true', 'false', or 'avoid' ");
        }        
    }

    private Nation GenerateNation(ProvinceEntity[] provinces)
    {
        Nation.Decoration randDecoration = RandomCountryGenerator.Generate();
        ProvinceEntity capitol = RandomUtil.Pick(provinces);

        Nation newNation = new(
            Guid.NewGuid().ToString(),
            null,
            randDecoration,
            provinces,
            capitol
        );

        // Register the new nation with all of its controlled provinces
        foreach (ProvinceEntity controlledProvince in provinces)
        {
            controlledProvince.occupierNation.Set(newNation);
            controlledProvince.ownerNation.Set(newNation);
        }

        _logger.LogInformation("Initialised: {newNation}", newNation);

        return newNation;
    }


}

