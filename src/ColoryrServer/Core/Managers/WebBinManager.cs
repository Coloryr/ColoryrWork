using ColoryrServer.SDK;
using System;
using System.IO;

namespace ColoryrServer.Core.FileSystem.Managers;

public static class WebBinManager
{
    private static readonly string WebBinStatic = ServerMain.RunLocal + "Static/";

    public static Action Reload;

    public static StaticDictionary BaseDir { get; private set; }

    public static byte[] GetFile(string[] url)
    {
        return BaseDir.GetFile(url, 0);
    }

    public static HttpResponseStream GetStream(HttpDllRequest request, string arg)
    {
        return FileStreamManager.NewStream(request, $"{WebBinStatic}/{arg}",
            ServerContentType.EndType(arg));
    }

    public static void Start()
    {
        if (!Directory.Exists(WebBinStatic))
            Directory.CreateDirectory(WebBinStatic);

        if (!File.Exists(WebBinStatic + "index.html"))
        {
            File.WriteAllBytes(WebBinStatic + "index.html", WebResource.IndexHtml);
        }

        if (!File.Exists(WebBinStatic + "404.html"))
        {
            File.WriteAllBytes(WebBinStatic + "404.html", WebResource.Html404);
        }

        if (!File.Exists(WebBinStatic + "fixmode.html"))
        {
            File.WriteAllBytes(WebBinStatic + "fixmode.html", WebResource.FixHtml);
        }

        if (!File.Exists(WebBinStatic + "favicon.ico"))
        {
            File.WriteAllBytes(WebBinStatic + "favicon.ico", WebResource.Icon);
        }

        BaseDir = new StaticDictionary(WebBinStatic);

        ServerMain.OnStop += BaseDir.Stop;
    }
}
