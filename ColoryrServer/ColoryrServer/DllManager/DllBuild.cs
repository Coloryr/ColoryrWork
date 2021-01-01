using ColoryrServer.DllManager.StartGen.GenType;
using ColoryrServer.DllManager.StartGen.GenUtils;
using ColoryrServer.FileSystem;
using ColoryrServer.Http;
using Lib.Build;
using Lib.Build.Object;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ColoryrServer.DllManager
{
    internal class DllBuild
    {
        public static readonly Dictionary<string, string> Token = new Dictionary<string, string>();
        public static HttpReturn HttpBuild(BuildOBJ Json, UserConfig User)
        {
            object Object = null;
            if (Json.Mode == ReType.Login)
            {
                var password = BitConverter.ToString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(Json.Code))).Replace("-", "");
                if (User.Password.ToLower() == password.ToLower())
                {
                    Json.UUID = Guid.NewGuid().ToString().Replace("-", "");
                    if (Token.ContainsKey(Json.User))
                        Token[Json.User] = Json.UUID;
                    else
                        Token.Add(Json.User, Json.UUID);
                    Object = new ReMessage
                    {
                        Build = true,
                        Message = Json.UUID
                    };
                }
                else
                    Object = new ReMessage
                    {
                        Build = false,
                        Message = "密码错误"
                    };
            }
            else if (Token.ContainsKey(Json.User) && Token[Json.User] == Json.Token)
            {
                GenReOBJ BuildBack;
                Stopwatch SW;
                CSFileCode File;
                CSFileList List;
                switch (Json.Mode)
                {
                    case ReType.AddDll:
                        if (CSFile.GetDll(Json.UUID) == null)
                        {
                            CSFile.StorageDll(new()
                            {
                                UUID = Json.UUID,
                                Type = CodeType.Dll,
                                Version = 1
                            });
                            Object = new ReMessage
                            {
                                Build = true,
                                Message = "已创建" + Json.UUID
                            };
                        }
                        else
                        Object = new ReMessage
                        {
                            Build = false,
                            Message = "UUID已存在"
                        };
                        break;
                    case ReType.AddClass:
                        if (CSFile.GetClass(Json.UUID) == null)
                        {
                            CSFile.StorageClass(new()
                            {
                                UUID = Json.UUID,
                                Type = CodeType.Class,
                                Version = 1
                            });
                            Object = new ReMessage
                            {
                                Build = true,
                                Message = "已创建" + Json.UUID
                            };
                        }
                        else
                            Object = new ReMessage
                        {
                            Build = false,
                            Message = "UUID已存在"
                        };
                        break;
                    case ReType.AddIoT:
                        if (CSFile.GetIoT(Json.UUID) == null)
                        {
                            CSFile.StorageIoT(new ()
                            {
                                UUID = Json.UUID,
                                Type = CodeType.IoT,
                                Version = 1
                            });
                            Object = new ReMessage
                            {
                                Build = true,
                                Message = "已创建" + Json.UUID
                            };
                        }
                        else
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = "UUID已存在"
                            };
                        break;
                    case ReType.AddWebSocket:
                        if (CSFile.GetWebSocket(Json.UUID) == null)
                        {
                            CSFile.StorageWebSocket(new()
                            {
                                UUID = Json.UUID,
                                Type = CodeType.WebSocket,
                                Version = 1
                            });
                            Object = new ReMessage
                            {
                                Build = true,
                                Message = "已创建" + Json.UUID
                            };
                        }
                        else
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = "UUID已存在"
                            };
                        break;
                    case ReType.AddRobot:
                        if (CSFile.GetRobot(Json.UUID) == null)
                        {
                            CSFile.StorageRobot(new()
                            {
                                UUID = Json.UUID,
                                Type = CodeType.Robot,
                                Version = 1
                            });
                            Object = new ReMessage
                            {
                                Build = true,
                                Message = "已创建" + Json.UUID
                            };
                        }
                        else
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = "UUID已存在"
                            };
                        break;
                    case ReType.GetDll:
                        List = new CSFileList();
                        foreach (var item in CSFile.DllFileList)
                        {
                            List.List.Add(item.Key, item.Value);
                        }
                        Object = List;
                        break;
                    case ReType.GetClass:
                        List = new CSFileList();
                        foreach (var item in CSFile.ClassFileList)
                        {
                            List.List.Add(item.Key, item.Value);
                        }
                        Object = List;
                        break;
                    case ReType.GetIoT:
                        List = new CSFileList();
                        foreach (var item in CSFile.IoTFileList)
                        {
                            List.List.Add(item.Key, item.Value);
                        }
                        Object = List;
                        break;
                    case ReType.GetWebSocket:
                        List = new CSFileList();
                        foreach (var item in CSFile.WebSocketFileList)
                        {
                            List.List.Add(item.Key, item.Value);
                        }
                        Object = List;
                        break;
                    case ReType.GetRobot:
                        List = new CSFileList();
                        foreach (var item in CSFile.RobotFileList)
                        {
                            List.List.Add(item.Key, item.Value);
                        }
                        Object = List;
                        break;
                    case ReType.CodeDll:
                        Object = CSFile.GetDll(Json.UUID);
                        break;
                    case ReType.CodeClass:
                        Object = CSFile.GetClass(Json.UUID);
                        break;
                    case ReType.CodeIoT:
                        Object = CSFile.GetIoT(Json.UUID);
                        break;
                    case ReType.CodeWebSocket:
                        Object = CSFile.GetWebSocket(Json.UUID);
                        break;
                    case ReType.CodeRobot:
                        Object = CSFile.GetRobot(Json.UUID);
                        break;
                    case ReType.GetApi:
                        Object = APIFile.list;
                        break;
                    case ReType.RemoveDll:
                        CSFile.RemoveFile(CodeType.Dll, Json.UUID);
                        Object = new ReMessage
                        {
                            Build = true,
                            Message = "已删除"
                        };
                        break;
                    case ReType.RemoveClass:
                        CSFile.RemoveFile(CodeType.Class, Json.UUID);
                        Object = new ReMessage
                        {
                            Build = true,
                            Message = "已删除"
                        };
                        break;
                    case ReType.RemoveIoT:
                        CSFile.RemoveFile(CodeType.IoT, Json.UUID);
                        Object = new ReMessage
                        {
                            Build = true,
                            Message = "已删除"
                        };
                        break;
                    case ReType.RemoveWebSocket:
                        CSFile.RemoveFile(CodeType.WebSocket, Json.UUID);
                        Object = new ReMessage
                        {
                            Build = true,
                            Message = "已删除"
                        };
                        break;
                    case ReType.RemoveRobot:
                        CSFile.RemoveFile(CodeType.Robot, Json.UUID);
                        Object = new ReMessage
                        {
                            Build = true,
                            Message = "已删除"
                        };
                        break;
                    case ReType.UpdataDll:
                        File = CSFile.GetDll(Json.UUID);
                        if (File == null)
                        {
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = "没有这个UUID"
                            };
                            break;
                        }
                        if (File.Version != Json.Version)
                        {
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = "版本号错误"
                            };
                            break;
                        }
                        File.Version++;
                        var list = JsonConvert.DeserializeObject<List<CodeEditObj>>(Json.Code);

                        File.Code = FileEdit.StartEdit(File.Code, list);
                        File.Text = Json.Text;

                        SW = new Stopwatch();
                        SW.Start();
                        BuildBack = GenDll.StartGen(File);
                        SW.Stop();
                        Object = new ReMessage
                        {
                            Build = BuildBack.Isok,
                            Message = BuildBack.Res,
                            UseTime = SW.ElapsedMilliseconds.ToString()
                        };
                        break;
                    case ReType.UpdataClass:
                        File = CSFile.GetClass(Json.UUID);
                        if (File == null)
                        {
                            File = new CSFileCode
                            {
                                UUID = Json.UUID,
                                Text = Json.Text,
                                Code = Json.Code,
                                Type = CodeType.Class,
                                Version = 1
                            };
                        }
                        else
                        {
                            File.Code = Json.Code;
                            File.Text = Json.Text;
                            if (File.Version != Json.Version)
                            {
                                Object = new ReMessage
                                {
                                    Build = false,
                                    Message = "版本号错误"
                                };
                                break;
                            }
                            File.Version++;
                        }
                        SW = new Stopwatch();
                        SW.Start();
                        BuildBack = GenClass.StartGen(File);
                        SW.Stop();
                        Object = new ReMessage
                        {
                            Build = BuildBack.Isok,
                            Message = BuildBack.Res,
                            UseTime = SW.ElapsedMilliseconds.ToString()
                        };
                        break;
                }
            }
            else
            {
                Object = new ReMessage
                {
                    Build = false,
                    Message = "233",
                    UseTime = "0"
                };
            }
            return new HttpReturn
            {
                Data = Object
            };
        }
    }
}
