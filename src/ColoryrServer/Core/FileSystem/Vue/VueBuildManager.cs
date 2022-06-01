using ColoryrWork.Lib.Build.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
