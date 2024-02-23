using ColoryrServer.Core.Database;
using ColoryrServer.Core.Dll.Gen;
using ColoryrServer.Core.FileSystem;
using ColoryrServer.Core.Managers;
using ColoryrServer.SDK;
using ColoryrWork.Lib.Build;
using ColoryrWork.Lib.Build.Object;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer.Core.BuilderPost;

public static class PostDo
{
    public static bool IsRebuild { get; private set; } = false;
    private readonly static Dictionary<string, string> Client = [];
    private readonly static Dictionary<string, int> Timer = [];
    private readonly static Thread TaskThread = new(Run)
    {
        Name = "LogThread"
    };
    private static bool IsRun;
    internal static void Start()
    {
        ServerMain.OnStop += Stop;
        IsRun = true;
        TaskThread.Start();
    }

    private static void Stop()
    {
        IsRun = false;
        Client.Clear();
        Timer.Clear();
    }

    public static async Task<object?> StartBuild(Stream input, string value)
    {
        using MemoryStream stream = new();
        await input.CopyToAsync(stream);
        return StartBuild(stream.ToArray(), value);
    }

    private static object? StartBuild(byte[] input, string value)
    {
        if (value != BuildKV.BuildV)
        {
            return PostRes.Old;
        }
        var receivedData = DeCode.AES256(input,
            ServerMain.Config.AES.Key, ServerMain.Config.AES.IV);
        var json = JsonUtils.ToObj<BuildObj>(receivedData);
        if (json == null)
        {
            return PostRes.ArgError;
        }
        else if (json.Mode == PostBuildType.Login)
        {
            if (json.Code == null || json.User == null)
            {
                return PostRes.UserError;
            }
            else if (LoginDatabase.CheckPassword(json.User.ToLower(), json.Code.ToLower()))
            {
                string uuid = Guid.NewGuid().ToString().Replace("-", "");
                LoginDatabase.UpdateToken(json.User.ToLower(), uuid.ToLower());
                return new ReMessage
                {
                    Build = true,
                    Message = uuid
                };
            }
            else
                return PostRes.UserError;
        }
        else if (json.Mode == PostBuildType.Check)
        {
            if (json.User == null || json.Token == null)
            {
                return PostRes.AutoError;
            }
            return new ReMessage
            {
                Build = LoginDatabase.CheckLogin(json.User.ToLower(), json.Token.ToLower())
            };
        }
        else if (!LoginDatabase.CheckLogin(json.User.ToLower(), json.Token.ToLower()))
        {
            return PostRes.LoginOut;
        }
        else
            return json.Mode switch
            {
                PostBuildType.AddDll => BuildDll.Add(json),
                PostBuildType.AddClass => BuildClass.Add(json),
                PostBuildType.AddSocket => BuildSocket.Add(json),
                PostBuildType.AddWebSocket => BuildWebSocket.Add(json),
                PostBuildType.AddRobot => BuildRobot.Add(json),
                PostBuildType.AddMqtt => BuildMqtt.Add(json),
                PostBuildType.AddService => BuildService.Add(json),
                PostBuildType.AddWeb => BuildWeb.Add(json),
                PostBuildType.GetDll => BuildDll.GetList(),
                PostBuildType.GetClass => BuildClass.GetList(),
                PostBuildType.GetSocket => BuildSocket.GetList(),
                PostBuildType.GetWebSocket => BuildWebSocket.GetList(),
                PostBuildType.GetRobot => BuildRobot.GetList(),
                PostBuildType.GetMqtt => BuildMqtt.GetList(),
                PostBuildType.GetTask => BuildService.GetList(),
                PostBuildType.GetWeb => BuildWeb.GetList(),
                PostBuildType.CodeDll => CodeManager.GetDll(json.UUID),
                PostBuildType.CodeClass => BuildClass.GetCode(json),
                PostBuildType.CodeSocket => CodeManager.GetSocket(json.UUID),
                PostBuildType.CodeWebSocket => CodeManager.GetWebSocket(json.UUID),
                PostBuildType.CodeRobot => CodeManager.GetRobot(json.UUID),
                PostBuildType.CodeMqtt => CodeManager.GetMqtt(json.UUID),
                PostBuildType.CodeTask => CodeManager.GetService(json.UUID),
                PostBuildType.CodeWeb => BuildWeb.GetCode(json),
                PostBuildType.GetApi => APIFile.list,
                PostBuildType.RemoveDll => BuildDll.Remove(json),
                PostBuildType.RemoveClass => BuildClass.Remove(json),
                PostBuildType.RemoveSocket => BuildSocket.Remove(json),
                PostBuildType.RemoveWebSocket => BuildWebSocket.Remove(json),
                PostBuildType.RemoveRobot => BuildRobot.Remove(json),
                PostBuildType.RemoveMqtt => BuildMqtt.Remove(json),
                PostBuildType.RemoveTask => BuildService.Remove(json),
                PostBuildType.RemoveWeb => BuildWeb.Remove(json),
                PostBuildType.UpdataDll => BuildDll.Updata(json),
                PostBuildType.UpdataClass => BuildClass.Updata(json),
                PostBuildType.UpdataSocket => BuildSocket.Updata(json),
                PostBuildType.UpdataRobot => BuildRobot.Updata(json),
                PostBuildType.UpdataWebSocket => BuildWebSocket.Updata(json),
                PostBuildType.UpdataMqtt => BuildMqtt.Updata(json),
                PostBuildType.UpdataTask => BuildService.Updata(json),
                PostBuildType.UpdataWeb => BuildWeb.Updata(json),
                PostBuildType.WebBuild => BuildWeb.Build(json),
                PostBuildType.WebBuildRes => BuildWeb.BuildRes(json),
                PostBuildType.WebAddCode => BuildWeb.AddCode(json),
                PostBuildType.WebRemoveFile => BuildWeb.RemoveFile(json),
                PostBuildType.WebAddFile => BuildWeb.WebAddFile(json),
                PostBuildType.WebSetIsVue => BuildWeb.SetIsVue(json),
                PostBuildType.WebCodeZIP => BuildWeb.ZIP(json),
                PostBuildType.AddClassFile => BuildClass.AddFile(json),
                PostBuildType.RemoveClassFile => BuildClass.RemoveFile(json),
                PostBuildType.BuildClass => BuildClass.Build(json),
                PostBuildType.WebDownloadFile => BuildWeb.Download(json),
                PostBuildType.ConfigGetHttpList => ServerConfig.GetHttpConfigList(json),
                PostBuildType.ConfigAddHttp => ServerConfig.AddHttpConfig(json),
                PostBuildType.ConfigRemoveHttp => ServerConfig.RemoveHttpConfig(json),
                PostBuildType.ConfigAddHttpUrlRoute => ServerConfig.AddHttpUrlRouteConfig(json),
                PostBuildType.ConfigRemoveHttpUrlRoute => ServerConfig.RemoveHttpUrlRouteConfig(json),
                PostBuildType.ConfigGetSocket => ServerConfig.GetSocketConfig(),
                PostBuildType.ConfigSetSocket => ServerConfig.SetSocketConfig(json),
                PostBuildType.ConfigAddHttpRoute => ServerConfig.AddHttpRouteConfig(json),
                PostBuildType.ConfigRemoveHttpRoute => ServerConfig.RemoveHttpRouteConfig(json),
                PostBuildType.SetServerEnable => ServerConfig.SetServerEnable(json),
                PostBuildType.ServerReboot => ServerConfig.Reboot(),
                PostBuildType.ConfigGetUser => PostUser.GetAll(),
                PostBuildType.ConfigAddUser => PostUser.Add(json),
                PostBuildType.ConfigRemoveUser => PostUser.Remove(json),
                PostBuildType.Rebuild => Rebuild(),
                PostBuildType.InitLog => AddClient(json.Token),
                PostBuildType.GetLog => GetLog(json.Token),
                PostBuildType.MakePack => MakePack(),
                _ => PostRes.Error
            };
    }

