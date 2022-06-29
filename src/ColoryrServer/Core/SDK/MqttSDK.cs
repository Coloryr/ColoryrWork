﻿using ColoryrServer.Core.PortServer;
using MQTTnet.Protocol;
using MQTTnet.Server;

namespace ColoryrServer.SDK;

public static class MqttSDK
{
    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="topic">标题</param>
    /// <param name="data">数据</param>
    public static void Send(string topic, string data, MqttQualityOfServiceLevel level = MqttQualityOfServiceLevel.ExactlyOnce)
        => PortMqttServer.Send(topic, data, level);
}

public class DllMqttLoadingRetainedMessages
{
    public LoadingRetainedMessagesEventArgs Context { get; init; }
    /// <summary>
    /// MQTT加载信息
    /// </summary>
    /// <param name="context">数据</param>
    public DllMqttLoadingRetainedMessages(LoadingRetainedMessagesEventArgs context)
        => Context = context;
}

public class DllMqttConnectionValidator
{
    public ValidatingConnectionEventArgs Context { get; init; }
    /// <summary>
    /// MQTT服务器验证
    /// </summary>
    /// <param name="context">数据</param>
    public DllMqttConnectionValidator(ValidatingConnectionEventArgs context)
        => Context = context;
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
    /// <param name="context">数据</param>
    public DllMqttSubscription(InterceptingSubscriptionEventArgs context)
       => Context = context;
    /// <summary>
}

public class DllMqttUnsubscription
{
    public InterceptingUnsubscriptionEventArgs Context { get; init; }
    /// <summary>
    /// Mqtt取消订阅
    /// </summary>
    /// <param name="context">数据</param>
    public DllMqttUnsubscription(InterceptingUnsubscriptionEventArgs context)
       => Context = context;
}
