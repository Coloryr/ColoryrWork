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
        public static HttpReturn Get(string Url)
        {
            byte[] temp;
            if (Url.IndexOf('.') == 1)
                temp = HtmlUtils.GetFileByName(Url);
            else
            {
                temp = HtmlUtils.GetFile(Url);
            }
            if (temp != null)
            {
                string type = ServerContentType.HTML;
                if (Url.EndsWith(".jpg") || Url.EndsWith(".jpge"))
                {
                    type = ServerContentType.JPEG;
                }
                else if (Url.ToLower().EndsWith(".png"))
                {
                    type = ServerContentType.PNG;
                }
                else if (Url.ToLower().EndsWith(".json"))
                {
                    type = ServerContentType.JSON;
                }
                else if (Url.ToLower().EndsWith(".xml"))
                {
                    type = ServerContentType.XML;
                }
                else if (Url.ToLower().EndsWith(".mp3"))
                {
                    type = ServerContentType.MP3;
                }
                else if (Url.ToLower().EndsWith(".mp4"))
                {
                    type = ServerContentType.MP4;
                }
                else if (Url.ToLower().EndsWith(".gif"))
                {
                    type = ServerContentType.GIF;
                }
                else if (Url.ToLower().EndsWith(".icon"))
                {
                    type = ServerContentType.ICO;
                }
                return new HttpReturn
                {
                    Data = temp,
                    ContentType = type
                };
            }
            return new HttpReturn
            {
                Data = HtmlUtils.Html404,
                ContentType = ServerContentType.HTML,
                ReCode = 200
            };
        }

        public static HttpReturn Get(string uuid, string name)
        {
            var temp = HtmlUtils.GetFile(uuid, name);
            if (temp != null)
            {
                string type = ServerContentType.HTML;
                if (name.EndsWith(".jpg") || name.EndsWith(".jpge"))
                {
                    type = ServerContentType.JPEG;
                }
                else if (name.ToLower().EndsWith(".png"))
                {
                    type = ServerContentType.PNG;
                }
                else if (name.ToLower().EndsWith(".json"))
                {
                    type = ServerContentType.JSON;
                }
                else if (name.ToLower().EndsWith(".xml"))
                {
                    type = ServerContentType.XML;
                }
                else if (name.ToLower().EndsWith(".mp3"))
                {
                    type = ServerContentType.MP3;
                }
                else if (name.ToLower().EndsWith(".mp4"))
                {
                    type = ServerContentType.MP4;
                }
                else if (name.ToLower().EndsWith(".gif"))
                {
                    type = ServerContentType.GIF;
                }
                else if (name.ToLower().EndsWith(".icon"))
                {
                    type = ServerContentType.ICO;
                }
                return new HttpReturn
                {
                    Data = temp,
                    ContentType = type
                };
            }
            return new HttpReturn
            {
                Data = HtmlUtils.Html404,
                ContentType = ServerContentType.HTML,
                ReCode = 200
            };
        }
    }
}
