using ColoryrWork.Lib.Build.Object;

namespace ColoryrServer.Core.FileSystem.Database;

internal static class CodeObj
{
    public static QCodeObj ToQCode(this CSFileCode obj)
    {
        return new QCodeObj()
        {
            uuid = obj.UUID,
            text = obj.Text,
            version = obj.Version,
            createtime = obj.CreateTime,
            updatetime = obj.UpdateTime
        };
    }
}

internal class QCodeObj
{
    public string uuid { get; set; }
    public string text { get; set; }
    public int version { get; set; }
    public string code { get; set; }
    public string createtime { get; set; }
    public string updatetime { get; set; }
    public CSFileCode ToCode()
    {
        return new()
        {
            UUID = uuid,
            Text = text,
            Version = version,
            CreateTime = createtime,
            UpdateTime = updatetime,
            Code = code
        };
    }
}