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
        Province[] provinceDefinitions = DefinitionLibrary.GetAllDefinitionsOfType<Province>();
        Dictionary<Province, ProvinceEntity> provinceDict = new(); 

        foreach (Province pDef in provinceDefinitions)
        {
            ProvinceEntity newProvince = CreateNewProvince(pDef);
            EntityLibrary.AddEntity(newProvince);
            provinceDict.Add(pDef, newProvince);
        }

        // Iterate back across each newly created province entity and set up their connected provinces
        foreach (KeyValuePair<Province, ProvinceEntity> pair in provinceDict)
        {
            List<ProvinceEntity> connectedProvinces = new();
            foreach (Province bordered in pair.Key.ConnectedProvinces)
            {
                connectedProvinces.Add(provinceDict[bordered]);
            }
            pair.Value.connectedProvinces = new SerializedList<ProvinceEntity>(connectedProvinces);
        }
    }

    private ProvinceEntity CreateNewProvince(Province definition)
    {
        float popVariance = WorldConfig.GetFloat("province_starting_population_variance");
        int popBase = WorldConfig.GetInt("province_starting_population");
        int pop = (int) RandomUtil.ApplyVariance(popBase, popVariance, RandomUtil.VarianceMethod.Multiplicative);

        return new ProvinceEntity()
        {
            UniqueIdentifier = Guid.NewGuid().ToString(),
            province = new SerializedField<Province>(definition),
            population = pop,
            buildings = [],
            ownerNation = new SerializedNullableField<Nation?>(),
            occupierNation = new SerializedNullableField<Nation?>()
        };
    }

    /// <summary>
    /// Populates the world with AI nations.
    /// </summary>
    private void SetupNations()
    {
        ProvinceEntity[] provinces = EntityLibrary.GetAllEntitiesOfType<ProvinceEntity>();  

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
            EntityLibrary.AddEntity(newNation);
        }
    }

    private ProvinceEntity? GetRandomNationStartingProvince()
    {
        string doAllowBorderSpawns = WorldConfig.Get("do_allow_ai_nation_spawn_at_border");
        ProvinceEntity[] allProvinces = [.. EntityLibrary.GetAllEntitiesOfType<ProvinceEntity>().Shuffle()];

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
        RandomlyGeneratedName randomName = RandomCountryNameGenerator.Generate();
        ProvinceEntity capitol = RandomUtil.Pick(provinces);

        Nation newNation = new()
        {
            UniqueIdentifier = Guid.NewGuid().ToString(),
            nameShort = randomName.shortName,
            nameLong = randomName.longName,
            colorPrimary = "not implemented",
            colorTertiary = "not implemented",
            noun = randomName.noun,
            captiolProvince = new SerializedField<ProvinceEntity>(capitol),
            provincesControlled = new SerializedList<ProvinceEntity>(provinces),
        };  

        // Register the new nation with all of its controlled provinces
        foreach (ProvinceEntity controlledProvince in provinces)
        {
            controlledProvince.occupierNation.Set(newNation);
            controlledProvince.ownerNation.Set(newNation);
        }

        return newNation;
    }
}