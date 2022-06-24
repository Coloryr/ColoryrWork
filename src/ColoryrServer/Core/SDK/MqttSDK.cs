using ColoryrServer.Core.MQTT;
using MQTTnet.Protocol;
using MQTTnet.Server;

namespace ColoryrServer.SDK;

public static class MqttSDK
{
    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="Topic">标题</param>
    /// <param name="data">数据</param>
    public static void Send(string Topic, string data, MqttQualityOfServiceLevel level = MqttQualityOfServiceLevel.ExactlyOnce)
        => MQTTServer.Send(Topic, data, level);
}

public class DllMqttLoadingRetainedMessages
{
    public LoadingRetainedMessagesEventArgs Context { get; init; }
    /// <summary>
    /// MQTT加载信息
    /// </summary>
    /// <param name="Context">数据</param>
    public DllMqttLoadingRetainedMessages(LoadingRetainedMessagesEventArgs Context)
        => this.Context = Context;
}

public class DllMqttConnectionValidator
{
    public ValidatingConnectionEventArgs Context { get; init; }
    /// <summary>
    /// MQTT服务器验证
    /// </summary>
    /// <param name="Context">数据</param>
    public DllMqttConnectionValidator(ValidatingConnectionEventArgs Context)
        => this.Context = Context;
    /// <summary>
    /// 设置验证后的返回
    /// </summary>
    /// <param name="state">状态</param>
    public void SetReasonCode(MqttConnectReasonCode state)
        => Context.ReasonCode = state;
}

public class DllMqttMessage
{
    public RetainedMessageChangedEventArgs Context { get; init; }
    /// <summary>
    /// MQTT服务器收到消息
    /// </summary>
    /// <param name="Context">数据</param>
    public DllMqttMessage(RetainedMessageChangedEventArgs Context)
        => this.Context = Context;
}

public class DllMqttSubscription
{
    public InterceptingSubscriptionEventArgs Context { get; init; }
    /// <summary>
    /// Mqtt订阅
    /// </summary>
    /// <param name="Context">数据</param>
    public DllMqttSubscription(InterceptingSubscriptionEventArgs Context)
       => this.Context = Context;
    /// <summary>
}

public class DllMqttUnsubscription
{
    public InterceptingUnsubscriptionEventArgs Context { get; init; }
    /// <summary>
    /// Mqtt取消订阅
    /// </summary>
    /// <param name="Context">数据</param>
    public DllMqttUnsubscription(InterceptingUnsubscriptionEventArgs Context)
       => this.Context = Context;
}
