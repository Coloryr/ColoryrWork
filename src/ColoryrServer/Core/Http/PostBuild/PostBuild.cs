using ColoryrServer.Core.FileSystem;
using ColoryrServer.Core.FileSystem.Code;
using ColoryrServer.Core.Http.PostBuild;
using ColoryrWork.Lib.Build.Object;
using System;

namespace ColoryrServer.Core.DllManager.PostBuild;

public static class PostBuild
{
    public static object StartBuild(BuildOBJ json)
    {
        if (json == null)
        {
            return new ReMessage
            {
                Build = false,
                Message = "参数错误"
            };
        }
        if (json.Mode == PostBuildType.Login)
        {
            if (json.Code == null || json.User == null)
            {
                return new ReMessage
                {
                    Build = false,
                    Message = "账户或密码错误"
                };
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
                return new ReMessage
                {
                    Build = false,
                    Message = "账户或密码错误"
                };
        }
        else if (json.Mode == PostBuildType.Check)
        {
            if (json.User == null || json.Token == null)
            {
                return new ReMessage
                {
                    Build = false,
                    Message = "自动登录"
                };
            }
            return new ReMessage
            {
                Build = LoginSave.CheckLogin(json.User.ToLower(), json.Token.ToLower()),
                Message = "自动登录"
            };
        }
        else if (LoginSave.CheckLogin(json.User.ToLower(), json.Token.ToLower()))
        {
            return json.Mode switch
            {
                PostBuildType.AddDll => PostBuildDll.Add(json),
                PostBuildType.AddClass => PostBuildClass.Add(json),
                PostBuildType.AddSocket => PostBuildSocket.Add(json),
                PostBuildType.AddWebSocket => PostBuildWebSocket.Add(json),
                PostBuildType.AddRobot => PostBuildRobot.Add(json),
                PostBuildType.AddMqtt => PostBuildMqtt.Add(json),
                PostBuildType.AddTask => PostBuildTask.Add(json),
                PostBuildType.AddWeb => PostBuildWeb.Add(json),
                PostBuildType.GetDll => PostBuildDll.GetList(),
                PostBuildType.GetClass => PostBuildClass.GetList(),
                PostBuildType.GetSocket => PostBuildSocket.GetList(),
                PostBuildType.GetWebSocket => PostBuildWebSocket.GetList(),
                PostBuildType.GetRobot => PostBuildRobot.GetList(),
                PostBuildType.GetMqtt => PostBuildMqtt.GetList(),
                PostBuildType.GetTask => PostBuildTask.GetList(),
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
                PostBuildType.RemoveTask => PostBuildTask.Remove(json),
                PostBuildType.RemoveWeb => PostBuildWeb.Remove(json),
                PostBuildType.UpdataDll => PostBuildDll.Updata(json),
                PostBuildType.UpdataClass => PostBuildClass.Updata(json),
                PostBuildType.UpdataSocket => PostBuildSocket.Updata(json),
                PostBuildType.UpdataRobot => PostBuildRobot.Updata(json),
                PostBuildType.UpdataWebSocket => PostBuildWebSocket.Updata(json),
                PostBuildType.UpdataMqtt => PostBuildMqtt.Updata(json),
                PostBuildType.UpdataTask => PostBuildTask.Updata(json),
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
                _ => new ReMessage
                {
                    Build = false,
                    Message = "null"
                }
            };
        }
        else
        {
            return new ReMessage
            {
                Build = false,
                Message = "233"
            };
        }
    }
}
