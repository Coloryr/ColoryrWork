using ColoryrServer.Core.Dll;
using ColoryrServer.SDK;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System;
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
                ServerMain.LogOut($"Mqtt服务器加载SSL证书[{ServerMain.Config.MqttConfig.Ssl}]");
            }
            catch (CryptographicException e)
            {
                ServerMain.LogError($"Mqtt服务器加载SSL证书[{ServerMain.Config.MqttConfig.Ssl}]错误", e);
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
        MqttServer = new MqttFactory().CreateMqttServer(optionsBuilder.Build());
        MqttServer.InterceptingPublishAsync += MqttServer_InterceptingPublishAsync;
        MqttServer.ValidatingConnectionAsync += ConnectionValidator;
        MqttServer.InterceptingSubscriptionAsync += InterceptingSubscription;
        MqttServer.InterceptingUnsubscriptionAsync += InterceptingUnsubscription;
        MqttServer.RetainedMessageChangedAsync += RetainedMessageChangedAsync;
        MqttServer.LoadingRetainedMessageAsync += LoadingRetainedMessageAsync;
        MqttServer.StartedAsync += MqttServer_StartedAsync;
        MqttServer.StoppedAsync += MqttServer_StoppedAsync;
        MqttServer.ApplicationMessageNotConsumedAsync += MqttServer_ApplicationMessageNotConsumedAsync;
        MqttServer.ClientConnectedAsync += MqttServer_ClientConnectedAsync;
        MqttServer.ClientDisconnectedAsync += MqttServer_ClientDisconnectedAsync;
        await MqttServer.StartAsync();
        ServerMain.OnStop += Stop;
    }

    private static Task MqttServer_StoppedAsync(EventArgs arg)
        => Task.Run(() => ServerMain.LogOut("Mqtt服务器已停止"));

    private static Task MqttServer_ApplicationMessageNotConsumedAsync(ApplicationMessageNotConsumedEventArgs arg)
        => Task.Run(() =>
        {
            if (ServerMain.Config.FixMode)
                return;
            DllRun.MqttGo(new DllMqttMessage(arg));
        });
    private static Task MqttServer_InterceptingPublishAsync(InterceptingPublishEventArgs arg)
        => Task.Run(()
             =>
        {
            if (ServerMain.Config.FixMode)
                return;
            DllRun.MqttGo(new DllMqttInterceptingPublish(arg));
        });
    private static Task MqttServer_ClientDisconnectedAsync(ClientDisconnectedEventArgs arg)
        => Task.Run(()
             =>
        {
            if (ServerMain.Config.FixMode)
                return;
            DllRun.MqttGo(new DllMqttClientDisconnected(arg));
        });
    private static Task MqttServer_ClientConnectedAsync(ClientConnectedEventArgs arg)
        => Task.Run(()
             =>
        {
            if (ServerMain.Config.FixMode)
                return;
            DllRun.MqttGo(new DllMqttClientConnected(arg));
        });
    private static Task LoadingRetainedMessageAsync(LoadingRetainedMessagesEventArgs arg)
        => Task.Run(()
             =>
        {
            if (ServerMain.Config.FixMode)
                return;
            DllRun.MqttGo(new DllMqttLoadingRetainedMessages(arg));
        });
    private static Task ConnectionValidator(ValidatingConnectionEventArgs arg)
        => Task.Run(()
             =>
        {
            if (ServerMain.Config.FixMode)
                return;
            DllRun.MqttGo(new DllMqttConnectionValidator(arg));
        });
    private static Task RetainedMessageChangedAsync(RetainedMessageChangedEventArgs arg)
        => Task.Run(()
             =>
        {
            if (ServerMain.Config.FixMode)
                return;
            DllRun.MqttGo(new DllMqttRetainedMessageChanged(arg));
        });
    private static Task InterceptingSubscription(InterceptingSubscriptionEventArgs arg)
        => Task.Run(()
             =>
        {
            if (ServerMain.Config.FixMode)
                return;
            DllRun.MqttGo(new DllMqttSubscription(arg));
        });
    public static Task InterceptingUnsubscription(InterceptingUnsubscriptionEventArgs arg)
        => Task.Run(()
            =>
        {
            if (ServerMain.Config.FixMode)
                return;
            DllRun.MqttGo(new DllMqttUnsubscription(arg));
        });

    private static Task MqttServer_StartedAsync(System.EventArgs arg)
        => Task.Run(() => ServerMain.LogOut("Mqtt服务器已启动"));
    private static async void Stop()
        => await MqttServer.StopAsync();

    public static Task Send(string topic, string data, string id, MqttQualityOfServiceLevel level)
    {
        return MqttServer.InjectApplicationMessage(
            new InjectedMqttApplicationMessage(new()
            {
                Topic = topic,
                Payload = Encoding.UTF8.GetBytes(data),
                QualityOfServiceLevel = level
            })
            {
                SenderClientId = id
            });
    }
}
