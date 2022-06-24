using ColoryrWork.Lib.Build.Object;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ColoryrBuild.PostBuild;

public partial class HttpUtils : HttpUtilsBase
{
    public async Task<CSFileList> GetList(CodeType type)
    {
        try
        {
            var reType = type switch
            {
                CodeType.Class => PostBuildType.GetClass,
                CodeType.Socket => PostBuildType.GetSocket,
                CodeType.Robot => PostBuildType.GetRobot,
                CodeType.WebSocket => PostBuildType.GetWebSocket,
                CodeType.Mqtt => PostBuildType.GetMqtt,
                CodeType.Task => PostBuildType.GetTask,
                CodeType.Web => PostBuildType.GetWeb,
                CodeType.Dll => PostBuildType.GetDll,
                _ => throw new NotImplementedException()
            };
            var pack = new BuildOBJ
            {
                User = App.Config.Name,
                Token = App.Config.Token,
                Mode = reType
            };
            HttpContent Content = new ByteArrayContent(AES(JsonConvert.SerializeObject(pack)));
            Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var temp = await httpClient.PostAsync(App.Config.Http, Content);
            var data = await temp.Content.ReadAsStringAsync();
            if (!CheckLogin(data))
            {
                return await GetList(type);
            }
            return JsonConvert.DeserializeObject<CSFileList>(data);
        }
        catch
        {
            return null;
        }
    }

    public async Task<ReMessage> ClassFileEdit(CSFileCode obj, string file, List<CodeEditObj> list)
    {
        try
        {
            var pack = new BuildOBJ
            {
                User = App.Config.Name,
                Token = App.Config.Token,
                Mode = PostBuildType.UpdataClass,
                UUID = obj.UUID,
                Version = obj.Version,
                Text = obj.Text,
                Temp = file,
                Code = JsonConvert.SerializeObject(list)
            };
            HttpContent Content = new ByteArrayContent(AES(JsonConvert.SerializeObject(pack)));
            Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var temp = await httpClient.PostAsync(App.Config.Http, Content);
            var data = await temp.Content.ReadAsStringAsync();
            if (!CheckLogin(data))
            {
                return await ClassFileEdit(obj, file, list);
            }
            return JsonConvert.DeserializeObject<ReMessage>(data);
        }
        catch
        {
            return null;
        }
    }

    public async Task<ReMessage> Add(CodeType type, string name, string arg = null)
    {
        try
        {
            var reType = type switch
            {
                CodeType.Class => PostBuildType.AddClass,
                CodeType.Socket => PostBuildType.AddSocket,
                CodeType.Robot => PostBuildType.AddRobot,
                CodeType.WebSocket => PostBuildType.AddWebSocket,
                CodeType.Mqtt => PostBuildType.AddMqtt,
                CodeType.Task => PostBuildType.AddTask,
                CodeType.Web => PostBuildType.AddWeb,
                _ => PostBuildType.AddDll,
            };
            var pack = new BuildOBJ
            {
                User = App.Config.Name,
                Token = App.Config.Token,
                Mode = reType,
                UUID = name,
                Code = arg
            };
            HttpContent Content = new ByteArrayContent(AES(JsonConvert.SerializeObject(pack)));
            Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var temp = await httpClient.PostAsync(App.Config.Http, Content);
            var data = await temp.Content.ReadAsStringAsync();
            if (!CheckLogin(data))
            {
                return await Add(type, name);
            }
            return JsonConvert.DeserializeObject<ReMessage>(data);
        }
        catch
        {
            return null;
        }
    }

    public async Task<ReMessage> WebDownloadFile(WebObj obj, string name)
    {
        try
        {
            var pack = new BuildOBJ
            {
                User = App.Config.Name,
                Token = App.Config.Token,
                Mode = PostBuildType.WebDownloadFile,
                UUID = obj.UUID,
                Version = obj.Version,
                Text = obj.Text,
                Code = name
            };
            HttpContent Content = new ByteArrayContent(AES(JsonConvert.SerializeObject(pack)));
            Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var temp = await httpClient.PostAsync(App.Config.Http, Content);
            var data = await temp.Content.ReadAsStringAsync();
            if (!CheckLogin(data))
            {
                return await WebDownloadFile(obj, name);
            }
            return JsonConvert.DeserializeObject<ReMessage>(data);
        }
        catch
        {
            return null;
        }
    }

    public async Task<ReMessage> Remove(CodeType type, CSFileObj obj)
    {
        try
        {
            var reType = type switch
            {
                CodeType.Class => PostBuildType.RemoveClass,
                CodeType.Socket => PostBuildType.RemoveSocket,
                CodeType.Robot => PostBuildType.RemoveRobot,
                CodeType.WebSocket => PostBuildType.RemoveWebSocket,
                CodeType.Mqtt => PostBuildType.RemoveMqtt,
                CodeType.Task => PostBuildType.RemoveTask,
                CodeType.Web => PostBuildType.RemoveWeb,
                _ => PostBuildType.RemoveDll,
            };
            var pack = new BuildOBJ
            {
                User = App.Config.Name,
                Token = App.Config.Token,
                Mode = reType,
                UUID = obj.UUID
            };
            HttpContent Content = new ByteArrayContent(AES(JsonConvert.SerializeObject(pack)));
            Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var temp = await httpClient.PostAsync(App.Config.Http, Content);
            var data = await temp.Content.ReadAsStringAsync();
            if (!CheckLogin(data))
            {
                return await Remove(type, obj);
            }
            return JsonConvert.DeserializeObject<ReMessage>(data);
        }
        catch
        {
            return null;
        }
    }

