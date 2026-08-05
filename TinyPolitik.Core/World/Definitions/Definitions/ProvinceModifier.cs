namespace PolitikServer.Core;

public class ProvinceModifier : GameDefinition
{
    public readonly int baseDuration;
    public readonly ProvinceModifierEffect[] effects;
    
    private string _followOnModifierUid = "";
    public ProvinceModifier? followOnModifier;

    public ProvinceModifier(string UniqueIdentifier, int duration, ProvinceModifierEffect[] effects, string followOn) : base(UniqueIdentifier)
    {
        baseDuration = duration;
        this.effects = effects;
        _followOnModifierUid = followOn;
    }

    public override void LateDeserialize()
    {
        if (_followOnModifierUid == "null" || string.IsNullOrEmpty(_followOnModifierUid))
        {
            followOnModifier = null;
            return;
        }
        else
        {
            followOnModifier = GameDefinitionLibrary.GetDefinition<ProvinceModifier>(_followOnModifierUid);
        }
    }

    public override string ToString()
    {
        return $"[{UniqueIdentifier}] Province Modifier. Duration: {baseDuration}, Follows on: {((followOnModifier != null) ? followOnModifier : "None")}, Effects: [{string.Join(",", effects.Select(o => $"{o.type}:{o.value}"))}]";
    }
}

public class ProvinceModifierEffect
{
    public string type = "";
    public string value = "";

    public ProvinceModifierEffect(string type, string value)
    {
        this.type = type;
        this.value = value;
    }
}