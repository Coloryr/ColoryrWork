using ColoryrServer.DllManager.StartGen.GenType;
using ColoryrServer.DllManager.StartGen.GenUtils;
using ColoryrServer.FileSystem;
using ColoryrServer.Http;
using ColoryrServer.Utils;
using Lib.Build;
using Lib.Build.Object;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ColoryrServer.DllManager
{
    internal class DllBuild
    {
        public static readonly Dictionary<string, string> Token = new();
        public static HttpReturn StartBuild(BuildOBJ Json, UserConfig User)
        {
            object Object = null;
            if (Json.Mode == ReType.Login)
            {
                if (Json.Code == null)
                {
                    Object = new ReMessage
                    {
                        Build = false,
                        Message = "密码错误"
                    };
                }
                else if (User.Password.ToLower() == Json.Code.ToLower())
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
            else if (Json.Mode == ReType.Check)
            {
                Object = new ReMessage
                {
                    Build = Token.ContainsKey(Json.User) && Token[Json.User] == Json.Token,
                    Message = "自动登录"
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
                            var time = string.Format("{0:s}", DateTime.Now);
                            File = new()
                            {
                                UUID = Json.UUID,
                                Type = CodeType.Dll,
                                Version = 1,
                                CreateTime = time,
                                UpdataTime = time,
                                Code = CodeDemo.dll_.Replace("{name}", Json.UUID)
                            };
                            CSFile.StorageDll(File);
                            Object = new ReMessage
                            {
                                Build = true,
                                Message = $"Dll[{Json.UUID}]已创建"
                            };
                            GenDll.StartGen(File);
                            ServerMain.LogOut($"Dll[{Json.UUID}]创建");
                        }
                        else
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = $"Dll[{Json.UUID}]已存在"
                            };
                        break;
                    case ReType.AddClass:
                        if (CSFile.GetClass(Json.UUID) == null)
                        {
                            var time = string.Format("{0:s}", DateTime.Now);
                            File = new()
                            {
                                UUID = Json.UUID,
                                Type = CodeType.Class,
                                Version = 1,
                                CreateTime = time,
                                UpdataTime = time,
                                Code = CodeDemo.class_.Replace("{name}", Json.UUID)
                            };
                            CSFile.StorageClass(File);
                            Object = new ReMessage
                            {
                                Build = true,
                                Message = $"Class[{Json.UUID}]已创建"
                            };
                            GenClass.StartGen(File);
                            ServerMain.LogOut($"Class[{Json.UUID}]创建");
                        }
                        else
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = $"Class[{Json.UUID}]已存在"
                            };
                        break;
                    case ReType.AddIoT:
                        if (CSFile.GetIoT(Json.UUID) == null)
                        {
                            var time = string.Format("{0:s}", DateTime.Now);
                            File = new()
                            {
                                UUID = Json.UUID,
                                Type = CodeType.IoT,
                                Version = 1,
                                CreateTime = time,
                                UpdataTime = time,
                                Code = CodeDemo.iot_.Replace("{name}", Json.UUID)
                            };
                            CSFile.StorageIoT(File);
                            Object = new ReMessage
                            {
                                Build = true,
                                Message = $"IoT[{Json.UUID}]已创建"
                            };
                            GenIoT.StartGen(File);
                            ServerMain.LogOut($"IoT[{Json.UUID}]创建");
                        }
                        else
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = $"IoT[{Json.UUID}]已存在"
                            };
                        break;
                    case ReType.AddWebSocket:
                        if (CSFile.GetWebSocket(Json.UUID) == null)
                        {
                            var time = string.Format("{0:s}", DateTime.Now);
                            File = new()
                            {
                                UUID = Json.UUID,
                                Type = CodeType.WebSocket,
                                Version = 1,
                                CreateTime = time,
                                UpdataTime = time,
                                Code = CodeDemo.websocket_.Replace("{name}", Json.UUID)
                            };
                            CSFile.StorageWebSocket(File);
                            Object = new ReMessage
                            {
                                Build = true,
                                Message = $"WebSocket[{Json.UUID}]已创建"
                            };
                            GenWebSocket.StartGen(File);
                            ServerMain.LogOut($"WebSocket[{Json.UUID}]创建");
                        }
                        else
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = $"WebSocket[{Json.UUID}]已存在"
                            };
                        break;
                    case ReType.AddRobot:
                        if (CSFile.GetRobot(Json.UUID) == null)
                        {
                            var time = string.Format("{0:s}", DateTime.Now);
                            File = new()
                            {
                                UUID = Json.UUID,
                                Type = CodeType.Robot,
                                Version = 1,
                                CreateTime = time,
                                UpdataTime = time,
                                Code = CodeDemo.robot_.Replace("{name}", Json.UUID)
                            };
                            CSFile.StorageRobot(File);
                            Object = new ReMessage
                            {
                                Build = true,
                                Message = $"Robot[{Json.UUID}]已创建"
                            };
                            GenRobot.StartGen(File);
                            ServerMain.LogOut($"Robot[{Json.UUID}]创建");
                        }
                        else
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = $"Robot[{Json.UUID}]已存在"
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
                    case ReType.GetApp:
                        List = new CSFileList();
                        foreach (var item in CSFile.AppFileList)
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
                            Message = $"Dll[{Json.UUID}]已删除"
                        };
                        break;
                    case ReType.RemoveClass:
                        CSFile.RemoveFile(CodeType.Class, Json.UUID);
                        Object = new ReMessage
                        {
                            Build = true,
                            Message = $"Class[{Json.UUID}]已删除"
                        };
                        break;
                    case ReType.RemoveIoT:
                        CSFile.RemoveFile(CodeType.IoT, Json.UUID);
                        Object = new ReMessage
                        {
                            Build = true,
                            Message = $"IoT[{Json.UUID}]已删除"
                        };
                        break;
                    case ReType.RemoveWebSocket:
                        CSFile.RemoveFile(CodeType.WebSocket, Json.UUID);
                        Object = new ReMessage
                        {
                            Build = true,
                            Message = $"WebSocket[{Json.UUID}]已删除"
                        };
                        break;
                    case ReType.RemoveRobot:
                        CSFile.RemoveFile(CodeType.Robot, Json.UUID);
                        Object = new ReMessage
                        {
                            Build = true,
                            Message = $"Robot[{Json.UUID}]已删除"
                        };
                        break;
                    case ReType.UpdataDll:
                        File = CSFile.GetDll(Json.UUID);
                        if (File == null)
                        {
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = $"没有这个Dll[{Json.UUID}]"
                            };
                            break;
                        }
                        if (File.Version != Json.Version)
                        {
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = $"Dll[{Json.UUID}]版本号错误"
                            };
                            break;
                        }

                        var list = JsonConvert.DeserializeObject<List<CodeEditObj>>(Json.Code);
                        File.Code = FileEdit.StartEdit(File.Code, list);
                        File.Text = Json.Text;

                        SW = new Stopwatch();
                        SW.Start();
                        BuildBack = GenDll.StartGen(File);
                        SW.Stop();
                        File.Version++;
                        Object = new ReMessage
                        {
                            Build = BuildBack.Isok,
                            Message = BuildBack.Res,
                            UseTime = SW.ElapsedMilliseconds.ToString(),
                            Time = BuildBack.Time
                        };
                        break;
                    case ReType.UpdataClass:
                        File = CSFile.GetClass(Json.UUID);
                        if (File == null)
                        {
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = $"没有这个Class[{Json.UUID}]"
                            };
                            break;
                        }
                        if (File.Version != Json.Version)
                        {
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = $"Class[{Json.UUID}]版本号错误"
                            };
                            break;
                        }

                        list = JsonConvert.DeserializeObject<List<CodeEditObj>>(Json.Code);
                        File.Code = FileEdit.StartEdit(File.Code, list);
                        File.Text = Json.Text;

                        SW = new Stopwatch();
                        SW.Start();
                        BuildBack = GenClass.StartGen(File);
                        SW.Stop();
                        File.Version++;
                        Object = new ReMessage
                        {
                            Build = BuildBack.Isok,
                            Message = BuildBack.Res,
                            UseTime = SW.ElapsedMilliseconds.ToString(),
                            Time = BuildBack.Time
                        };
                        break;
                    case ReType.UpdataIoT:
                        File = CSFile.GetIoT(Json.UUID);
                        if (File == null)
                        {
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = $"没有这个IoT[{Json.UUID}]"
                            };
                            break;
                        }
                        if (File.Version != Json.Version)
                        {
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = $"IoT[{Json.UUID}]版本号错误"
                            };
                            break;
                        }

                        list = JsonConvert.DeserializeObject<List<CodeEditObj>>(Json.Code);
                        File.Code = FileEdit.StartEdit(File.Code, list);
                        File.Text = Json.Text;

                        SW = new Stopwatch();
                        SW.Start();
                        BuildBack = GenIoT.StartGen(File);
                        SW.Stop();
                        File.Version++;
                        Object = new ReMessage
                        {
                            Build = BuildBack.Isok,
                            Message = BuildBack.Res,
                            UseTime = SW.ElapsedMilliseconds.ToString(),
                            Time = BuildBack.Time
                        };
                        break;
                    case ReType.UpdataRobot:
                        File = CSFile.GetRobot(Json.UUID);
                        if (File == null)
                        {
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = $"没有这个Robot[{Json.UUID}]"
                            };
                            break;
                        }
                        if (File.Version != Json.Version)
                        {
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = $"Robot[{Json.UUID}]版本号错误"
                            };
                            break;
                        }

                        list = JsonConvert.DeserializeObject<List<CodeEditObj>>(Json.Code);
                        File.Code = FileEdit.StartEdit(File.Code, list);
                        File.Text = Json.Text;

                        SW = new Stopwatch();
                        SW.Start();
                        BuildBack = GenRobot.StartGen(File);
                        SW.Stop();
                        File.Version++;
                        Object = new ReMessage
                        {
                            Build = BuildBack.Isok,
                            Message = BuildBack.Res,
                            UseTime = SW.ElapsedMilliseconds.ToString(),
                            Time = BuildBack.Time
                        };
                        break;
                    case ReType.UpdataWebSocket:
                        File = CSFile.GetWebSocket(Json.UUID);
                        if (File == null)
                        {
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = $"没有这个WebSocket[{Json.UUID}]"
                            };
                            break;
                        }
                        if (File.Version != Json.Version)
                        {
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = $"WebSocket[{Json.UUID}]版本号错误"
                            };
                            break;
                        }

                        list = JsonConvert.DeserializeObject<List<CodeEditObj>>(Json.Code);
                        File.Code = FileEdit.StartEdit(File.Code, list);
                        File.Text = Json.Text;

                        SW = new Stopwatch();
                        SW.Start();
                        BuildBack = GenWebSocket.StartGen(File);
                        SW.Stop();
                        File.Version++;
                        Object = new ReMessage
                        {
                            Build = BuildBack.Isok,
                            Message = BuildBack.Res,
                            UseTime = SW.ElapsedMilliseconds.ToString(),
                            Time = BuildBack.Time
                        };
                        break;


                    case ReType.AppRemoveFile:
                        var file = CSFile.GetApp(Json.UUID);
                        if (file == null)
                        {
                            Object = CSFile.RemoveAppFile(Json.UUID, Json.Text);
                        }
                        else
                        {
                            Object = new ReMessage
                            {
                                Build = false,
                                Message = "文件未找到"
                            };
                        }
                        break;
                }
            }
            else
            {
                Object = new ReMessage
                {
                    Build = false,
                    Message = "233"
                };
            }
            return new HttpReturn
            {
                Data = StreamUtils.JsonOBJ(Object)
            };
        }
    }
}
