using System;
using System.Collections.Generic;

namespace ColoryrWork.Lib.Build.Object;

public enum CodeType
{
    Dll, Class, Socket, WebSocket, Robot, Mqtt, Task, Web
}
public class CSFileObj
{
    /// <summary>
    /// 名字
    /// </summary>
    public string UUID { get; set; }
    /// <summary>
    /// 注释
    /// </summary>
    public string Text { get; set; }
    /// <summary>
    /// 版本
    /// </summary>
    public int Version { get; set; }
    /// <summary>
    /// 创建时间
    /// </summary>
    public string CreateTime { get; set; }
    /// <summary>
    /// 修改时间
    /// </summary>
    public string UpdateTime { get; set; }
    public void Up()
    {
        var time = string.Format("{0:s}", DateTime.Now);
        UpdateTime = time;
        Version++;
    }
    public void Next()
    {
        Version++;
    }

}
public class CSFileCode : CSFileObj
{
    public CSFileCode(CSFileObj obj)
    {
        UUID = obj.UUID;
        Text = obj.Text;
    }
    public CSFileCode()
    {
    }
    public string Code { get; set; }
    public CodeType Type { get; set; }
}
public class WebObj : CSFileObj
{
    public bool IsVue { get; set; }
    public Dictionary<string, string> Codes { get; set; }
    public List<string> Files { get; set; }
}
public record CSFileList
{
    public Dictionary<string, CSFileObj> List { get; set; } = new();
}
