using ColoryrServer.FileSystem;
using ColoryrServer.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColoryrServer.Http
{
    public class HttpStatic
    {
        public static HttpReturn Get(string url)
        {
            byte[] temp;
            if (url.IndexOf('.') > 0)
                temp = HtmlUtils.GetFileByName(url);
            else
                temp = HtmlUtils.GetByUUID(url);
            if (temp != null)
            {
                int a = url.IndexOf('.') > 0 ? url.LastIndexOf(".") : 0;
                url = url.ToLower()[a..];
                return new()
                {
                    Data = temp,
                    ContentType = url switch
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
                Data = HtmlUtils.Html404,
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
                Data = HtmlUtils.Html404,
                ContentType = ServerContentType.HTML
            };
        }
    }
}
