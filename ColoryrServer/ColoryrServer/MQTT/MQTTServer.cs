using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColoryrServer.MQTT
{
    class MQTTServer
    {
        private static IMqttServer MqttServer;
        public static async void Start()
        {
            var optionsBuilder = new MqttServerOptionsBuilder()
                     .WithConnectionValidator(ConnectionValidator)
                     .WithSubscriptionInterceptor(SubscriptionInterceptor)
                     .WithApplicationMessageInterceptor(ApplicationMessageInterceptor)
                     .WithDefaultEndpointPort(ServerMain.Config.MQTTConfig.Port);

            MqttServer = new MqttFactory().CreateMqttServer();
            await MqttServer.StartAsync(optionsBuilder.Build());

        }

        private static void ConnectionValidator(MqttConnectionValidatorContext data)
        {
            
        }

        private static void ApplicationMessageInterceptor(MqttApplicationMessageInterceptorContext data)
        {
            
        }
        private static void SubscriptionInterceptor(MqttSubscriptionInterceptorContext data)
        { 
            
        }
    }
}
