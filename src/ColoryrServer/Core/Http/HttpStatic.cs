﻿using ColoryrServer.Core.FileSystem.Html;
using ColoryrServer.SDK;

namespace ColoryrServer.Core.Http;

public static class HttpStatic
{
    public static HttpReturn GetStatic(string[] arg)
    {
        var temp = WebBinManager.BaseDir.GetFile(arg, 0);
        if (temp != null)
        {
            return new()
            {
                Data = temp,
                ContentType = ServerContentType.EndType(arg[^1], true)
            };
        }
        return HttpReturnSave.Res404;
    }

    public static HttpResponseStream GetStream(HttpDllRequest request, string arg)
    {
        return WebBinManager.GetStream(request, arg);
    }
}