    public async Task<CSFileCode> GetCode(CodeType type, string name)
    {
        try
        {
            var reType = type switch
            {
                CodeType.Class => PostBuildType.CodeClass,
                CodeType.Socket => PostBuildType.CodeSocket,
                CodeType.Robot => PostBuildType.CodeRobot,
                CodeType.WebSocket => PostBuildType.CodeWebSocket,
                CodeType.Mqtt => PostBuildType.CodeMqtt,
                CodeType.Task => PostBuildType.CodeTask,
                _ => PostBuildType.CodeDll,
            };
            var pack = new BuildOBJ
            {
                User = App.Config.Name,
                Token = App.Config.Token,
                Mode = reType,
                UUID = name
            };
            HttpContent Content = new ByteArrayContent(AES(JsonConvert.SerializeObject(pack)));
            Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var temp = await httpClient.PostAsync(App.Config.Http, Content);
            var data = await temp.Content.ReadAsStringAsync();
            if (data == "null")
            {
                return null;
            }
            if (!CheckLogin(data))
            {
                return await GetCode(type, name);
            }
            return JsonConvert.DeserializeObject<CSFileCode>(data);
        }
        catch
        {
            return null;
        }
    }
    public async Task<ReMessage> Build(CSFileObj obj, CodeType type, List<CodeEditObj> list)
    {
        try
        {
            var reType = type switch
            {
                CodeType.Class => PostBuildType.BuildClass,
                CodeType.Socket => PostBuildType.UpdataSocket,
                CodeType.Robot => PostBuildType.UpdataRobot,
                CodeType.WebSocket => PostBuildType.UpdataWebSocket,
                CodeType.Mqtt => PostBuildType.UpdataMqtt,
                CodeType.Task => PostBuildType.UpdataTask,
                _ => PostBuildType.UpdataDll,
            };
            var pack = new BuildOBJ
            {
                User = App.Config.Name,
                Token = App.Config.Token,
                Mode = reType,
                UUID = obj.UUID,
                Version = obj.Version,
                Text = obj.Text,
                Code = JsonConvert.SerializeObject(list)
            };
            HttpContent Content = new ByteArrayContent(AES(JsonConvert.SerializeObject(pack)));
            Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var temp = await httpClient.PostAsync(App.Config.Http, Content);
            var data = await temp.Content.ReadAsStringAsync();
            if (!CheckLogin(data))
            {
                return await Build(obj, type, list);
            }
            return JsonConvert.DeserializeObject<ReMessage>(data);
        }
        catch
        {
            return null;
        }
    }



    public async Task<APIFileObj> GetApi()
    {
        try
        {
            var pack = new BuildOBJ
            {
                User = App.Config.Name,
                Token = App.Config.Token,
                Mode = PostBuildType.GetApi
            };
            HttpContent Content = new ByteArrayContent(AES(JsonConvert.SerializeObject(pack)));
            Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var temp = await httpClient.PostAsync(App.Config.Http, Content);
            var data = await temp.Content.ReadAsStringAsync();
            if (!CheckLogin(data))
            {
                return await GetApi();
            }
            return JsonConvert.DeserializeObject<APIFileObj>(data);
        }
        catch
        {
            return null;
        }
    }

    public async Task<ReMessage> AddClassFile(CSFileObj obj, string file)
    {
        try
        {
            var pack = new BuildOBJ
            {
                User = App.Config.Name,
                Token = App.Config.Token,
                Mode = PostBuildType.AddClassFile,
                UUID = obj.UUID,
                Temp = file
            };
            HttpContent Content = new ByteArrayContent(AES(JsonConvert.SerializeObject(pack)));
            Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var temp = await httpClient.PostAsync(App.Config.Http, Content);
            var data = await temp.Content.ReadAsStringAsync();
            if (!CheckLogin(data))
            {
                return await AddClassFile(obj, file);
            }
            return JsonConvert.DeserializeObject<ReMessage>(data);
        }
        catch
        {
            return null;
        }
    }

    public async Task<ReMessage> RemoveClassFile(CSFileObj obj, string file)
    {
        try
        {
            var pack = new BuildOBJ
            {
                User = App.Config.Name,
                Token = App.Config.Token,
                Mode = PostBuildType.RemoveClassFile,
                UUID = obj.UUID,
                Temp = file
            };
            HttpContent Content = new ByteArrayContent(AES(JsonConvert.SerializeObject(pack)));
            Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var temp = await httpClient.PostAsync(App.Config.Http, Content);
            var data = await temp.Content.ReadAsStringAsync();
            if (!CheckLogin(data))
            {
                return await AddClassFile(obj, file);
            }
            return JsonConvert.DeserializeObject<ReMessage>(data);
        }
        catch
        {
            return null;
        }
    }

    public async Task<ClassCodeGetObj> GetClassCode(CSFileObj obj)
    {
        try
        {
            var pack = new BuildOBJ
            {
                User = App.Config.Name,
                Token = App.Config.Token,
                Mode = PostBuildType.CodeClass,
                UUID = obj.UUID
            };
            HttpContent Content = new ByteArrayContent(AES(JsonConvert.SerializeObject(pack)));
            Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var temp = await httpClient.PostAsync(App.Config.Http, Content);
            var data = await temp.Content.ReadAsStringAsync();
            if (!CheckLogin(data))
            {
                return await GetClassCode(obj);
            }
            return JsonConvert.DeserializeObject<ClassCodeGetObj>(data);
        }
        catch
        {
            return null;
        }
    }
}
