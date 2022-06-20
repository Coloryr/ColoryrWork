using System.Collections.Generic;

namespace ColoryrWork.Lib.Build.Object;

public record ClassCodeObj
{ 
    public string name { get; set; }
    public string code { get; set; }
}

public record ClassCodeGetObj
{
    public CSFileCode Obj { get; set; }
    public List<ClassCodeObj> List { get; set; }
}
