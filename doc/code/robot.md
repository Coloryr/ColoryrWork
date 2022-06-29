# ColoryrServer

## 机器人代码编写
[返回](code.md)

默认的机器人代码  

```C#
using ColoryrServer.SDK;

[DLLIN]
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
    public bool OnRobotEvent(RobotEvent head)
    {
        return false;
    }
}
```

类**必须**带有[ColoryrServer.SDK.DLLIN](../../src/ColoryrServer/Core/SDK/NotesSDK.cs#L21)的属性 

- `OnMessage`表示收到消息
- `OnMessagSend`表示机器人发送消息后
- `OnRobotEvent`表示收到其他事件后，需要在服务器配置中添加

返回如果为true，则这个事件不会传到下个Dll中去
