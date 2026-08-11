namespace PolitikServer.Core;

public class Nation : GameEntity
{
    public required string nameLong;
    public required string nameShort;
    public required string colorPrimary;
    public required string colorTertiary;
    public required string noun;

    public required SerializedList<ProvinceEntity> provincesControlled;
    public required SerializedField<ProvinceEntity> captiolProvince;

    public override string ToString()
    {
        return $"Nation [{UniqueIdentifier}], {nameLong}, Captiol: '{captiolProvince.Get().province.Get().UniqueIdentifier}'";
    }
}