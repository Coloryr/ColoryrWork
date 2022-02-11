using System.Collections.Generic;

namespace ColoryrWork.Lib.Build.Object
{
    public record APIFileObj
    {
        public Dictionary<string, string> List { get; init; } = new();
    }
}
