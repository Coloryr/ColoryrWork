using ColoryrServer.IoT;
using System.Collections.Generic;
using System.Text;

namespace ColoryrServer.SDK
{
    public class IoTRequest
    {
        public string Name { get; private set; }
        public byte[] Data { get; private set; }
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="Name">IoT设备注册的名字</param>
        /// <param name="Data">IoT发送的数据</param>
        public IoTRequest(string Name, byte[] Data)
        {
            this.Name = Name;
            this.Data = Data;
        }
    }

    public class IoT
    {
        /// <summary>
        /// 获取IoT设备列表
        /// </summary>
        /// <returns>设备列表</returns>
        public static List<string> GetIoTList()
        {
            return IoTSocketServer.GetList();
        }
        /// <summary>
        /// 向IoT设备发送字符串
        /// </summary>
        /// <param name="Name">设备名</param>
        /// <param name="Data">字符串</param>
        public static void Send(string Name, string Data)
        {
            IoTPackDo.SendPack(Name, Encoding.UTF8.GetBytes(Data));
        }
        /// <summary>
        /// 向IoT设备发送数据
        /// </summary>
        /// <param name="Name">设备名</param>
        /// <param name="Data">数据</param>
        public static void Send(string Name, byte[] Data)
        {
            IoTPackDo.SendPack(Name, Data);
        }
    }
}
