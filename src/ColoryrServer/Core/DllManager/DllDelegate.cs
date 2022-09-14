using ColoryrServer.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColoryrServer.Core.DllManager;

internal static partial class DllRun
{
    public delegate dynamic DllIN(HttpDllRequest arg);
    public delegate bool SocketTcpIn(SocketTcpRequest arg);
    public delegate bool SocketUdpIn(SocketUdpRequest arg);
    public delegate bool WebSocketMessageIn(WebSocketMessage arg);
    public delegate bool WebSocketOpenIn(WebSocketOpen arg);
    public delegate bool WebSocketCloseIn(WebSocketClose arg);
    public delegate bool RobotMessageIn(RobotMessage arg);
    public delegate bool RobotEventIn(RobotEvent arg);
    public delegate bool RobotSendIn(RobotSend arg);
    public delegate bool MQTTMessageLoadingIn(DllMqttLoadingRetainedMessages arg);
    public delegate bool MQTTValidatorIn(DllMqttConnectionValidator arg);
    public delegate bool MQTTUnsubscriptionIn(DllMqttUnsubscription arg);
    public delegate bool MQTTMessageIn(DllMqttMessage arg);
    public delegate bool MQTTSubscriptionIn(DllMqttSubscription arg);
    public delegate bool MQTTClientConnectedIn(DllMqttClientConnected arg);
    public delegate bool MQTTClientDisconnectedIn(DllMqttClientDisconnected arg);
    public delegate bool MQTTInterceptingPublishIn(DllMqttInterceptingPublish arg);
    public delegate bool MQTTRetainedMessageChangedIn(DllMqttRetainedMessageChanged arg);
    public delegate bool ServiceErrorIn(Exception arg);
    public delegate bool ServicePerBuildIn(PerBuildArg arg);
    public delegate bool ServicePostBuildIn(PostBuildArg arg);
}
