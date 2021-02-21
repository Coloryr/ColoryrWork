using ColoryrServer.SDK;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColoryrServer.FileSystem
{
    internal class NoteFile
    {
        private static readonly string DllFileLocal = ServerMain.RunLocal + @"Notes\Dll\";
        private static readonly string ClassFileLocal = ServerMain.RunLocal + @"Notes\Class\";
        private static readonly string IoTFileLocal = ServerMain.RunLocal + @"Notes\IoT\";
        private static readonly string WebSocketFileLocal = ServerMain.RunLocal + @"\Notes\WebScoket\";
        private static readonly string RobotFileLocal = ServerMain.RunLocal + @"Notes\Robot\";
        private static readonly string MqttFileLocal = ServerMain.RunLocal + @"Notes\Mqtt\";
        private static readonly string AppFileLocal = ServerMain.RunLocal + @"Notes\App\";

        //public static readonly ConcurrentDictionary<string, NotesSDK> DllFileList = new();
        //public static readonly ConcurrentDictionary<string, NotesSDK> ClassFileList = new();
        //public static readonly ConcurrentDictionary<string, NotesSDK> IoTFileList = new();
        //public static readonly ConcurrentDictionary<string, NotesSDK> WebSocketFileList = new();
        //public static readonly ConcurrentDictionary<string, NotesSDK> RobotFileList = new();
        //public static readonly ConcurrentDictionary<string, NotesSDK> MqttFileList = new();
        //public static readonly ConcurrentDictionary<string, NotesSDK> AppFileList = new();

        public static void Start()
        {
            if (!Directory.Exists(DllFileLocal))
            {
                Directory.CreateDirectory(DllFileLocal);
            }
            if (!Directory.Exists(ClassFileLocal))
            {
                Directory.CreateDirectory(ClassFileLocal);
            }
            if (!Directory.Exists(IoTFileLocal))
            {
                Directory.CreateDirectory(IoTFileLocal);
            }
            if (!Directory.Exists(WebSocketFileLocal))
            {
                Directory.CreateDirectory(WebSocketFileLocal);
            }
            if (!Directory.Exists(RobotFileLocal))
            {
                Directory.CreateDirectory(RobotFileLocal);
            }
            if (!Directory.Exists(AppFileLocal))
            {
                Directory.CreateDirectory(AppFileLocal);
            }
            if (!Directory.Exists(MqttFileLocal))
            {
                Directory.CreateDirectory(MqttFileLocal);
            }
        }
        private static void Storage(string Local, object obj)
        {
            Task.Run(() =>
            {
                try
                {
                    File.WriteAllText(Local, JsonConvert.SerializeObject(obj, Formatting.Indented));
                }
                catch (Exception e)
                {
                    ServerMain.LogError(e);
                }
            });
        }

        public static void StorageDll(string UUID, NotesSDK obj)
        {
            var url = DllFileLocal + UUID + ".json";
            Storage(url, obj);
        }
        public static void StorageClass(string UUID, NotesSDK obj)
        {
            var url = ClassFileLocal + UUID + ".json";
            Storage(url, obj);
        }
        public static void StorageIoT(string UUID, NotesSDK obj)
        {
            var url = IoTFileLocal + UUID + ".json";
            Storage(url, obj);
        }
        public static void StorageWebSocket(string UUID, NotesSDK obj)
        {
            var url = WebSocketFileLocal + UUID + ".json";
            Storage(url, obj);
        }
        public static void StorageRobot(string UUID, NotesSDK obj)
        {
            var url = RobotFileLocal + UUID + ".json";
            Storage(url, obj);
        }
        public static void StorageMqtt(string UUID, NotesSDK obj)
        {
            var url = MqttFileLocal + UUID + ".json";
            Storage(url, obj);
        }
    }
}
