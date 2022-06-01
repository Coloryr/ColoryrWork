using ColoryrServer.Core.FileSystem.Html;
using System;

namespace ColoryrServer.SDK;

public class WebHtml
{
    /// <summary>
    /// 向前端添加Web资源
    /// </summary>
    /// <param name="uuid">资源UUID</param>
    /// <param name="name">名字</param>
    /// <param name="code">内容</param>
    public static void AddHtml(string uuid, string name, string code)
    {
        var web = WebFileManager.GetHtml(uuid);
        if (web == null)
        {
            var time = string.Format("{0:s}", DateTime.Now);
            web = new()
            {
                UUID = uuid,
                CreateTime = time,
                Text = "",
                Codes = new()
                {
                    { name, code }
                },
                Files = new()
            };
            WebFileManager.New(web);
        }
        else
        {
            if (!web.Codes.ContainsKey(name))
                WebFileManager.AddCode(web, name, code);
            else
            {
                WebFileManager.Save(web, name, code);
            }
        }
    }
    /// <summary>
    /// 向前端添加Web文件
    /// </summary>
    /// <param name="uuid">资源UUID</param>
    /// <param name="name">名字</param>
    /// <param name="code">内容</param>
    public static void AddFile(string uuid, string name, byte[] code)
    {
        var web = WebFileManager.GetHtml(uuid);
        if (web == null)
        {
            var time = string.Format("{0:s}", DateTime.Now);
            web = new()
            {
                UUID = uuid,
                CreateTime = time,
                Text = "",
                Codes = new(),
                Files = new()
            };
            WebFileManager.New(web);
        }
        else
        {
            if (!web.Codes.ContainsKey(name))
                WebFileManager.AddFile(web, name, code);
            else
            {
                WebFileManager.SaveFile(web, name, code);
            }
        }
    }
    /// <summary>
    /// 获取前端资源
    /// </summary>
    /// <param name="uuid">资源UUID</param>
    /// <param name="name">名字</param>
    /// <returns></returns>
    public static byte[] GetWeb(string uuid, string name)
    {
        return WebFileManager.GetFile(uuid, name);
    }
}
