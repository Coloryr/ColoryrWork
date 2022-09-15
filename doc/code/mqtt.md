# ColoryrServer

Mqtt代码编写

[返回](code.md)

默认的Mqtt代码  

```C#
using ColoryrServer.SDK;

//ColoryrServer_Debug

[MqttIN]
public class test
{
    public bool OnMessage(DllMqttMessage head)
    {
        return false; //true表示事件已处理完毕
    }
    public bool OnRetainedMessageChanged(DllMqttRetainedMessageChanged head)
    {
        return false;
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
    public bool OnClientConnected(DllMqttClientConnected head)
    {
        return false;
    }
    public bool OnClientDisconnected(DllMqttClientDisconnected head)
    {
        return false;
    }
    public bool OnInterceptingPublish(DllMqttInterceptingPublish head)
    {
        return false;
    }
}
```

类**必须**带有[ColoryrServer.SDK.DLLIN](../../src/ColoryrServer/Core/SDK/NotesSDK.cs#L21)的属性 

- `OnMessage`收到消息
- `OnRetainedMessageChanged`保留消息修改
- `OnMessageLoading`消息正在加载
- `OnValidator`有客户端正在验证链接
- `OnSubscription`有客户端正在订阅频道
- `OnUnsubscription`有客户端正在取消订阅频道
- `OnClientConnected`有客户端链接
- `OnClientDisconnected`有客户端断开链接
- `OnInterceptingPublish`有消息发送

返回如果为true，则这个事件不会传到下个Dll中去
