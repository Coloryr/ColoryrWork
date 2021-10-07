using ColoryrServer.IoT;
using System.Collections.Generic;
using System.Text;

namespace ColoryrServer.SDK
{
    public class TcpIoTRequest
    {
        private int Server { get; init; }
        public int Port { get; init; }
        public byte[] Data { get; init; }
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="Port">IoT端口</param>
        /// <param name="Data">IoT发送的数据</param>
        public TcpIoTRequest(int Port, byte[] Data, int Server)
        {
            this.Port = Port;
            this.Data = Data;
            this.Server = Server;
        }
        /// <summary>
        /// 向IoT设备发送字符串
        /// </summary>
        /// <param name="Name">设备名</param>
        /// <param name="Data">字符串</param>
        public void Send(int Port, string Data)
            => IoTPackDo.SendTcpPack(Port, Encoding.UTF8.GetBytes(Data), Server);
        /// <summary>
        /// 向IoT设备发送数据
        /// </summary>
        /// <param name="Name">设备名</param>
        /// <param name="Data">数据</param>
        public void Send(int Port, byte[] Data)
           => IoTPackDo.SendTcpPack(Port, Data, Server);
    }
    public class UdpIoTRequest
    {
        private int Server { get; init; }
        public int Port { get; init; }
        public byte[] Data { get; init; }
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="Port">IoT端口</param>
        /// <param name="Data">IoT发送的数据</param>
        public UdpIoTRequest(int Port, byte[] Data, int Server)
        {
            this.Port = Port;
            this.Data = Data;
            this.Server = Server;
        }
        /// <summary>
        /// 向IoT设备发送字符串
        /// </summary>
        /// <param name="Name">设备名</param>
        /// <param name="Data">字符串</param>
        public void Send(int Port, string Data)
            => IoTPackDo.SendUdpPack(Port, Encoding.UTF8.GetBytes(Data), Server);
        /// <summary>
        /// 向IoT设备发送数据
        /// </summary>
        /// <param name="Name">设备名</param>
        /// <param name="Data">数据</param>
        public void Send(int Port, byte[] Data)
           => IoTPackDo.SendUdpPack(Port, Data, Server);
    }

    public class TcpIoT
    {
        /// <summary>
        /// 获取IoT设备列表
        /// </summary>
        /// <returns>设备列表</returns>
        public static List<int> GetList()
            => IoTSocketServer.GetTcpList();
    }

    public class UdpIoT
    {
        /// <summary>
        /// 获取IoT设备列表
        /// </summary>
        /// <returns>设备列表</returns>
        public static List<int> GetList()
            => IoTSocketServer.GetUdpList();
    }
}
