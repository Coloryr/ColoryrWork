using ColoryrServer.MQTT;
using MQTTnet.Protocol;
using MQTTnet.Server;

namespace ColoryrServer.SDK
{
    public class ServerMqttConnectionValidator
    {
        public MqttConnectionValidatorContext Context { get; init; }
        /// <summary>
        /// MQTT服务器验证
        /// </summary>
        /// <param name="Context">数据</param>
        public ServerMqttConnectionValidator(MqttConnectionValidatorContext Context)
            => this.Context = Context;
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="Topic">标题</param>
        /// <param name="data">数据</param>
        public void Send(string Topic, string data)
            => MQTTServer.Send(Topic, data);
        /// <summary>
        /// 设置验证后的返回
        /// </summary>
        /// <param name="state">状态</param>
        public void SetReasonCode(MqttConnectReasonCode state)
            => Context.ReasonCode = state;
    }

    public class ServerMqttMessage
    {
        public MqttApplicationMessageInterceptorContext Context { get; init; }
        /// <summary>
        /// MQTT服务器收到消息
        /// </summary>
        /// <param name="Context">数据</param>
        public ServerMqttMessage(MqttApplicationMessageInterceptorContext Context)
            => this.Context = Context;
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="Topic">标题</param>
        /// <param name="data">数据</param>
        public void Send(string Topic, string data)
            => MQTTServer.Send(Topic, data);
        /// <summary>
        /// 设置是否允许
        /// </summary>
        /// <param name="Publish">状态</param>
        public void SetPublish(bool Publish)
            => Context.AcceptPublish = Publish;
    }

    public class ServerMqttSubscription
    {
        public MqttSubscriptionInterceptorContext Context { get; init; }
        /// <summary>
        /// MQTT服务器订阅
        /// </summary>
        /// <param name="Context">数据</param>
        public ServerMqttSubscription(MqttSubscriptionInterceptorContext Context)
           => this.Context = Context;
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="Topic">标题</param>
        /// <param name="data">数据</param>
        public void Send(string Topic, string data)
            => MQTTServer.Send(Topic, data);
    }
}
