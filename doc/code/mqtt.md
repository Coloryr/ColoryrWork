# ColoryrServer

## Mqtt代码编写
[返回](code.md)

默认的Mqtt代码  

```C#
using ColoryrServer.SDK;

[DLLIN]
public class test
{
    public bool OnMessage(DllMqttMessage head)
    {
        return false; //true表示事件已处理完毕
    }
    public bool OnMessageLoading(DllMqttLoadingRetainedMessages head)
    {
        return false;
    }
    public bool OnValidator(DllMqttConnectionValidator head)
    {
        return false;
    }
    public bool OnSubscription(DllMqttSubscription head)
    {
        return false;
    }
    public bool OnUnsubscription(DllMqttUnsubscription head)
    {
        return false;
    }
}
```

类**必须**带有[ColoryrServer.SDK.DLLIN](../../src/ColoryrServer/Core/SDK/NotesSDK.cs#L21)的属性 

- `OnMessage`表示收到消息
- `OnMessageLoading`表示消息正在加载
- `OnValidator`表示有客户端正在验证链接
- `OnSubscription`表示有客户端正在订阅频道
- `OnUnsubscription`表示有客户端正在取消订阅频道

返回如果为true，则这个事件不会传到下个Dll中去
