//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Sockets.Kcp;
//using System.Net.Sockets.Kcp.Simple;
//using System.Text;
//using System.Threading.Tasks;

//namespace ColoryrTest.Run;

//internal class KcpDemo
//{
//    public static void Start()
//    {
//        var kcpClient = new SimpleKcpClient(1234);
//        Task.Run(async () =>
//        {
//            while (true)
//            {
//                kcpClient.kcp.Update(DateTime.UtcNow);
//                await Task.Delay(10);
//            }
//        });
//        StartRecv(kcpClient);

//        static async void StartRecv(SimpleKcpClient client)
//        {
//            var res = await client.ReceiveAsync();
//            StartRecv(client);

//            await Task.Delay(1);
//            var str = Encoding.UTF8.GetString(res);
//            if ("发送一条消息" == str)
//            {
//                Console.WriteLine(str);

//                var buffer = Encoding.UTF8.GetBytes("回复一条消息");
//                client.SendAsync(buffer, buffer.Length);
//            }

//        }
//    }
//}
