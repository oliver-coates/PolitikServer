namespace PolitikServer.Core;

public class BuildingEntity : GameEntity
{
    public readonly SerializedField<BuildingType> buildingDefinition;

    public readonly SerializedNullableField<ProductionMode> productionMode;



    [Newtonsoft.Json.JsonConstructor]
    public BuildingEntity(string UniqueIdentifier, SerializedField<BuildingType> buildingDefinition, 
        SerializedNullableField<ProductionMode> productionMode) : base(UniqueIdentifier)
    {
        this.buildingDefinition = buildingDefinition;
        this.productionMode = productionMode;
    }

    public BuildingEntity(BuildingType type, ProductionMode? mode) : base (Guid.NewGuid().ToString())
    {
        buildingDefinition = new SerializedField<BuildingType>(type);
     
        productionMode = new SerializedNullableField<ProductionMode>(mode);
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