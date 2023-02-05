namespace ColoryrWork.Lib.Build.Object;

public record APIFileObj
{
    public Dictionary<string, string> List { get; init; } = new();
    public Dictionary<string, string> Other { get; init; } = new();
}
