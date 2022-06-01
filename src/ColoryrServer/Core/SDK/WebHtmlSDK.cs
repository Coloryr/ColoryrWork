using ColoryrServer.Core.FileSystem;
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
        var web = HtmlUtils.GetHtml(uuid);
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
            HtmlUtils.New(web);
        }
        else
        {
            if (!web.Codes.ContainsKey(name))
                HtmlUtils.AddCode(web, name, code);
            else
            {
                HtmlUtils.Save(web, name, code);
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
        var web = HtmlUtils.GetHtml(uuid);
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
            HtmlUtils.New(web);
        }
        else
        {
            if (!web.Codes.ContainsKey(name))
                HtmlUtils.AddFile(web, name, code);
            else
            {
                HtmlUtils.SaveFile(web, name, code);
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
        return HtmlUtils.GetFile(uuid, name);
    }
}
