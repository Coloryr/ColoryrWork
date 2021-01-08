using ColoryrServer.IoT;
using System.Collections.Generic;
using System.Text;

namespace ColoryrServer.SDK
{
    public class TcpIoTRequest
    {
        public int Port { get; private set; }
        public byte[] Data { get; private set; }
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="Port">IoT端口</param>
        /// <param name="Data">IoT发送的数据</param>
        public TcpIoTRequest(int Port, byte[] Data)
        {
            this.Port = Port;
            this.Data = Data;
        }
    }
    public class UdpIoTRequest
    {
        public int Port { get; private set; }
        public byte[] Data { get; private set; }
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="Port">IoT端口</param>
        /// <param name="Data">IoT发送的数据</param>
        public UdpIoTRequest(int Port, byte[] Data)
        {
            this.Port = Port;
            this.Data = Data;
        }
    }

    public class TcpIoT
    {
        /// <summary>
        /// 获取IoT设备列表
        /// </summary>
        /// <returns>设备列表</returns>
        public static List<int> GetList()
        {
            return IoTSocketServer.GetTcpList();
        }
        /// <summary>
        /// 向IoT设备发送字符串
        /// </summary>
        /// <param name="Name">设备名</param>
        /// <param name="Data">字符串</param>
        public static void Send(int Port, string Data)
        {
            IoTPackDo.SendTcpPack(Port, Encoding.UTF8.GetBytes(Data));
        }
        /// <summary>
        /// 向IoT设备发送数据
        /// </summary>
        /// <param name="Name">设备名</param>
        /// <param name="Data">数据</param>
        public static void Send(int Port, byte[] Data)
        {
            IoTPackDo.SendTcpPack(Port, Data);
        }
    }

    public class UdpIoT
    {
        /// <summary>
        /// 获取IoT设备列表
        /// </summary>
        /// <returns>设备列表</returns>
        public static List<int> GetList()
        {
            return IoTSocketServer.GetUdpList();
        }
        /// <summary>
        /// 向IoT设备发送字符串
        /// </summary>
        /// <param name="Name">设备名</param>
        /// <param name="Data">字符串</param>
        public static void Send(int Port, string Data)
        {
            IoTPackDo.SendUdpPack(Port, Encoding.UTF8.GetBytes(Data));
        }
        /// <summary>
        /// 向IoT设备发送数据
        /// </summary>
        /// <param name="Name">设备名</param>
        /// <param name="Data">数据</param>
        public static void Send(int Port, byte[] Data)
        {
            IoTPackDo.SendUdpPack(Port, Data);
        }
    }
}
