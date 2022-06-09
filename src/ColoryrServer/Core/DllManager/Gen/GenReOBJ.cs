using System.IO;

namespace ColoryrServer.Core.DllManager.Gen;

internal record GenReOBJ
{
    /// <summary>
    /// 是否编译通过
    /// </summary>
    public bool Isok { get; set; }
    /// <summary>
    /// 编译信息
    /// </summary>
    public string Res { get; set; }
    /// <summary>
    /// 构建时间
    /// </summary>
    public string Time { get; set; }
    public MemoryStream MS { get; set; }
    public MemoryStream MSPdb { get; set; }
}
