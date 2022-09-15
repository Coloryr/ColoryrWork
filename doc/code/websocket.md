# ColoryrServer

WebSocket代码编写

[返回](code.md)

默认的WebSocket代码   

```C#
using ColoryrServer.SDK;

[WebSocketIN]
public class test
{
    public bool OnMessage(WebSocketMessage head)
    {
        return false; //true表示事件已处理完毕
    }
    public bool OnOpen(WebSocketOpen head)
    {
        return false;
    }
    public bool OnClose(WebSocketClose head)
    {
        return false;
    }
}
```

类**必须**带有[ColoryrServer.SDK.DLLIN](../../src/ColoryrServer/Core/SDK/NotesSDK.cs#L21)的属性 

- `OnMessage`表示收到消息
- `OnOpen`表示有客户端链接后
- `OnClose`表示有客户端断开后

返回如果为true，则这个事件不会传到下个Dll中去
