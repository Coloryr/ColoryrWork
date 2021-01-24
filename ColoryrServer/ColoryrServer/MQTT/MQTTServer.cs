using ColoryrServer.DllManager;
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
            ServerMain.LogOut("正在启动Mqtt");
            var optionsBuilder = new MqttServerOptionsBuilder()
                     .WithConnectionValidator(ConnectionValidator)
                     .WithSubscriptionInterceptor(SubscriptionInterceptor)
                     .WithApplicationMessageInterceptor(ApplicationMessageInterceptor)
                     .WithDefaultEndpointPort(ServerMain.Config.MQTTConfig.Port);

            MqttServer = new MqttFactory().CreateMqttServer();
            await MqttServer.StartAsync(optionsBuilder.Build());
            ServerMain.LogOut("已启动Mqtt");
        }

        private static void ConnectionValidator(MqttConnectionValidatorContext data)
        {
            Task.Run(() =>
            {
                DllRun.MqttGo(data);
            });
        }

        private static void ApplicationMessageInterceptor(MqttApplicationMessageInterceptorContext data)
        {
            Task.Run(() =>
            {
                DllRun.MqttGo(data);
            });
        }
        private static void SubscriptionInterceptor(MqttSubscriptionInterceptorContext data)
        {
            Task.Run(() =>
            {
                DllRun.MqttGo(data);
            });
        }

        public static async void Stop()
        {
            await MqttServer.StopAsync();
        }

        private static void PipeConnectionValidator(MqttConnectionValidatorContext data)
        {
            
        }

        private static void PipeApplicationMessageInterceptor(MqttApplicationMessageInterceptorContext data)
        {
            
        }
        private static void PipeSubscriptionInterceptor(MqttSubscriptionInterceptorContext data)
        {
            
        }

        public static async void StartPipe()
        {
            ServerMain.LogOut("正在启动Mqtt");
            var optionsBuilder = new MqttServerOptionsBuilder()
                    .WithConnectionValidator(PipeConnectionValidator)
                    .WithSubscriptionInterceptor(PipeSubscriptionInterceptor)
                    .WithApplicationMessageInterceptor(PipeApplicationMessageInterceptor)
                    .WithDefaultEndpointPort(ServerMain.Config.MQTTConfig.Port);

            MqttServer = new MqttFactory().CreateMqttServer();
            await MqttServer.StartAsync(optionsBuilder.Build());
            ServerMain.LogOut("已启动Mqtt");
        }
    }
}
