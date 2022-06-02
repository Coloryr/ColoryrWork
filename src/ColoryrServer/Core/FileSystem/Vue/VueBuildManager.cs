using ColoryrWork.Lib.Build.Object;
using System.Collections.Generic;

namespace ColoryrServer.Core.FileSystem.Vue;

public static class VueBuildManager
{
    public static Dictionary<string, VueBuild> NowBuild = new();

    public static void StartBuild(WebObj web)
    {
        var item = new VueBuild(web);
        item.Init();

    }
}