    public static ReMessage MakePack()
    {
        Task.Run(ServerPackage.MakePackage);
        return new()
        {
            Build = true,
            Message = "服务器正在打包"
        };
    }

    public static ReMessage Rebuild()
    {
        if (IsRebuild)
            return PostRes.RebuildGoing;
        IsRebuild = true;
        Task.Run(() =>
        {
            ServerMain.Config.FixMode = true;
            foreach (var item in CodeManager.ClassFileList)
            {
                var res = GenClass.StartGen(item.Value);
                ServerMain.LogOut(res.Res);
                Thread.Sleep(100);
            }
            foreach (var item in CodeManager.DllFileList)
            {
                var res = GenDll.StartGen(item.Value, "ColoryrServer");
                ServerMain.LogOut(res.Res);
                Thread.Sleep(100);
            }
            foreach (var item in CodeManager.RobotFileList)
            {
                var res = GenRobot.StartGen(item.Value, "ColoryrServer");
                ServerMain.LogOut(res.Res);
                Thread.Sleep(100);
            }
            foreach (var item in CodeManager.MqttFileList)
            {
                var res = GenMqtt.StartGen(item.Value, "ColoryrServer");
                ServerMain.LogOut(res.Res);
                Thread.Sleep(100);
            }
            foreach (var item in CodeManager.SocketFileList)
            {
                var res = GenSocket.StartGen(item.Value, "ColoryrServer");
                ServerMain.LogOut(res.Res);
                Thread.Sleep(100);
            }
            foreach (var item in CodeManager.WebSocketFileList)
            {
                var res = GenWebSocket.StartGen(item.Value, "ColoryrServer");
                ServerMain.LogOut(res.Res);
                Thread.Sleep(100);
            }
            foreach (var item in CodeManager.ServiceFileList)
            {
                var res = GenService.StartGen(item.Value, "ColoryrServer");
                ServerMain.LogOut(res.Res);
                Thread.Sleep(100);
            }
            IsRebuild = false;
        });

        return PostRes.RebuildStart;
    }

