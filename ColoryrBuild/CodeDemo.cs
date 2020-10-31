namespace ColoryrBuild
{
    class CodeDemo
    {
        public const string main =
@"public dynamic main(HttpRequest http)
{
    return true;
}
";
        public const string main_ =
@"namespace ColoryrSDK
{
    public class app_{name}
    {
        public dynamic main(HttpRequest http)
        {  
            return true;
        }
	}
}
";
        public const string class_ =
@"namespace ColoryrSDK
{
    public class {name}
    {
        public {name}
        {
             
        }
    }
}
";
        public const string iot_ =
@"namespace ColoryrSDK
{
    public class {name}
    {
        public void main(IoTRequest head)
        {
             
        }
	}
}
";
        public const string websocket_ =
@"namespace ColoryrSDK
{
    public class {name}
    {
        public void main(WebSocketMessage head)
        {
             
        }
        public void open(WebSocketOpen head)
        {
             
        }
        public void close(WebSocketClose head)
        {
            
        }
	}
}
";
        public const string robot_ =
@"namespace ColoryrSDK
{
    public class {name}
    {
        public void main(RobotRequest head)
        {
            
        }
        public void after(RobotAfter head)
        {
            
        }
        public void robot(RobotEvent head)
        {
            
        }
    }
}
";
    }
}
