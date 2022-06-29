using ColoryrServer.Core.FileSystem.Web;
using ColoryrServer.Core.Http;

namespace ColoryrServer.SDK;

public static class WebHtml
{
    /// <summary>
    /// 获取动态前端资源
    /// </summary>
    /// <param name="uuid"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static byte[] GetWebFile(string uuid, string name)
    {
        var route = HttpInvokeRoute.Get(uuid);
        return route.Invoke(null, name).Data as byte[];
    }
    /// <summary>
    /// 获取静态前端资源
    /// </summary>
    /// <param name="uuid">资源UUID</param>
    /// <param name="name">名字</param>
    /// <returns></returns>
    public static byte[] GetStaticWebFile(string[] uuid)
    {
        return WebBinManager.GetFile(uuid);
    }
}
