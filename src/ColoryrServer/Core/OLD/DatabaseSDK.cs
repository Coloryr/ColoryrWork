using ColoryrServer.DataBase;

namespace ColoryrServer.SDK;

public partial class RamData
{
    /// <summary>
    /// 启用缓存
    /// </summary>
    /// <param name="Name">缓存名</param>
    public RamData(string Name)
    {
        this.Name = RamDataBase.NewCache(Name);
    }
}