namespace ColoryrServer.DllManager
{
    class CodeDemo
    {
        public const string DllMain = "main";
        public const string IoTTcp = "tcpmessage";
        public const string IoTUdp = "udpmessage";
        public const string WebSocketMessage = "message";
        public const string WebSocketOpen = "open";
        public const string WebSocketClose = "close";
        public const string RobotMessage = "message";
        public const string RobotEvent = "robot";
        public const string RobotSend = "after";
        public const string MQTTMessage = "message";
        public const string MQTTValidator = "check";
        public const string MQTTSubscription = "subscription";

        public const string dll_ =
@"using ColoryrServer.SDK;
namespace ColoryrServer
{
    public class app_{name}
    {
        public dynamic " + DllMain + @"(HttpRequest http)
        {  
            return " + "\"true\"" + @";
        }
    }
}
";
        public const string class_ =
@"using ColoryrServer.SDK;
namespace ColoryrServer
{
    public class {name}
    {
        public {name}()
        {
             
        }
    }
}
";
        public const string iot_ =
@"using ColoryrServer.SDK;
namespace ColoryrServer
{
    public class {name}
    {
        public void " + IoTTcp + @"(TcpIoTRequest head)
        {
             
        }
        public void " + IoTUdp + @"(UdpIoTRequest head)
        {
             
        }
    }
}
";
        public const string websocket_ =
@"using ColoryrServer.SDK;
namespace ColoryrServer
{
    public class {name}
    {
        public void " + WebSocketMessage + @"(WebSocketMessage head)
        {
             
        }
        public void " + WebSocketOpen + @"(WebSocketOpen head)
        {
             
        }
        public void " + WebSocketClose + @"(WebSocketClose head)
        {
            
        }
    }
}
";
        public const string robot_ =
@"using ColoryrServer.SDK;
namespace ColoryrServer
{
    public class {name}
    {
        public void " + RobotMessage + @"(RobotRequest head)
        {
            
        }
        public void " + RobotSend + @"(RobotAfter head)
        {
            
        }
        public void " + RobotEvent + @"(RobotEvent head)
        {
            
        }
    }
}
";
        public const string mqtt_ =
@"";
    }
}
