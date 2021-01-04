using System.Collections.Generic;

namespace Lib.Build.Object
{
    record APIFileObj
    {
        public Dictionary<string, string> list { get; init; } = new();
    }
}
