using ColoryrServer.DllManager;
using ColoryrServer.SDK;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using MQTTnet.Server;
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

        public static async void Send(string Topic, string data)
        {
            await MqttServer.PublishAsync(new MqttApplicationMessage()
            {
                Topic = Topic,
                Payload = Encoding.UTF8.GetBytes(data),
                QualityOfServiceLevel = MqttQualityOfServiceLevel.ExactlyOnce
            });
        }

        private static void ConnectionValidator(MqttConnectionValidatorContext data)
        => Task.Run(()
             => DllRun.MqttGo(new MqttConnectionValidator(data)));


        private static void ApplicationMessageInterceptor(MqttApplicationMessageInterceptorContext data)
            => Task.Run(()
                 => DllRun.MqttGo(new MqttMessage(data)));
        private static void SubscriptionInterceptor(MqttSubscriptionInterceptorContext data)
            => Task.Run(()
                 => DllRun.MqttGo(new MqttSubscription(data)));
        public static async void Stop()
          => await MqttServer.StopAsync();

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
