namespace TinyPolitik.Core;

public interface IGameDefintion
{
    public string Id {get; init; }
}

public record StrategicResourceDefinition : IGameDefintion
{
    public string Id {get; init; } = "";
    public string NameNonTechnical { get; init; } = "";
    public string NameTechnical { get; init; } = "";

    public int SellPriceBase { get; init; } = 0;
}