using ColoryrServer.SDK;
using ColoryrWork.Lib.Server;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace ColoryrServer.Core.FileSystem.Managers;

internal class StreamTemp
{
    public required Stream stream;
    public int time;

    public void Tick() { time--; }
}

internal static class FileStreamManager
{
    private static readonly ConcurrentDictionary<string, StreamTemp> EtagTemp = new();

    private static bool IsRun;
    private static Thread TickThread;

    private const int ResetTime = 180;

    static FileStreamManager()
    {
        TickThread = new(() =>
        {
            while (IsRun)
            {
                Thread.Sleep(1000);
                foreach (var item in EtagTemp)
                {
                    item.Value.Tick();
                    if (item.Value.time == 0)
                        EtagTemp.TryRemove(item.Key, out var v);
                }
            }
        })
        {
            Name = "StreamTickThread"
        };
    }

    public static void Start()
    {
        IsRun = true;

        TickThread.Start();
        ServerMain.OnStop += Stop;
    }

    private static void Stop()
    {
        IsRun = false;
        foreach (var item in EtagTemp.Values)
        {
            item.stream.Dispose();
        }
        EtagTemp.Clear();
    }

    public static bool Have(string cookie)
    {
        return EtagTemp.ContainsKey(cookie);
    }

    public static Stream? LoadStream(string cookie)
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

    public static HttpResponseStream NewStream(HttpDllRequest http, string file, string contentType)
    {
        try
        {
            var etag = http.RowRequest["If-Match"];
            Stream stream;
            if (etag == null || http.Cookie.Count == 0 || http.Cookie.ContainsKey("etag"))
            {
                Guid guid = Guid.NewGuid();
                etag = guid.ToString();
            }
            else
            {
                if (!http.Cookie.TryGetValue("etag", out var list) || list.Count == 0)
                {
                    Guid guid = Guid.NewGuid();
                    etag = guid.ToString();
                }
                else
                {
                    etag = list[0];
                }
            }

            if (!Have(etag))
            {
                stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                EtagTemp.TryAdd(etag, new()
                {
                    stream = stream,
                    time = ResetTime
                });
            }
            else
            {
                EtagTemp[etag].stream.Dispose();
                stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                EtagTemp[etag].stream = stream;
                EtagTemp[etag].time = ResetTime;
            }

            int pos = 0;
            var range = http.RowRequest["Range"];
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
                Pos = pos,
                ContentType = contentType
            };
            res.AddHead("Content-Range", $"bytes {pos}-{stream.Length - 1}/{stream.Length}");
            res.AddHead("Etag", $"{etag}");
            return res;
        }
        catch (FileNotFoundException e1)
        {
            throw new ErrorDump("读取文件找不到", e1);
        }
        catch (Exception e)
        {
            throw new ErrorDump("流处理发生错误", e);
        }
    }
}
