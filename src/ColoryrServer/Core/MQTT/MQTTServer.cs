using ColoryrServer.Core.DllManager;
using ColoryrServer.SDK;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System.Text;
using System.Threading.Tasks;

namespace ColoryrServer.Core.MQTT;

internal static class MQTTServer
{
    private static IMqttServer MqttServer;
    private static MQTTC mqttc = new();
    public static async void Start()
    {
        ServerMain.LogOut("Mqtt服务器正在启动");
        var optionsBuilder = new MqttServerOptionsBuilder()
                 .WithConnectionValidator(ConnectionValidator)
                 .WithSubscriptionInterceptor(SubscriptionInterceptor)
                 .WithApplicationMessageInterceptor(ApplicationMessageInterceptor)
                 .WithUnsubscriptionInterceptor(mqttc)
                 .WithDefaultEndpointPort(ServerMain.Config.MqttConfig.Port);
        ServerMain.LogOut($"Mqtt服务器监听{ServerMain.Config.MqttConfig.Port}");
        MqttServer = new MqttFactory().CreateMqttServer();
        await MqttServer.StartAsync(optionsBuilder.Build());
        ServerMain.OnStop += Stop;
        ServerMain.LogOut("Mqtt服务器已启动");
    }

    public static async void Send(string Topic, string data, MqttQualityOfServiceLevel level)
    {
        await MqttServer.PublishAsync(new MqttApplicationMessage()
        {
            Topic = Topic,
            Payload = Encoding.UTF8.GetBytes(data),
            QualityOfServiceLevel = level
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
    private static async void Stop()
      => await MqttServer.StopAsync();

    public class MQTTC : IMqttServerUnsubscriptionInterceptor
    {
        public Task InterceptUnsubscriptionAsync(MqttUnsubscriptionInterceptorContext context)
            => Task.Run(()
                 => DllRun.MqttGo(new MqttUnsubscription(context)));

    }
}
