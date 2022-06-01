using ColoryrWork.Lib.Build.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColoryrServer.Core.FileSystem.Code;

internal record CodeFileMAP
{
    public List<CSFileObj> DllList { get; set; }
    public List<CSFileObj> ClassList { get; set; }
    public List<CSFileObj> SocketList { get; set; }
    public List<CSFileObj> WebSocketList { get; set; }
    public List<CSFileObj> RobotList { get; set; }
    public List<CSFileObj> MqttList { get; set; }
    public List<CSFileObj> TaskList { get; set; }
}
