namespace PolitikServer.Core.Serialization;

public class SerializedProvinceModifier : SerializedGameDefinition
{
    public int baseDuration;
    public SerializedPair[] effects = [];
    public string followOnModifier = "";

    public override GameDefinition Deserialize()
    {
        ProvinceModifierEffect[] deserializedEffects = effects.Select(e => new ProvinceModifierEffect(e.key, e.value)).ToArray();

        return new ProvinceModifier(_uniqueIdentifier, baseDuration, deserializedEffects, followOnModifier);
    }
}