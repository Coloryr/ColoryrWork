﻿using ColoryrServer.Core;
using ColoryrServer.DllManager.StartGen.GenType;
using ColoryrServer.DllManager.StartGen.GenUtils;
using ColoryrServer.FileSystem;
using ColoryrServer.Http;
using ColoryrWork.Lib.Build;
using ColoryrWork.Lib.Build.Object;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ColoryrServer.DllManager;

public class DllBuild
{
    private static readonly Dictionary<string, string> Token = new();
    public static HttpReturn StartBuild(BuildOBJ json, UserConfig user)
    {
        object resObj = null;
        if (json.Mode == ReType.Login)
        {
            if (json.Code == null)
            {
                resObj = new ReMessage
                {
                    Build = false,
                    Message = "账户或密码错误"
                };
            }
            else if (user.Password.ToLower() == json.Code.ToLower())
            {
                json.UUID = Guid.NewGuid().ToString().Replace("-", "");
                if (Token.ContainsKey(json.User))
                    Token[json.User] = json.UUID;
                else
                    Token.Add(json.User, json.UUID);
                resObj = new ReMessage
                {
                    Build = true,
                    Message = json.UUID
                };
            }
            else
                resObj = new ReMessage
                {
                    Build = false,
                    Message = "账户或密码错误"
                };
        }
        else if (json.Mode == ReType.Check)
        {
            resObj = new ReMessage
            {
                Build = Token.ContainsKey(json.User) && Token[json.User] == json.Token,
                Message = "自动登录"
            };
        }
        else if (Token.ContainsKey(json.User) && Token[json.User] == json.Token)
        {
            GenReOBJ BuildBack;
            Stopwatch SW;
            CSFileCode File;
            WebObj File2;
            CSFileList List;
            switch (json.Mode)
            {
                case ReType.AddDll:
                    if (CSFile.GetDll(json.UUID) == null)
                    {
                        var time = string.Format("{0:s}", DateTime.Now);
                        File = new()
                        {
                            UUID = json.UUID,
                            Type = CodeType.Dll,
                            CreateTime = time,
                            Code = DemoResource.Dll
                            .Replace(CodeDemo.Name, json.UUID)
                        };
                        CSFile.StorageDll(File);
                        resObj = new ReMessage
                        {
                            Build = true,
                            Message = $"Dll[{json.UUID}]已创建"
                        };
                        GenDll.StartGen(File);
                        ServerMain.LogOut($"Dll[{json.UUID}]创建");
                    }
                    else
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Dll[{json.UUID}]已存在"
                        };
                    break;
                case ReType.AddClass:
                    if (CSFile.GetClass(json.UUID) == null)
                    {
                        var time = string.Format("{0:s}", DateTime.Now);
                        File = new()
                        {
                            UUID = json.UUID,
                            Type = CodeType.Class,
                            CreateTime = time,
                            Code = DemoResource.Class
                            .Replace(CodeDemo.Name, json.UUID)
                        };
                        CSFile.StorageClass(File);
                        resObj = new ReMessage
                        {
                            Build = true,
                            Message = $"Class[{json.UUID}]已创建"
                        };
                        GenClass.StartGen(File);
                        ServerMain.LogOut($"Class[{json.UUID}]创建");
                    }
                    else
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Class[{json.UUID}]已存在"
                        };
                    break;
                case ReType.AddSocket:
                    if (CSFile.GetSocket(json.UUID) == null)
                    {
                        var time = string.Format("{0:s}", DateTime.Now);
                        File = new()
                        {
                            UUID = json.UUID,
                            Type = CodeType.Socket,
                            CreateTime = time,
                            Code = DemoResource.Socket
                            .Replace(CodeDemo.Name, json.UUID)
                        };
                        CSFile.StorageSocket(File);
                        resObj = new ReMessage
                        {
                            Build = true,
                            Message = $"Socket[{json.UUID}]已创建"
                        };
                        GenSocket.StartGen(File);
                        ServerMain.LogOut($"Socket[{json.UUID}]创建");
                    }
                    else
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Socket[{json.UUID}]已存在"
                        };
                    break;
                case ReType.AddWebSocket:
                    if (CSFile.GetWebSocket(json.UUID) == null)
                    {
                        var time = string.Format("{0:s}", DateTime.Now);
                        File = new()
                        {
                            UUID = json.UUID,
                            Type = CodeType.WebSocket,
                            CreateTime = time,
                            Code = DemoResource.WebSocket
                            .Replace(CodeDemo.Name, json.UUID)
                        };
                        CSFile.StorageWebSocket(File);
                        resObj = new ReMessage
                        {
                            Build = true,
                            Message = $"WebSocket[{json.UUID}]已创建"
                        };
                        GenWebSocket.StartGen(File);
                        ServerMain.LogOut($"WebSocket[{json.UUID}]创建");
                    }
                    else
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"WebSocket[{json.UUID}]已存在"
                        };
                    break;
                case ReType.AddRobot:
                    if (CSFile.GetRobot(json.UUID) == null)
                    {
                        var time = string.Format("{0:s}", DateTime.Now);
                        File = new()
                        {
                            UUID = json.UUID,
                            Type = CodeType.Robot,
                            CreateTime = time,
                            Code = DemoResource.Robot
                            .Replace(CodeDemo.Name, json.UUID)
                        };
                        CSFile.StorageRobot(File);
                        resObj = new ReMessage
                        {
                            Build = true,
                            Message = $"Robot[{json.UUID}]已创建"
                        };
                        GenRobot.StartGen(File);
                        ServerMain.LogOut($"Robot[{json.UUID}]创建");
                    }
                    else
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Robot[{json.UUID}]已存在"
                        };
                    break;
                case ReType.AddMqtt:
                    if (CSFile.GetMqtt(json.UUID) == null)
                    {
                        var time = string.Format("{0:s}", DateTime.Now);
                        File = new()
                        {
                            UUID = json.UUID,
                            Type = CodeType.Mqtt,
                            CreateTime = time,
                            Code = DemoResource.Mqtt
                            .Replace(CodeDemo.Name, json.UUID)
                        };
                        CSFile.StorageMqtt(File);
                        resObj = new ReMessage
                        {
                            Build = true,
                            Message = $"Mqtt[{json.UUID}]已创建"
                        };
                        GenMqtt.StartGen(File);
                        ServerMain.LogOut($"Mqtt[{json.UUID}]创建");
                    }
                    else
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Mqtt[{json.UUID}]已存在"
                        };
                    break;
                case ReType.AddTask:
                    if (CSFile.GetTask(json.UUID) == null)
                    {
                        var time = string.Format("{0:s}", DateTime.Now);
                        File = new()
                        {
                            UUID = json.UUID,
                            Type = CodeType.Task,
                            CreateTime = time,
                            Code = DemoResource.Task
                            .Replace(CodeDemo.Name, json.UUID)
                        };
                        CSFile.StorageTask(File);
                        resObj = new ReMessage
                        {
                            Build = true,
                            Message = $"Task[{json.UUID}]已创建"
                        };
                        GenTask.StartGen(File);
                        ServerMain.LogOut($"Task[{json.UUID}]创建");
                    }
                    else
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Task[{json.UUID}]已存在"
                        };
                    break;
                case ReType.AddWeb:
                    if (HtmlUtils.GetHtml(json.UUID) == null)
                    {
                        var time = string.Format("{0:s}", DateTime.Now);
                        File2 = new()
                        {
                            UUID = json.UUID,
                            CreateTime = time,
                            Text = "",
                            Codes = new()
                            {
                                {
                                    "index.html",
                                    DemoResource.Html
                                    .Replace(CodeDemo.Name, json.UUID)
                                },
                                { "js.js", DemoResource.Js }
                            },
                            Files = new()
                        };
                        HtmlUtils.New(File2);
                        resObj = new ReMessage
                        {
                            Build = true,
                            Message = $"Web[{json.UUID}]已创建"
                        };
                        ServerMain.LogOut($"Web[{json.UUID}]创建");
                    }
                    else
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Web[{json.UUID}]已存在"
                        };
                    break;
                case ReType.GetDll:
                    List = new CSFileList();
                    foreach (var item in CSFile.DllFileList)
                    {
                        List.List.Add(item.Key, item.Value);
                    }
                    resObj = List;
                    break;
                case ReType.GetClass:
                    List = new CSFileList();
                    foreach (var item in CSFile.ClassFileList)
                    {
                        List.List.Add(item.Key, item.Value);
                    }
                    resObj = List;
                    break;
                case ReType.GetSocket:
                    List = new CSFileList();
                    foreach (var item in CSFile.SocketFileList)
                    {
                        List.List.Add(item.Key, item.Value);
                    }
                    resObj = List;
                    break;
                case ReType.GetWebSocket:
                    List = new CSFileList();
                    foreach (var item in CSFile.WebSocketFileList)
                    {
                        List.List.Add(item.Key, item.Value);
                    }
                    resObj = List;
                    break;
                case ReType.GetRobot:
                    List = new CSFileList();
                    foreach (var item in CSFile.RobotFileList)
                    {
                        List.List.Add(item.Key, item.Value);
                    }
                    resObj = List;
                    break;
                case ReType.GetMqtt:
                    List = new CSFileList();
                    foreach (var item in CSFile.MqttFileList)
                    {
                        List.List.Add(item.Key, item.Value);
                    }
                    resObj = List;
                    break;
                case ReType.GetTask:
                    List = new CSFileList();
                    foreach (var item in CSFile.TaskFileList)
                    {
                        List.List.Add(item.Key, item.Value);
                    }
                    resObj = List;
                    break;
                case ReType.GetWeb:
                    List = new CSFileList();
                    foreach (var item in HtmlUtils.HtmlCodeList)
                    {
                        List.List.Add(item.Key, item.Value);
                    }
                    resObj = List;
                    break;
                case ReType.CodeDll:
                    resObj = CSFile.GetDll(json.UUID);
                    break;
                case ReType.CodeClass:
                    resObj = CSFile.GetClass(json.UUID);
                    break;
                case ReType.CodeSocket:
                    resObj = CSFile.GetSocket(json.UUID);
                    break;
                case ReType.CodeWebSocket:
                    resObj = CSFile.GetWebSocket(json.UUID);
                    break;
                case ReType.CodeRobot:
                    resObj = CSFile.GetRobot(json.UUID);
                    break;
                case ReType.CodeMqtt:
                    resObj = CSFile.GetMqtt(json.UUID);
                    break;
                case ReType.CodeTask:
                    resObj = CSFile.GetTask(json.UUID);
                    break;
                case ReType.CodeWeb:
                    resObj = HtmlUtils.GetHtml(json.UUID);
                    break;
                case ReType.GetApi:
                    resObj = APIFile.list;
                    break;
                case ReType.RemoveDll:
                    CSFile.RemoveFile(CodeType.Dll, json.UUID);
                    resObj = new ReMessage
                    {
                        Build = true,
                        Message = $"Dll[{json.UUID}]已删除"
                    };
                    break;
                case ReType.RemoveClass:
                    CSFile.RemoveFile(CodeType.Class, json.UUID);
                    resObj = new ReMessage
                    {
                        Build = true,
                        Message = $"Class[{json.UUID}]已删除"
                    };
                    break;
                case ReType.RemoveSocket:
                    CSFile.RemoveFile(CodeType.Socket, json.UUID);
                    resObj = new ReMessage
                    {
                        Build = true,
                        Message = $"Socket[{json.UUID}]已删除"
                    };
                    break;
                case ReType.RemoveWebSocket:
                    CSFile.RemoveFile(CodeType.WebSocket, json.UUID);
                    resObj = new ReMessage
                    {
                        Build = true,
                        Message = $"WebSocket[{json.UUID}]已删除"
                    };
                    break;
                case ReType.RemoveRobot:
                    CSFile.RemoveFile(CodeType.Robot, json.UUID);
                    resObj = new ReMessage
                    {
                        Build = true,
                        Message = $"Robot[{json.UUID}]已删除"
                    };
                    break;
                case ReType.RemoveMqtt:
                    CSFile.RemoveFile(CodeType.Mqtt, json.UUID);
                    resObj = new ReMessage
                    {
                        Build = true,
                        Message = $"Mqtt[{json.UUID}]已删除"
                    };
                    break;
                case ReType.RemoveTask:
                    CSFile.RemoveFile(CodeType.Task, json.UUID);
                    resObj = new ReMessage
                    {
                        Build = true,
                        Message = $"Task[{json.UUID}]已删除"
                    };
                    break;
                case ReType.UpdataDll:
                    File = CSFile.GetDll(json.UUID);
                    if (File == null)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"没有这个Dll[{json.UUID}]"
                        };
                        break;
                    }
                    if (File.Version != json.Version)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Dll[{json.UUID}]版本号错误"
                        };
                        break;
                    }

                    var list = JsonConvert.DeserializeObject<List<CodeEditObj>>(json.Code);
                    File.Code = FileEdit.StartEdit(File.Code, list);
                    File.Text = json.Text;

                    SW = new Stopwatch();
                    SW.Start();
                    BuildBack = GenDll.StartGen(File);
                    SW.Stop();
                    File.Up();
                    resObj = new ReMessage
                    {
                        Build = BuildBack.Isok,
                        Message = BuildBack.Res,
                        UseTime = SW.ElapsedMilliseconds.ToString(),
                        Time = BuildBack.Time
                    };
                    break;
                case ReType.UpdataClass:
                    File = CSFile.GetClass(json.UUID);
                    if (File == null)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"没有这个Class[{json.UUID}]"
                        };
                        break;
                    }
                    if (File.Version != json.Version)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Class[{json.UUID}]版本号错误"
                        };
                        break;
                    }

                    list = JsonConvert.DeserializeObject<List<CodeEditObj>>(json.Code);
                    File.Code = FileEdit.StartEdit(File.Code, list);
                    File.Text = json.Text;

                    SW = new Stopwatch();
                    SW.Start();
                    BuildBack = GenClass.StartGen(File);
                    SW.Stop();
                    File.Up();
                    resObj = new ReMessage
                    {
                        Build = BuildBack.Isok,
                        Message = BuildBack.Res,
                        UseTime = SW.ElapsedMilliseconds.ToString(),
                        Time = BuildBack.Time
                    };
                    break;
                case ReType.UpdataSocket:
                    File = CSFile.GetSocket(json.UUID);
                    if (File == null)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"没有这个Socket[{json.UUID}]"
                        };
                        break;
                    }
                    if (File.Version != json.Version)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Socket[{json.UUID}]版本号错误"
                        };
                        break;
                    }

                    list = JsonConvert.DeserializeObject<List<CodeEditObj>>(json.Code);
                    File.Code = FileEdit.StartEdit(File.Code, list);
                    File.Text = json.Text;

                    SW = new Stopwatch();
                    SW.Start();
                    BuildBack = GenSocket.StartGen(File);
                    SW.Stop();
                    File.Up();
                    resObj = new ReMessage
                    {
                        Build = BuildBack.Isok,
                        Message = BuildBack.Res,
                        UseTime = SW.ElapsedMilliseconds.ToString(),
                        Time = BuildBack.Time
                    };
                    break;
                case ReType.UpdataRobot:
                    File = CSFile.GetRobot(json.UUID);
                    if (File == null)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"没有这个Robot[{json.UUID}]"
                        };
                        break;
                    }
                    if (File.Version != json.Version)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Robot[{json.UUID}]版本号错误"
                        };
                        break;
                    }

                    list = JsonConvert.DeserializeObject<List<CodeEditObj>>(json.Code);
                    File.Code = FileEdit.StartEdit(File.Code, list);
                    File.Text = json.Text;

                    SW = new Stopwatch();
                    SW.Start();
                    BuildBack = GenRobot.StartGen(File);
                    SW.Stop();
                    File.Up();
                    resObj = new ReMessage
                    {
                        Build = BuildBack.Isok,
                        Message = BuildBack.Res,
                        UseTime = SW.ElapsedMilliseconds.ToString(),
                        Time = BuildBack.Time
                    };
                    break;
                case ReType.UpdataWebSocket:
                    File = CSFile.GetWebSocket(json.UUID);
                    if (File == null)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"没有这个WebSocket[{json.UUID}]"
                        };
                        break;
                    }
                    if (File.Version != json.Version)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"WebSocket[{json.UUID}]版本号错误"
                        };
                        break;
                    }

                    list = JsonConvert.DeserializeObject<List<CodeEditObj>>(json.Code);
                    File.Code = FileEdit.StartEdit(File.Code, list);
                    File.Text = json.Text;

                    SW = new Stopwatch();
                    SW.Start();
                    BuildBack = GenWebSocket.StartGen(File);
                    SW.Stop();
                    File.Up();
                    resObj = new ReMessage
                    {
                        Build = BuildBack.Isok,
                        Message = BuildBack.Res,
                        UseTime = SW.ElapsedMilliseconds.ToString(),
                        Time = BuildBack.Time
                    };
                    break;
                case ReType.UpdataMqtt:
                    File = CSFile.GetMqtt(json.UUID);
                    if (File == null)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"没有这个Mqtt[{json.UUID}]"
                        };
                        break;
                    }
                    if (File.Version != json.Version)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Mqtt[{json.UUID}]版本号错误"
                        };
                        break;
                    }

                    list = JsonConvert.DeserializeObject<List<CodeEditObj>>(json.Code);
                    File.Code = FileEdit.StartEdit(File.Code, list);
                    File.Text = json.Text;

                    SW = new Stopwatch();
                    SW.Start();
                    BuildBack = GenMqtt.StartGen(File);
                    SW.Stop();
                    File.Up();
                    resObj = new ReMessage
                    {
                        Build = BuildBack.Isok,
                        Message = BuildBack.Res,
                        UseTime = SW.ElapsedMilliseconds.ToString(),
                        Time = BuildBack.Time
                    };
                    break;
                case ReType.UpdataTask:
                    File = CSFile.GetTask(json.UUID);
                    if (File == null)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"没有这个Task[{json.UUID}]"
                        };
                        break;
                    }
                    if (File.Version != json.Version)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Task[{json.UUID}]版本号错误"
                        };
                        break;
                    }

                    list = JsonConvert.DeserializeObject<List<CodeEditObj>>(json.Code);
                    File.Code = FileEdit.StartEdit(File.Code, list);
                    File.Text = json.Text;

                    SW = new Stopwatch();
                    SW.Start();
                    BuildBack = GenTask.StartGen(File);
                    SW.Stop();
                    File.Up();
                    resObj = new ReMessage
                    {
                        Build = BuildBack.Isok,
                        Message = BuildBack.Res,
                        UseTime = SW.ElapsedMilliseconds.ToString(),
                        Time = BuildBack.Time
                    };
                    break;
                case ReType.UpdataWeb:
                    File2 = HtmlUtils.GetHtml(json.UUID);
                    if (File2 == null)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"没有这个Web[{json.UUID}]"
                        };
                        break;
                    }
                    if (File2.Version != json.Version)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Web[{json.UUID}]版本号错误"
                        };
                        break;
                    }

                    list = JsonConvert.DeserializeObject<List<CodeEditObj>>(json.Code);
                    if (!File2.Codes.TryGetValue(json.Temp, out var code))
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Web[{json.UUID}]不存在文件[{json.Temp}]"
                        };
                        break;
                    }
                    code = FileEdit.StartEdit(code, list);
                    File2.Text = json.Text;

                    HtmlUtils.Save(File2, json.Temp, code);
                    resObj = new ReMessage
                    {
                        Build = true,
                        Message = $"Web[{json.UUID}]文件[{json.Temp}]已修改",
                        UseTime = "0",
                        Time = File2.UpdataTime
                    };
                    break;
                case ReType.WebAddCode:
                    File2 = HtmlUtils.GetHtml(json.UUID);
                    if (File2 == null)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Web[{json.UUID}]没有找到"
                        };
                        break;
                    }
                    if (File2.Codes.ContainsKey(json.Code))
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Web[{json.UUID}]的源码文件[{json.Code}]已存在"
                        };
                        break;
                    }

                    HtmlUtils.AddCode(File2, json.Code, "");
                    resObj = new ReMessage
                    {
                        Build = true,
                        Message = $"Web[{json.UUID}]已添加源码文件[{json.Code}]"
                    };
                    break;
                case ReType.WebRemoveFile:
                    File2 = HtmlUtils.GetHtml(json.UUID);
                    if (File2 == null)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Web[{json.UUID}]没有找到"
                        };
                        break;
                    }
                    if (json.Code == "index.html")
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Web[{json.UUID}]的主文件不允许删除"
                        };
                        break;
                    }
                    if (!File2.Codes.ContainsKey(json.Code) && !File2.Files.ContainsKey(json.Code))
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Web[{json.UUID}]的文件[{json.Code}]不存在"
                        };
                        break;
                    }

                    HtmlUtils.Remove(File2, json.Code);
                    resObj = new ReMessage
                    {
                        Build = true,
                        Message = $"Web[{json.UUID}]已删除文件[{json.Code}]"
                    };
                    break;
                case ReType.WebAddFile:
                    File2 = HtmlUtils.GetHtml(json.UUID);
                    if (File2 == null)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Web[{json.UUID}]没有找到"
                        };
                        break;
                    }
                    if (File2.Files.ContainsKey(json.Code))
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Web[{json.UUID}]的文件[{json.Code}]已存在"
                        };
                        break;
                    }

                    HtmlUtils.AddFile(File2, json.Code, BuildUtils.HexStringToByte(json.Temp));
                    resObj = new ReMessage
                    {
                        Build = true,
                        Message = $"Web[{json.UUID}]已添加文件[{json.Code}]"
                    };
                    break;
                case ReType.RemoveWeb:
                    File2 = HtmlUtils.GetHtml(json.UUID);
                    if (File2 == null)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Web[{json.UUID}]没有找到"
                        };
                        break;
                    }

                    HtmlUtils.DeleteAll(File2);
                    resObj = new ReMessage
                    {
                        Build = true,
                        Message = $"Web[{json.UUID}]已删除"
                    };
                    break;
            }
        }
        else
        {
            resObj = new ReMessage
            {
                Build = false,
                Message = "233"
            };
        }
        return new HttpReturn
        {
            Data = resObj,
            Res = ResType.Json
        };
    }
}
