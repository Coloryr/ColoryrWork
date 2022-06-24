using ColoryrServer.Core.DllManager;
using ColoryrServer.SDK;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ColoryrServer.Core.MQTT;

internal static class MQTTServer
{
    private static MqttServer MqttServer;
    public static async void Start()
    {
        ServerMain.LogOut("Mqtt服务器正在启动");
        var optionsBuilder = new MqttServerOptionsBuilder()
            .WithDefaultEndpointBoundIPAddress(IPAddress.Parse(ServerMain.Config.MqttConfig.Socket.IP))
            .WithDefaultEndpointPort(ServerMain.Config.MqttConfig.Socket.Port);
        if (ServerMain.Config.MqttConfig.UseSsl)
        {
            optionsBuilder = optionsBuilder.WithEncryptionSslProtocol(SslProtocols.Tls13)
                .WithEncryptionCertificate(
                new X509Certificate2(ServerMain.Config.MqttConfig.Ssl,
                        ServerMain.Config.MqttConfig.Password));
        }
        ServerMain.LogOut($"Mqtt服务器监听{ServerMain.Config.MqttConfig.Socket.IP}:{ServerMain.Config.MqttConfig.Socket.Port}");
        MqttServer = new MqttFactory().CreateMqttServer(optionsBuilder.Build());
        MqttServer.ValidatingConnectionAsync += ConnectionValidator;
        MqttServer.InterceptingSubscriptionAsync += InterceptingSubscription;
        MqttServer.InterceptingUnsubscriptionAsync += InterceptingUnsubscription;
        MqttServer.RetainedMessageChangedAsync += RetainedMessageChangedAsync;
        MqttServer.LoadingRetainedMessageAsync += LoadingRetainedMessageAsync;
        await MqttServer.StartAsync();
        ServerMain.OnStop += Stop;
        ServerMain.LogOut("Mqtt服务器已启动");
    }

    public static async void Send(string Topic, string data, MqttQualityOfServiceLevel level)
    {
        await MqttServer.InjectApplicationMessage(
            new InjectedMqttApplicationMessage(new()
            {
                Topic = Topic,
                Payload = Encoding.UTF8.GetBytes(data),
                QualityOfServiceLevel = level
            }));
    }

    private static Task LoadingRetainedMessageAsync(LoadingRetainedMessagesEventArgs data)
    => Task.Run(()
         => DllRun.MqttGo(new DllMqttLoadingRetainedMessages(data)));
    private static Task ConnectionValidator(ValidatingConnectionEventArgs data)
        => Task.Run(()
             => DllRun.MqttGo(new DllMqttConnectionValidator(data)));
    private static Task RetainedMessageChangedAsync(RetainedMessageChangedEventArgs data)
        => Task.Run(()
             => DllRun.MqttGo(new DllMqttMessage(data)));
    private static Task InterceptingSubscription(InterceptingSubscriptionEventArgs data)
        => Task.Run(()
             => DllRun.MqttGo(new DllMqttSubscription(data)));
    public static Task InterceptingUnsubscription(InterceptingUnsubscriptionEventArgs context)
        => Task.Run(()
            => DllRun.MqttGo(new DllMqttUnsubscription(context)));
    private static async void Stop()
        => await MqttServer.StopAsync();
}
