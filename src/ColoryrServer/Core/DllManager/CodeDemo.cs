namespace ColoryrServer.Core.DllManager;

internal static class CodeDemo
{
    public const string DllMain = "Main";
    public const string SocketTcp = "OnTcpMessage";
    public const string SocketUdp = "OnUdpMessage";
    public const string WebSocketMessage = "OnMessage";
    public const string WebSocketOpen = "OnOpen";
    public const string WebSocketClose = "OnClose";
    public const string RobotMessage = "OnMessage";
    public const string RobotEvent = "OnRobotEvent";
    public const string RobotSend = "OnMessagSend";
    public const string MQTTMessageLoading = "OnMessageLoading";
    public const string MQTTMessage = "OnMessage";
    public const string MQTTValidator = "OnValidator";
    public const string MQTTSubscription = "OnSubscription";
    public const string MQTTUnsubscription = "OnUnsubscription";
    public const string MQTTClientConnected = "OnClientConnected";
    public const string MQTTClientDisconnected = "OnClientDisconnected";
    public const string MQTTInterceptingPublish = "OnInterceptingPublish";
    public const string MQTTRetainedMessageChanged = "OnRetainedMessageChanged";
    public const string ServiceRun = "Run";
    public const string ServiceStart = "OnStart";
    public const string ServiceStop = "OnStop";
    public const string ServiceError = "OnError";
    public const string ServicePerBuild = "OnPerBuild";
    public const string ServicePostBuild = "OnPostBuild";

    public const string Name = "{name}";
}
