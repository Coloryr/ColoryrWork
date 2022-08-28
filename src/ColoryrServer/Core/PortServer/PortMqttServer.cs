using ColoryrServer.Core.DllManager;
using ColoryrServer.SDK;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ColoryrServer.Core.PortServer;

internal static class PortMqttServer
{
    private static MqttServer MqttServer;
    public static async void Start()
    {
        ServerMain.LogOut("Mqtt服务器正在启动");
        var mqttFactory = new MqttFactory();

        var optionsBuilder = new MqttServerOptionsBuilder();


        if (ServerMain.Config.MqttConfig.UseSsl)
        {
            try
            {
                optionsBuilder
                    .WithEncryptedEndpoint()
                    .WithEncryptedEndpointPort(ServerMain.Config.MqttConfig.Socket.Port)
                    .WithEncryptionSslProtocol(SslProtocols.Tls13)
                    .WithEncryptionCertificate(
                    new X509Certificate2(ServerMain.Config.MqttConfig.Ssl,
                            ServerMain.Config.MqttConfig.Password))
                    .WithEncryptedEndpointBoundIPAddress(
                    IPAddress.Parse(ServerMain.Config.MqttConfig.Socket.IP)); ;
                ServerMain.LogOut($"Mqtt服务器加载SSL证书{ServerMain.Config.MqttConfig.Ssl}");
            }
            catch (CryptographicException e)
            {
                ServerMain.LogError($"Mqtt服务器加载SSL证书{ServerMain.Config.MqttConfig.Ssl}错误");
                ServerMain.LogError(e);
            }
        }
        else
        {
            optionsBuilder
                .WithDefaultEndpoint()
                .WithDefaultEndpointPort(ServerMain.Config.MqttConfig.Socket.Port)
                .WithDefaultEndpointBoundIPAddress(
                IPAddress.Parse(ServerMain.Config.MqttConfig.Socket.IP));
        }
        ServerMain.LogOut($"Mqtt服务器监听{ServerMain.Config.MqttConfig.Socket.IP}:{ServerMain.Config.MqttConfig.Socket.Port}");
        MqttServer = mqttFactory.CreateMqttServer(optionsBuilder.Build());
        MqttServer.ValidatingConnectionAsync += ConnectionValidator;
        MqttServer.InterceptingSubscriptionAsync += InterceptingSubscription;
        MqttServer.InterceptingUnsubscriptionAsync += InterceptingUnsubscription;
        MqttServer.RetainedMessageChangedAsync += RetainedMessageChangedAsync;
        MqttServer.LoadingRetainedMessageAsync += LoadingRetainedMessageAsync;
        MqttServer.StartedAsync += MqttServer_StartedAsync;
        MqttServer.ClientConnectedAsync += MqttServer_ClientConnectedAsync;
        await MqttServer.StartAsync();
        ServerMain.OnStop += Stop;
        
    }

    private static Task MqttServer_ClientConnectedAsync(ClientConnectedEventArgs arg)
    => Task.Run(() =>
        {
            ServerMain.LogOut($"Mqtt有客户端链接：{arg.Endpoint} {arg.ClientId}");
        });

    private static Task MqttServer_StartedAsync(System.EventArgs arg)
    => Task.Run(() =>
        {
            ServerMain.LogOut("Mqtt服务器已启动");
        });
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