    public static ReMessage AddClient(string uuid)
    {
        lock (Timer)
        {
            if (!Timer.TryAdd(uuid, 0))
            {
                Timer[uuid] = 0;
            }
        }
        lock (Client)
        {
            Client.TryAdd(uuid, "");
        }

        return PostRes.AddClient;
    }

    public static ReMessage GetLog(string uuid)
    {
        lock (Timer)
        {
            if (Timer.ContainsKey(uuid))
            {
                Timer[uuid] = 0;
            }
        }

        lock (Client)
        {
            if (Client.TryGetValue(uuid, out string? value))
            {
                string text = value;
                Client[uuid] = "";
                return new ReMessage
                {
                    Build = true,
                    Message = text
                };
            }
        }

        return PostRes.ListError;
    }

    public static void AddLog(string log)
    {
        lock (Client)
        {
            foreach (var item in Client)
            {
                Client[item.Key] += log + Environment.NewLine;
            }
        }
    }

    private static void Run()
    {
        List<string> remove = [];
        while (IsRun)
        {
            try
            {
                lock (Timer)
                {
                    remove.Clear();
                    foreach (var item in Timer)
                    {
                        int time = item.Value + 1;
                        if (time > 600)
                        {
                            remove.Add(item.Key);
                        }
                        else
                        {
                            Timer[item.Key] = time;
                        }
                    }
                    if (remove.Count > 0)
                    {
                        lock (Client)
                        {
                            remove.ForEach((a) =>
                            {
                                Timer.Remove(a);
                                Client.Remove(a);
                            });
                        }
                    }
                }
                Thread.Sleep(1000);
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        }
    }
}
