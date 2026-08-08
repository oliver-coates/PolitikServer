namespace PolitikServer.Core;

public class Nation : GameEntity
{
    public required string nameLong;
    public required string nameShort;
    public required string colorPrimary;
    public required string colorTertiary;
    public required string noun;

    public List<ProvinceEntity> provincesControlled = [];
    public required ProvinceEntity captiolProvince;
}