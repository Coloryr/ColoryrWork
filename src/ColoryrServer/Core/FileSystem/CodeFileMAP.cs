using ColoryrWork.Lib.Build.Object;
using System.Collections.Generic;

namespace ColoryrServer.Core.FileSystem;

internal record CodeFileMAP
{
    public List<CSFileObj> DllList { get; set; }
    public List<CSFileObj> ClassList { get; set; }
    public List<CSFileObj> SocketList { get; set; }
    public List<CSFileObj> WebSocketList { get; set; }
    public List<CSFileObj> RobotList { get; set; }
    public List<CSFileObj> MqttList { get; set; }
    public List<CSFileObj> ServiceList { get; set; }
}
