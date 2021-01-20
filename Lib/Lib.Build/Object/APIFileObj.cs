using System.Collections.Generic;

namespace Lib.Build.Object
{
    public record APIFileObj
    {
        public Dictionary<string, string> List { get; init; } = new();
    }
}
