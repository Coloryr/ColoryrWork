using ColoryrServer.Core.MQTT;
using MQTTnet.Protocol;
using MQTTnet.Server;

namespace ColoryrServer.SDK;

public class MqttConnectionValidator
{
    public MqttConnectionValidatorContext Context { get; init; }
    /// <summary>
    /// MQTT服务器验证
    /// </summary>
    /// <param name="Context">数据</param>
    public MqttConnectionValidator(MqttConnectionValidatorContext Context)
        => this.Context = Context;
    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="Topic">标题</param>
    /// <param name="data">数据</param>
    public void Send(string Topic, string data, MqttQualityOfServiceLevel level = MqttQualityOfServiceLevel.ExactlyOnce)
        => MQTTServer.Send(Topic, data, level);
    /// <summary>
    /// 设置验证后的返回
    /// </summary>
    /// <param name="state">状态</param>
    public void SetReasonCode(MqttConnectReasonCode state)
        => Context.ReasonCode = state;
}

public class MqttMessage
{
    public MqttApplicationMessageInterceptorContext Context { get; init; }
    /// <summary>
    /// MQTT服务器收到消息
    /// </summary>
    /// <param name="Context">数据</param>
    public MqttMessage(MqttApplicationMessageInterceptorContext Context)
        => this.Context = Context;
    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="Topic">标题</param>
    /// <param name="data">数据</param>
    public void Send(string Topic, string data, MqttQualityOfServiceLevel level = MqttQualityOfServiceLevel.ExactlyOnce)
        => MQTTServer.Send(Topic, data, level);
    /// <summary>
    /// 设置是否允许
    /// </summary>
    /// <param name="Publish">状态</param>
    public void SetPublish(bool Publish)
        => Context.AcceptPublish = Publish;
}

public class MqttSubscription
{
    public MqttSubscriptionInterceptorContext Context { get; init; }
    /// <summary>
    /// MQTT服务器订阅
    /// </summary>
    /// <param name="Context">数据</param>
    public MqttSubscription(MqttSubscriptionInterceptorContext Context)
       => this.Context = Context;
    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="Topic">标题</param>
    /// <param name="data">数据</param>
    public void Send(string Topic, string data, MqttQualityOfServiceLevel level = MqttQualityOfServiceLevel.ExactlyOnce)
        => MQTTServer.Send(Topic, data, level);
}

public class MqttUnsubscription
{
    public MqttUnsubscriptionInterceptorContext Context { get; init; }
    /// <summary>
    /// MQTT服务器订阅
    /// </summary>
    /// <param name="Context">数据</param>
    public MqttUnsubscription(MqttUnsubscriptionInterceptorContext Context)
       => this.Context = Context;
    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="Topic">标题</param>
    /// <param name="data">数据</param>
    public void Send(string Topic, string data, MqttQualityOfServiceLevel level = MqttQualityOfServiceLevel.ExactlyOnce)
        => MQTTServer.Send(Topic, data, level);
}
