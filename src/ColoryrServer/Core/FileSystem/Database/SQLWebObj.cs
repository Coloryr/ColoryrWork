﻿using ColoryrWork.Lib.Build.Object;

namespace ColoryrServer.Core.FileSystem.Database;

public record QWebObj
{
    public string uuid { get; set; }
    public string text { get; set; }
    public int version { get; set; }
    public bool vue { get; set; }
    public string createtime { get; set; }
    public string updatetime { get; set; }

    public WebObj ToWeb()
    {
        return new()
        {
            UUID = uuid,
            Text = text,
            Version = version,
            CreateTime = createtime,
            UpdateTime = updatetime,
            IsVue = vue,
            Codes = new(),
            Files = new()
        };
    }
}

public record FWebObj
{
    public string uuid { get; set; }
    public string name { get; set; }
    public byte[] data { get; set; }
    public string time { get; set; }
}

public record CWebObj
{
    public string uuid { get; set; }
    public string name { get; set; }
    public string code { get; set; }
    public string time { get; set; }
}