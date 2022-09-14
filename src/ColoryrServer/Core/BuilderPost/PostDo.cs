using ColoryrServer.Core.DllManager.Gen;
using ColoryrServer.Core.FileSystem;
using ColoryrServer.Core.FileSystem.Code;
using ColoryrServer.SDK;
using ColoryrWork.Lib.Build;
using ColoryrWork.Lib.Build.Object;
using ColoryrWork.Lib.Server;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer.Core.BuilderPost;

public static class PostDo
{
    public static bool IsRebuild { get; private set; } = false;
    private readonly static Dictionary<string, string> Client = new();
    private readonly static Dictionary<string, int> Timer = new();
    private readonly static Thread TaskThread = new(Run) 
    {
        Name = "LogThread"   
    };
    private static bool IsRun;
    public static void Start() 
    {
        IsRun = true;
        TaskThread.Start();
        ServerMain.OnStop += Stop;
    }

    public static void Stop() 
    {
        IsRun = false;
        Client.Clear();
        Timer.Clear();
    }

    public static async Task<object> StartBuild(Stream input, string value)
    {
        using MemoryStream stream = new();
        await input.CopyToAsync(stream);
        return StartBuild(stream.ToArray(), value);
    }

    public static object StartBuild(byte[] input, string value)
    {
        if (value != BuildKV.BuildV)
        {
            return PostRes.Old;
        }
        var receivedData = DeCode.AES256(input,
            ServerMain.Config.AES.Key, ServerMain.Config.AES.IV);
        var str = Encoding.UTF8.GetString(receivedData);
        JObject obj;
        try
        {
            obj = JObject.Parse(Function.GetSrings(str, "{"));
        }
        catch
        {
            return PostRes.ArgError;
        }
        var json = obj.ToObject<BuildOBJ>();
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
            else if (LoginSave.CheckPassword(json.User.ToLower(), json.Code.ToLower()))
            {
                string uuid = Guid.NewGuid().ToString().Replace("-", "");
                LoginSave.UpdateToken(json.User.ToLower(), uuid.ToLower());
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
                Build = LoginSave.CheckLogin(json.User.ToLower(), json.Token.ToLower())
            };
        }
        else if (!LoginSave.CheckLogin(json.User.ToLower(), json.Token.ToLower()))
        {
            return PostRes.LoginOut;
        } 
        else
            return json.Mode switch
            {
                PostBuildType.AddDll => PostBuildDll.Add(json),
                PostBuildType.AddClass => PostBuildClass.Add(json),
                PostBuildType.AddSocket => PostBuildSocket.Add(json),
                PostBuildType.AddWebSocket => PostBuildWebSocket.Add(json),
                PostBuildType.AddRobot => PostBuildRobot.Add(json),
                PostBuildType.AddMqtt => PostBuildMqtt.Add(json),
                PostBuildType.AddService => PostBuildService.Add(json),
                PostBuildType.AddWeb => PostBuildWeb.Add(json),
                PostBuildType.GetDll => PostBuildDll.GetList(),
                PostBuildType.GetClass => PostBuildClass.GetList(),
                PostBuildType.GetSocket => PostBuildSocket.GetList(),
                PostBuildType.GetWebSocket => PostBuildWebSocket.GetList(),
                PostBuildType.GetRobot => PostBuildRobot.GetList(),
                PostBuildType.GetMqtt => PostBuildMqtt.GetList(),
                PostBuildType.GetTask => PostBuildService.GetList(),
                PostBuildType.GetWeb => PostBuildWeb.GetList(),
                PostBuildType.CodeDll => CodeFileManager.GetDll(json.UUID),
                PostBuildType.CodeClass => PostBuildClass.GetCode(json),
                PostBuildType.CodeSocket => CodeFileManager.GetSocket(json.UUID),
                PostBuildType.CodeWebSocket => CodeFileManager.GetWebSocket(json.UUID),
                PostBuildType.CodeRobot => CodeFileManager.GetRobot(json.UUID),
                PostBuildType.CodeMqtt => CodeFileManager.GetMqtt(json.UUID),
                PostBuildType.CodeTask => CodeFileManager.GetTask(json.UUID),
                PostBuildType.CodeWeb => PostBuildWeb.GetCode(json),
                PostBuildType.GetApi => APIFile.list,
                PostBuildType.RemoveDll => PostBuildDll.Remove(json),
                PostBuildType.RemoveClass => PostBuildClass.Remove(json),
                PostBuildType.RemoveSocket => PostBuildSocket.Remove(json),
                PostBuildType.RemoveWebSocket => PostBuildWebSocket.Remove(json),
                PostBuildType.RemoveRobot => PostBuildRobot.Remove(json),
                PostBuildType.RemoveMqtt => PostBuildMqtt.Remove(json),
                PostBuildType.RemoveTask => PostBuildService.Remove(json),
                PostBuildType.RemoveWeb => PostBuildWeb.Remove(json),
                PostBuildType.UpdataDll => PostBuildDll.Updata(json),
                PostBuildType.UpdataClass => PostBuildClass.Updata(json),
                PostBuildType.UpdataSocket => PostBuildSocket.Updata(json),
                PostBuildType.UpdataRobot => PostBuildRobot.Updata(json),
                PostBuildType.UpdataWebSocket => PostBuildWebSocket.Updata(json),
                PostBuildType.UpdataMqtt => PostBuildMqtt.Updata(json),
                PostBuildType.UpdataTask => PostBuildService.Updata(json),
                PostBuildType.UpdataWeb => PostBuildWeb.Updata(json),
                PostBuildType.WebBuild => PostBuildWeb.Build(json),
                PostBuildType.WebBuildRes => PostBuildWeb.BuildRes(json),
                PostBuildType.WebAddCode => PostBuildWeb.AddCode(json),
                PostBuildType.WebRemoveFile => PostBuildWeb.RemoveFile(json),
                PostBuildType.WebAddFile => PostBuildWeb.WebAddFile(json),
                PostBuildType.WebSetIsVue => PostBuildWeb.SetIsVue(json),
                PostBuildType.WebCodeZIP => PostBuildWeb.ZIP(json),
                PostBuildType.AddClassFile => PostBuildClass.AddFile(json),
                PostBuildType.RemoveClassFile => PostBuildClass.RemoveFile(json),
                PostBuildType.BuildClass => PostBuildClass.Build(json),
                PostBuildType.WebDownloadFile => PostBuildWeb.Download(json),
                PostBuildType.ConfigGetHttpList => PostServerConfig.GetHttpConfigList(json),
                PostBuildType.ConfigAddHttp => PostServerConfig.AddHttpConfig(json),
                PostBuildType.ConfigRemoveHttp => PostServerConfig.RemoveHttpConfig(json),
                PostBuildType.ConfigAddHttpUrlRoute => PostServerConfig.AddHttpUrlRouteConfig(json),
                PostBuildType.ConfigRemoveHttpUrlRoute => PostServerConfig.RemoveHttpUrlRouteConfig(json),
                PostBuildType.ConfigGetSocket => PostServerConfig.GetSocketConfig(),
                PostBuildType.ConfigGetRobot => PostServerConfig.GetRobotConfig(),
                PostBuildType.ConfigSetRobot => PostServerConfig.SetRobotConfig(json),
                PostBuildType.ConfigAddHttpRoute => PostServerConfig.AddHttpRouteConfig(json),
                PostBuildType.ConfigRemoveHttpRoute => PostServerConfig.RemoveHttpRouteConfig(json),
                PostBuildType.SetServerEnable => PostServerConfig.SetServerEnable(json),
                PostBuildType.ServerReboot => PostServerConfig.Reboot(),
                PostBuildType.ConfigGetUser => PostUser.GetAll(),
                PostBuildType.ConfigAddUser => PostUser.Add(json),
                PostBuildType.ConfigRemoveUser => PostUser.Remove(json),
                PostBuildType.Rebuild => Rebuild(),
                PostBuildType.InitLog => AddClient(json.Token),
                PostBuildType.GetLog => GetLog(json.Token),
                _ => PostRes.Error
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
            foreach (var item in CodeFileManager.ClassFileList)
            {
                GenClass.StartGen(item.Value);
                Thread.Sleep(100);
            }
            foreach (var item in CodeFileManager.DllFileList)
            {
                GenDll.StartGen(item.Value, "ColoryrServer");
                Thread.Sleep(100);
            }
        });

        return PostRes.RebuildStart;
    }

    public static ReMessage AddClient(string uuid)
    {
        lock (Timer)
        {
            if (Timer.ContainsKey(uuid))
            {
                Timer[uuid] = 0;
            }
            else
            {
                Timer.Add(uuid, 0);
            }
        }
        lock (Client)
        {
            if (!Client.ContainsKey(uuid))
            {
                Client.Add(uuid, "");
            }
        }

        return PostRes.AddClient; 
    }

    public static ReMessage GetLog(string uuid) 
    {
        lock(Timer)
        {
            if (Timer.ContainsKey(uuid))
            {
                Timer[uuid] = 0;
            }
        }

        lock (Client)
        {
            if (Client.ContainsKey(uuid))
            {
                string text = Client[uuid];
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
        List<string> remove = new();
        while (IsRun)
        {
            try
            {
                lock (Timer)
                {
                    remove.Clear();
                    foreach (var item in Timer)
                    {
                        int time = item.Value+ 1;
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
