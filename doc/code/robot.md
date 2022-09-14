# ColoryrServer

机器人代码编写

[返回](../code.md)

默认的机器人代码  

```C#
using ColoryrServer.SDK;

[RobotIN(new int[] {})] //在这里添加更多订阅的事件
public class test
{
    public bool OnMessage(RobotMessage head)
    {
        return false; //true表示事件已处理完毕
    }
    public bool OnMessagSend(RobotSend head)
    {
        return false;
    }
    //这里是更多事件的回调
    public bool OnRobotEvent(RobotEvent head)
    {
        return false;
    }
}
```

类**必须**带有[ColoryrServer.SDK.DLLIN](../../src/ColoryrServer/Core/SDK/NotesSDK.cs#L44)的属性 

- `OnMessage`表示收到消息
- `OnMessagSend`表示机器人发送消息后
- `OnRobotEvent`表示收到其他事件后，需要在类里面添加

返回如果为true，则这个事件不会传到下个Dll中去
