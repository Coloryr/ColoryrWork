using ColoryrServer.SDK;
using ColoryrWork.Lib.Server;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace ColoryrServer.FileSystem
{
    class StreamTemp
    {
        public Stream stream;
        public int time;

        public void tick() { time--; }
    }
    public class FileHttpStream
    {
        private static readonly ConcurrentDictionary<string, StreamTemp> EtagTemp = new();
        public static string Local;

        private static bool IsRun;
        private static Thread thread;

        private const int ResetTime = 180;

        public static void Start()
        {
            Local = ServerMain.RunLocal + @"/Stream/";
            if (!Directory.Exists(Local))
            {
                Directory.CreateDirectory(Local);
            }
            IsRun = true;
            thread = new(() =>
            {
                while (IsRun)
                {
                    Thread.Sleep(1000);
                    foreach (var item in EtagTemp)
                    {
                        item.Value.tick();
                        if (item.Value.time == 0)
                            EtagTemp.TryRemove(item.Key, out var v);
                    }
                }
            });
            thread.Start();
        }

        public static void Stop()
        {
            IsRun = false;
            foreach (var item in EtagTemp.Values)
            {
                item.stream.Dispose();
            }
        }

        public static bool Have(string cookie)
        {
            return EtagTemp.ContainsKey(cookie);
        }

        public static Stream LoadStream(string cookie)
        {
            try
            {
                if (EtagTemp.TryGetValue(cookie, out var stream))
                {
                    stream.time = ResetTime;
                    return stream.stream;
                }
                else
                    return null;
            }
            catch (Exception e)
            {
                throw new ErrorDump("流处理发生错误", e);
            }
        }

        public static HttpResponseStream StartStream(HttpRequest http, string file)
        {
            try
            {
                string etag = http.RowRequest["If-Match"];
                Stream stream;
                if (etag == null || http.Cookie.Count == 0 || http.Cookie.ContainsKey("etag"))
                {
                    Guid guid = Guid.NewGuid();
                    etag = guid.ToString();
                }
                else
                {
                    http.Cookie.TryGetValue("etag", out var list);
                    if (list.Count == 0)
                    {
                        Guid guid = Guid.NewGuid();
                        etag = guid.ToString();
                    }
                    else
                    {
                        etag = list[0];
                    }
                }
                if (!File.Exists(file))
                {
                    return null;
                }
                if (!Have(etag))
                {
                    stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                    EtagTemp.TryAdd(etag, new()
                    {
                        stream = stream,
                        time = ResetTime
                    });
                }
                else
                {
                    EtagTemp[etag].stream.Dispose();
                    stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                    EtagTemp[etag].stream = stream;
                    EtagTemp[etag].time = ResetTime;
                }

                int pos = 0;
                string range = http.RowRequest["Range"];
                if (range != null)
                {
                    if (range.StartsWith("bytes="))
                    {
                        var temp = Function.GetSrings(range, "bytes=", "-", true);
                        pos = int.Parse(temp);
                    }
                }

                var res = new HttpResponseStream()
                {
                    Data = stream,
                    Pos = pos
                };
                res.AddHead("Content-Range", $"bytes {pos}-{stream.Length - 1}/{stream.Length}");
                res.AddHead("Etag", $"{etag}");
                return res;
            }
            catch (Exception e)
            {
                throw new ErrorDump("流处理发生错误", e);
            }
        }

        public static HttpResponseStream StartStream(HttpRequest http, string local, string name)
        {
            string file = Local + local + "/" + name;
            return StartStream(http, file);
        }
    }
}
