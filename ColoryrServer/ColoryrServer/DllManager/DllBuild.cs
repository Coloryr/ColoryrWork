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
    class DllBuild
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
                        Message = Json.UUID,
                        UseTime = User.Admin ? "1" : "0"
                    };
                }
                else
                {
                    Object = new ReMessage
                    {
                        Build = false,
                        Message = "密码错误",
                        UseTime = "0"
                    };
                }
            }
            else if (Token.ContainsKey(Json.User) && Token[Json.User] == Json.Token)
            {
                GenReOBJ BuildBack = null;
                Stopwatch SW;
                CSFileCode File;
                switch (Json.Mode)
                {
                    case ReType.AddDll:
                        File = CSFile.GetDll(Json.UUID);
                        if (File == null)
                        {
                            File = new CSFileCode
                            {
                                UUID = Json.UUID,
                                Type = CodeType.Dll,
                                Version = 1
                            };
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = "已创建" + Json.UUID
                            };
                        }
                        else
                        {
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = "UUID已存在"
                            };
                        }
                        break;
                    case ReType.AddClass:
                        if (!User.Admin)
                        {
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = "错误请求"
                            };
                            break;
                        }
                        File = CSFile.GetClass(Json.UUID);
                        if (File == null)
                        {
                            File = new CSFileCode
                            {
                                UUID = Json.UUID,
                                Type = CodeType.Class,
                                Version = 1
                            };
                        }
                        else
                        {
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = "UUID已存在"
                            };
                        }
                        break;
                    case ReType.AddIoT:
                        if (!User.Admin)
                        {
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = "错误请求"
                            };
                            break;
                        }
                        File = CSFile.GetIoT(Json.UUID);
                        if (File == null)
                        {
                            File = new CSFileCode
                            {
                                UUID = Json.UUID,
                                Type = CodeType.IoT,
                                Version = 1
                            };
                        }
                        else
                        {
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = "UUID已存在"
                            };
                        }
                        break;
                    case ReType.AddWebSocket:
                        if (!User.Admin)
                        {
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = "错误请求"
                            };
                            break;
                        }
                        File = CSFile.GetWebSocket(Json.UUID);
                        if (File == null)
                        {
                            File = new CSFileCode
                            {
                                UUID = Json.UUID,
                                Type = CodeType.WebSocket,
                                Version = 1
                            };
                        }
                        else
                        {
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = "UUID已存在"
                            };
                            break;
                        }
                        break;
                    case ReType.AddRobot:
                        if (!User.Admin)
                        {
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = "错误请求"
                            };
                            break;
                        }
                        File = CSFile.GetRobot(Json.UUID);
                        if (File == null)
                        {
                            File = new CSFileCode
                            {
                                UUID = Json.UUID,
                                Text = Json.Text,
                                Code = Json.Code,
                                Type = CodeType.Robot,
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
                        BuildBack = GenRobot.StartGen(File);
                        SW.Stop();
                        Object = new ReMessage
                        {
                            Build = BuildBack.Isok,
                            Message = BuildBack.Res,
                            UseTime = SW.ElapsedMilliseconds.ToString()
                        };
                        break;
                    case ReType.CheckLogin:
                        Object = new ReMessage
                        {
                            Build = true,
                            UseTime = User.Admin ? "1" : "0"
                        };
                        break;
                    case ReType.GetDll:
                        if (!User.Admin)
                        {
                            var DllList = CSFile.DllFileList.Where(a => a.Value.User == User.Username);
                            var ReList = new Dictionary<string, CSFileObj>();
                            foreach (var Item in DllList)
                            {
                                ReList.Add(Item.Key, Item.Value);
                            }
                            Object = new CSFileList
                            {
                                list = ReList
                            };
                        }
                        else
                        {
                            var temp = new CSFileList();
                            foreach (var item in CSFile.DllFileList)
                            {
                                temp.list.Add(item.Key, item.Value);
                            }
                            Object = temp;
                        }
                        break;
                    case ReType.GetClass:
                        if (User.Admin)
                        {
                            var temp = new CSFileList();
                            foreach (var item in CSFile.ClassFileList)
                            {
                                temp.list.Add(item.Key, item.Value);
                            }
                            Object = temp;
                        }
                        else
                            Object = new CSFileList
                            {
                                list = new Dictionary<string, CSFileObj>()
                            };
                        break;
                    case ReType.GetIoT:
                        if (User.Admin)
                        {
                            var temp = new CSFileList();
                            foreach (var item in CSFile.IoTFileList)
                            {
                                temp.list.Add(item.Key, item.Value);
                            }
                            Object = temp;
                        }
                        else
                            Object = new CSFileList
                            {
                                list = new Dictionary<string, CSFileObj>()
                            };
                        break;
                    case ReType.GetWebSocket:
                        if (User.Admin)
                        {
                            var temp = new CSFileList();
                            foreach (var item in CSFile.WebSocketFileList)
                            {
                                temp.list.Add(item.Key, item.Value);
                            }
                            Object = temp;
                        }
                        else
                            Object = new CSFileList
                            {
                                list = new Dictionary<string, CSFileObj>()
                            };
                        break;
                    case ReType.GetRobot:
                        if (User.Admin)
                        {
                            var temp = new CSFileList();
                            foreach (var item in CSFile.RobotFileList)
                            {
                                temp.list.Add(item.Key, item.Value);
                            }
                            Object = temp;
                        }
                        else
                            Object = new CSFileList
                            {
                                list = new Dictionary<string, CSFileObj>()
                            };
                        break;
                    case ReType.CodeDll:
                        if (!User.Admin && CSFile.CheckPermission(Json.UUID, User.Username))
                        {
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = "错误请求"
                            };
                            break;
                        }
                        else
                            Object = CSFile.GetDll(Json.UUID);
                        break;
                    case ReType.CodeClass:
                        if (!User.Admin)
                        {
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = "错误请求"
                            };
                            break;
                        }
                        Object = CSFile.GetClass(Json.UUID);
                        break;
                    case ReType.CodeIoT:
                        if (!User.Admin)
                        {
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = "错误请求"
                            };
                            break;
                        }
                        Object = CSFile.GetIoT(Json.UUID);
                        break;
                    case ReType.CodeWebSocket:
                        if (!User.Admin)
                        {
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = "错误请求"
                            };
                            break;
                        }
                        Object = CSFile.GetWebSocket(Json.UUID);
                        break;
                    case ReType.CodeRobot:
                        if (!User.Admin)
                        {
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = "错误请求"
                            };
                            break;
                        }
                        Object = CSFile.GetRobot(Json.UUID);
                        break;
                    case ReType.GetApi:
                        Object = APIFile.list;
                        break;
                    case ReType.RemoveDll:
                        if (!User.Admin && CSFile.CheckPermission(Json.UUID, User.Username))
                        {
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = "错误请求"
                            };
                            break;
                        }
                        CSFile.RemoveFile(CodeType.Dll, Json.UUID);
                        Object = new ReMessage
                        {
                            Build = true,
                            Message = "已删除"
                        };
                        break;
                    case ReType.RemoveClass:
                        if (!User.Admin)
                        {
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = "错误请求"
                            };
                            break;
                        }
                        CSFile.RemoveFile(CodeType.Class, Json.UUID);
                        Object = new ReMessage
                        {
                            Build = true,
                            Message = "已删除"
                        };
                        break;
                    case ReType.RemoveIoT:
                        if (!User.Admin)
                        {
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = "错误请求"
                            };
                            break;
                        }
                        CSFile.RemoveFile(CodeType.IoT, Json.UUID);
                        Object = new ReMessage
                        {
                            Build = true,
                            Message = "已删除"
                        };
                        break;
                    case ReType.RemoveWebSocket:
                        if (!User.Admin)
                        {
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = "错误请求"
                            };
                            break;
                        }
                        CSFile.RemoveFile(CodeType.WebSocket, Json.UUID);
                        Object = new ReMessage
                        {
                            Build = true,
                            Message = "已删除"
                        };
                        break;
                    case ReType.RemoveRobot:
                        if (!User.Admin)
                        {
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = "错误请求"
                            };
                            break;
                        }
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
                        //不是他的DLL
                        else if (!User.Admin && File.User != User.Username)
                        {
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = "错误请求"
                            };
                            break;
                        }
                        else
                        {
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

                        var list = JsonConvert.DeserializeObject<List<CodeEditObj>>(Json.Code);

                        File.Code = FileEdit.StartEdit(File.Code, list);
                        File.Text = Json.Text;

                        SW = new Stopwatch();
                        SW.Start();
                        BuildBack = GenDll.StartGen(User, File);
                        SW.Stop();
                        Object = new ReMessage
                        {
                            Build = BuildBack.Isok,
                            Message = BuildBack.Res,
                            UseTime = SW.ElapsedMilliseconds.ToString()
                        };
                        break;
                    case ReType.UpdataClass:
                        if (!User.Admin)
                        {
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = "错误请求"
                            };
                            break;
                        }
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
