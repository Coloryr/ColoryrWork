using ColoryrServer.SDK;
using System.IO;
using System.Text;

namespace ColoryrServer.Core.FileSystem.Html;

public static class WebBinManager
{
    private static readonly string WebBinStatic = ServerMain.RunLocal + "Static/";

    public static StaticDir BaseDir { get; private set; }

    public static byte[] GetFile(string[] url)
    {
        return BaseDir.GetFile(url, 0);
    }

    public static HttpResponseStream GetStream(HttpDllRequest request, string arg)
    {
        return FileHttpStream.StartStream(request, $"{WebBinStatic}/{arg}");
    }

    public static void Start()
    {
        if (!Directory.Exists(WebBinStatic))
            Directory.CreateDirectory(WebBinStatic);

        if (!File.Exists(WebBinStatic + "index.html"))
        {
            File.WriteAllText(WebBinStatic + "index.html", WebResource.IndexHtml, Encoding.UTF8);
        }

        if (!File.Exists(WebBinStatic + "404.html"))
        {
            File.WriteAllText(WebBinStatic + "404.html", WebResource._404Html, Encoding.UTF8);
        }

        if (!File.Exists(WebBinStatic + "favicon.ico"))
        {
            File.WriteAllBytes(WebBinStatic + "favicon.ico", WebResource.Icon);
        }

        BaseDir = new StaticDir(WebBinStatic);
        ServerMain.OnStop += BaseDir.Stop;
    }
}
