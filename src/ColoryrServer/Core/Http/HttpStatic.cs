using ColoryrServer.Core.FileSystem;
using ColoryrServer.SDK;

namespace ColoryrServer.Core.Http;

public class HttpStatic
{
    public static HttpReturn Get(string uuid)
    {
        byte[] temp = HtmlUtils.GetByUUID(uuid);
        return new()
        {
            Data = temp ?? HtmlUtils.BaseDir.Html404,
            ContentType = ServerContentType.HTML
        };
    }

    public static HttpReturn Get(string uuid, string name)
    {
        var temp = HtmlUtils.GetFile(uuid, name);
        if (temp != null)
        {
            int a = name.LastIndexOf(".");
            name = name.ToLower()[a..];
            return new()
            {
                Data = temp,
                ContentType = name switch
                {
                    ".jpg" => ServerContentType.JPG,
                    ".jpge" => ServerContentType.JPEG,
                    ".png" => ServerContentType.PNG,
                    ".json" => ServerContentType.JSON,
                    ".xml" => ServerContentType.XML,
                    ".mp3" => ServerContentType.MP3,
                    ".mp4" => ServerContentType.MP4,
                    ".gif" => ServerContentType.GIF,
                    ".icon" => ServerContentType.ICO,
                    _ => ServerContentType.HTML,
                }
            };
        }
        return new()
        {
            Data = HtmlUtils.BaseDir.Html404,
            ContentType = ServerContentType.HTML
        };
    }

    public static HttpReturn GetStatic(string[] arg)
    {
        var temp = HtmlUtils.BaseDir.GetFile(arg, 0);
        if (temp != null)
        {
            var a = arg[^1].LastIndexOf(".");
            if (a != -1)
            {
                var name = arg[^1].ToLower()[a..];
                return new()
                {
                    Data = temp,
                    ContentType = name switch
                    {
                        ".jpg" => ServerContentType.JPG,
                        ".jpge" => ServerContentType.JPEG,
                        ".png" => ServerContentType.PNG,
                        ".json" => ServerContentType.JSON,
                        ".xml" => ServerContentType.XML,
                        ".mp3" => ServerContentType.MP3,
                        ".mp4" => ServerContentType.MP4,
                        ".gif" => ServerContentType.GIF,
                        ".ico" => ServerContentType.ICO,
                        ".icon" => ServerContentType.ICO,
                        _ => ServerContentType.HTML,
                    }
                };
            }
            return new()
            {
                Data = temp,
                ContentType = ServerContentType.HTML
            };
        }
        return new()
        {
            Data = HtmlUtils.BaseDir.Html404,
            ContentType = ServerContentType.HTML
        };
    }

    public static HttpResponseStream GetStream(HttpRequest request, string arg)
    {
        return HtmlUtils.GetStream(request, arg);
    }
}
