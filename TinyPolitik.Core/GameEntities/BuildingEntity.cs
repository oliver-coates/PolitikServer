namespace PolitikServer.Core;

public class BuildingEntity : GameEntity
{
    public readonly SerializedField<BuildingType> buildingDefinition;

    public readonly SerializedNullableField<ProductionMode> productionMode;

    public int level;

    [Newtonsoft.Json.JsonConstructor]
    public BuildingEntity(string UniqueIdentifier, SerializedField<BuildingType> buildingDefinition, 
        SerializedNullableField<ProductionMode> productionMode, int level) : base(UniqueIdentifier)
    {
        this.buildingDefinition = buildingDefinition;
        this.productionMode = productionMode;
        this.level = level;
    }

    public BuildingEntity(BuildingType type, ProductionMode? mode) : base (Guid.NewGuid().ToString())
    {
        buildingDefinition = new SerializedField<BuildingType>(type);
     
        productionMode = new SerializedNullableField<ProductionMode>(mode);

        level = 1;
    }

    public override string GetReadableName()
    {
        if (productionMode.Get() == null)
        {
            return $"Building Entity of type '{buildingDefinition.Get().UniqueIdentifier}', no production mode.";
        }
        else
        {
            return $"Building Entity of type '{buildingDefinition.Get().UniqueIdentifier}', production mode is {productionMode.Get()}";
        }
    }

    internal override void Deserialize()
    {
        
    }
}