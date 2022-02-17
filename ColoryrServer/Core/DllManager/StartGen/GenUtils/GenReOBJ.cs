using System.IO;

namespace ColoryrServer.DllManager.StartGen.GenUtils;

internal record GenReOBJ
{
    public bool Isok { get; set; }
    public string Res { get; set; }
    public string Time { get; set; }
    public MemoryStream MS { get; set; }
    public MemoryStream MSPdb { get; set; }
}
