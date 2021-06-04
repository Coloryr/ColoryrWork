using ColoryrServer.SDK;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace ColoryrServer.FileSystem
{
    internal class FileHttpStream
    {
        private static ConcurrentDictionary<string, Stream> CookieTemp = new();
        public static string Local;

        public static void Start()
        {
            Local = ServerMain.RunLocal + @"/Stream/";
            if (!Directory.Exists(Local))
            {
                Directory.CreateDirectory(Local);
            }
        }

        public static void Stop()
        {
            foreach (var item in CookieTemp.Values)
            {
                item.Dispose();
            }
        }

        public static bool Have(string cookie)
        {
            return CookieTemp.ContainsKey(cookie);
        }

        public static Stream LoadStream(string cookie)
        {
            try
            {
                if (CookieTemp.TryGetValue(cookie, out var stream))
                    return stream;
                else
                    return null;
            }
            catch (Exception e)
            {
                throw new VarDump(e);
            }
        }

        public static HttpResponseStream StartStream(HttpRequest http, string local, string name)
        {
            try
            {
                string cookie;
                Stream stream;
                if (http.Cookie == null || http.Cookie == "")
                {
                    Guid guid = Guid.NewGuid();
                    cookie = guid.ToString();
                }
                else
                {
                    cookie = http.Cookie;
                }
                string file = Local + local + "/" + name;
                if (!File.Exists(file))
                {
                    return null;
                }
                if (!Have(cookie))
                {
                    stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                    CookieTemp.TryAdd(cookie, stream);
                }
                else
                {
                    CookieTemp[cookie].Dispose();
                    stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                    CookieTemp[cookie] = stream;
                }

                int pos = 0;
                string range = http.RowRequest["Range"];
                if (range != null)
                {
                    if (range.StartsWith("bytes="))
                    {
                        var temp = range.Replace("bytes=", "").Replace("-", "");
                        pos = int.Parse(temp);
                    }
                }

                var res = new HttpResponseStream()
                {
                    SetCookie = true,
                    Cookie = cookie,
                    Data = stream,
                    Pos = pos,
                    ContentType = ServerContentType.MP4
                };
                res.AddHead("ETag", "\"5f6f4e61-ff34c2\"");
                res.AddHead("Content-Range", $"bytes {pos}-{stream.Length - 1}/{stream.Length}");
                return res;
            }
            catch (Exception e)
            {
                throw new VarDump(e);
            }
        }
    }
}
