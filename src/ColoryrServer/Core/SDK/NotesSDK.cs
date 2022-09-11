using System;
using System.Collections.Generic;

namespace ColoryrServer.SDK;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class NotesSDK : Attribute
{
    public string Text;
    public string[] Input;
    public string[] Output;

    public NotesSDK(string text, string[] input = null, string[] output = null)
    {
        Text = text;
        Input = input ?? new string[1];
        Output = output ?? new string[1];
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class DllIN : Attribute
{
    public bool Debug;
    public DllIN(bool debug = false)
    {
        Debug = debug;
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ClassIN : Attribute
{

}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class MqttIN : Attribute
{

}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class RobotIN : Attribute
{
    public List<int> Event = new();
    public RobotIN(int[] list)
    {
        Event.AddRange(list);
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class SocketIN : Attribute
{
    public bool Netty;
    public SocketIN(bool netty = false)
    {
        Netty = netty;
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class TaskIN : Attribute
{

}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class WebSocketIN : Attribute
{

}