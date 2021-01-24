using ColoryrServer.MQTT;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColoryrServer.SDK
{
    public class ServerMqttConnectionValidator
    {
        public MqttConnectionValidatorContext Context { get; init; }
        public ServerMqttConnectionValidator(MqttConnectionValidatorContext Context)
        {
            this.Context = Context;
        }
        public void Send(string Topic, string data)
        {
            MQTTServer.Send(Topic, data);
        }
        public void SetReasonCode(MqttConnectReasonCode state)
        {
            Context.ReasonCode = state;
        }
    }

    public class ServerMqttMessage
    {
        public MqttApplicationMessageInterceptorContext Context { get; init; }
        public ServerMqttMessage(MqttApplicationMessageInterceptorContext Context)
        {
            this.Context = Context;
        }
        public void Send(string Topic, string data)
        {
            MQTTServer.Send(Topic, data);
        }
        public void SetPublish(bool Publish)
        {
            Context.AcceptPublish = Publish;
        }
    }

    public class ServerMqttSubscription
    {
        public MqttSubscriptionInterceptorContext Context { get; init; }
        public ServerMqttSubscription(MqttSubscriptionInterceptorContext Context)
        {
            this.Context = Context;
        }
        public void Send(string Topic, string data)
        {
            MQTTServer.Send(Topic, data);
        }
    }
}
