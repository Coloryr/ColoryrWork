﻿using ColoryrServer.Core.Database;
using ColoryrServer.Core.PortServer;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System;

namespace ColoryrServer.SDK;

public static class MqttSDK
{
    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="topic">标题</param>
    /// <param name="data">数据</param>
    public static void Send(string topic, string data, string id = "ColoryrServer",
        MqttQualityOfServiceLevel level = MqttQualityOfServiceLevel.ExactlyOnce)
        => PortMqttServer.Send(topic, data, id, level).Wait();

    public static void SendAsync(string topic, string data, string id = "ColoryrServer",
        MqttQualityOfServiceLevel level = MqttQualityOfServiceLevel.ExactlyOnce)
    {
        try
        {
            PortMqttServer.Send(topic, data, id, level);
        }
        catch (Exception e)
        {
            LogDatabsae.PutError("ColoryrServer", e.ToString());
        }
    }
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
    public ApplicationMessageNotConsumedEventArgs Context { get; init; }
    /// <summary>
    /// MQTT服务器收到消息
    /// </summary>
    /// <param name="Context">数据</param>
    public DllMqttMessage(ApplicationMessageNotConsumedEventArgs Context)
        => this.Context = Context;
}

public class DllMqttRetainedMessageChanged
{
    public RetainedMessageChangedEventArgs Context { get; init; }
    /// <summary>
    /// MQTT保留消息修改
    /// </summary>
    /// <param name="Context">数据</param>
    public DllMqttRetainedMessageChanged(RetainedMessageChangedEventArgs Context)
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

public class DllMqttClientConnected
{
    public ClientConnectedEventArgs Context { get; init; }
    /// <summary>
    /// Mqtt客户端链接
    /// </summary>
    /// <param name="context"></param>
    public DllMqttClientConnected(ClientConnectedEventArgs context)
        => Context = context;
}

public class DllMqttClientDisconnected
{
    public ClientDisconnectedEventArgs Context { get; init; }
    /// <summary>
    /// Mqtt客户端断开链接
    /// </summary>
    /// <param name="context"></param>
    public DllMqttClientDisconnected(ClientDisconnectedEventArgs context)
        => Context = context;
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

public class DllMqttInterceptingPublish
{
    public InterceptingPublishEventArgs Context { get; init; }
    /// <summary>
    /// Mqtt推送消息
    /// </summary>
    /// <param name="context">数据</param>
    public DllMqttInterceptingPublish(InterceptingPublishEventArgs context)
       => Context = context;
}