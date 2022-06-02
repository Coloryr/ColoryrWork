using System.Collections.Generic;

namespace ColoryrWork.Lib.Build.Object;

public record ClassReadObj
{
    public string File { get; set; }
    public string Code { get; set; }
}

public record ClassCodeGetObj
{
    public CSFileCode Obj { get; set; }
    public List<ClassReadObj> List { get; set; }
}
