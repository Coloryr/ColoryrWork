using ColoryrServer.Core.DataBase;
using ColoryrServer.Core.FileSystem.Html;
using ColoryrWork.Lib.Build.Object;
using System.Collections.Concurrent;

namespace ColoryrServer.Core.FileSystem.Vue;

public static class VueBuildManager
{
    public static ConcurrentDictionary<string, VueBuild> NowBuild = new();
    public static ConcurrentDictionary<string, string> BuildRes = new();

    public static void StartBuild(WebObj web)
    {
        var item = new VueBuild(web);
        item.Init();
        NowBuild.TryAdd(web.UUID, item);
        item.Build();
    }

    public static void BuildDone(WebObj web) 
    {
        if (NowBuild.TryRemove(web.UUID, out var item))
        {
            BuildRes.AddOrUpdate(web.UUID, item.Res);
            WebFileManager.Add(web);
        }
    }

    public static bool IsBuildNow(string uuid) 
    {
        return NowBuild.ContainsKey(uuid);
    }

    public static string GetBuildRes(string uuid) 
    {
        if (BuildRes.TryGetValue(uuid, out var item))
        {
            return item;
        }

        return null;
    }

    public static bool IsBuildDone(string uuid) 
    {
        return BuildRes.ContainsKey(uuid);
    }
}
