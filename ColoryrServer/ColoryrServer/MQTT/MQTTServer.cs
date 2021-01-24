using ColoryrServer.DllManager;
using ColoryrServer.SDK;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Formatter;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer.MQTT
{
    class MQTTServer
    {
        private static IMqttServer MqttServer;
        private static IMqttClient MqttClient;
        private static string UUID;
        public static async void Start()
        {
            ServerMain.LogOut("正在启动Mqtt");
            UUID = new Guid().ToString();
            var optionsBuilder = new MqttServerOptionsBuilder()
                     .WithConnectionValidator(ConnectionValidator)
                     .WithSubscriptionInterceptor(SubscriptionInterceptor)
                     .WithApplicationMessageInterceptor(ApplicationMessageInterceptor)
                     .WithDefaultEndpointPort(ServerMain.Config.MQTTConfig.Port);

            MqttServer = new MqttFactory().CreateMqttServer();
            MqttClient = new MqttFactory().CreateMqttClient();
            await MqttServer.StartAsync(optionsBuilder.Build());
            var options = new MqttClientOptionsBuilder()
                .WithClientId("ColoryrServer")
                .WithTcpServer("127.0.0.1", ServerMain.Config.MQTTConfig.Port)
                .WithCredentials(UUID, "")
                .WithCleanSession()
                .Build();
            await MqttClient.UseDisconnectedHandler(DisConnect).ConnectAsync(options, CancellationToken.None);
            //await MqttClient.SubscribeAsync(new MqttTopicFilterBuilder().Build());
            ServerMain.LogOut("已启动Mqtt");
        }

        private static async void DisConnect(MqttClientDisconnectedEventArgs arg)
        {
            try
            {
                var options = new MqttClientOptionsBuilder()
                .WithClientId("ColoryrServer")
                .WithTcpServer("127.0.0.1", ServerMain.Config.MQTTConfig.Port)
                .WithCredentials(UUID, "")
                .WithCleanSession()
                .Build();
                await MqttClient.ConnectAsync(options, CancellationToken.None);
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        }

        public static async void Send(string Topic, string data)
        {
            if (MqttClient.IsConnected)
            {
                await MqttClient.PublishAsync(Topic, data);
            }
        }

        private static void ConnectionValidator(MqttConnectionValidatorContext data)
        {
            if (data.Username == UUID)
            {
                data.ReasonCode = MqttConnectReasonCode.Success;
                return;
            }
            Task.Run(() =>
            {
                DllRun.MqttGo(new ServerMqttConnectionValidator(data));
            });
        }

        private static void ApplicationMessageInterceptor(MqttApplicationMessageInterceptorContext data)
        {
            Task.Run(() =>
            {
                DllRun.MqttGo(new ServerMqttMessage(data));
            });
        }
        private static void SubscriptionInterceptor(MqttSubscriptionInterceptorContext data)
        {
            Task.Run(() =>
            {
                DllRun.MqttGo(new ServerMqttSubscription(data));
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
