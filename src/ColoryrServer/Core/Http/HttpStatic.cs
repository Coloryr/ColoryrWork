using ColoryrServer.Core.FileSystem.Html;
using ColoryrServer.SDK;

namespace ColoryrServer.Core.Http;

public static class HttpStatic
{
    public static HttpReturn GetStatic(string[] arg)
    {
        var temp = WebBinManager.BaseDir.GetFile(arg, 0);
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
            Data = WebBinManager.BaseDir.Html404,
            ContentType = ServerContentType.HTML
        };
    }

    public static HttpResponseStream GetStream(HttpDllRequest request, string arg)
    {
        return WebBinManager.GetStream(request, arg);
    }
}
