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
                CodeType.Class => ReType.GetClass,
                CodeType.Socket => ReType.GetSocket,
                CodeType.Robot => ReType.GetRobot,
                CodeType.WebSocket => ReType.GetWebSocket,
                CodeType.Mqtt => ReType.GetMqtt,
                CodeType.Task => ReType.GetTask,
                CodeType.Web => ReType.GetWeb,
                CodeType.Dll => ReType.GetDll,
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
                Mode = ReType.UpdataClass,
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
                CodeType.Class => ReType.AddClass,
                CodeType.Socket => ReType.AddSocket,
                CodeType.Robot => ReType.AddRobot,
                CodeType.WebSocket => ReType.AddWebSocket,
                CodeType.Mqtt => ReType.AddMqtt,
                CodeType.Task => ReType.AddTask,
                CodeType.Web => ReType.AddWeb,
                _ => ReType.AddDll,
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
                Mode = ReType.WebDownloadFile,
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
                CodeType.Class => ReType.RemoveClass,
                CodeType.Socket => ReType.RemoveSocket,
                CodeType.Robot => ReType.RemoveRobot,
                CodeType.WebSocket => ReType.RemoveWebSocket,
                CodeType.Mqtt => ReType.RemoveMqtt,
                CodeType.Task => ReType.RemoveTask,
                CodeType.Web => ReType.RemoveWeb,
                _ => ReType.RemoveDll,
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
                CodeType.Class => ReType.CodeClass,
                CodeType.Socket => ReType.CodeSocket,
                CodeType.Robot => ReType.CodeRobot,
                CodeType.WebSocket => ReType.CodeWebSocket,
                CodeType.Mqtt => ReType.CodeMqtt,
                CodeType.Task => ReType.CodeTask,
                _ => ReType.CodeDll,
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
                CodeType.Class => ReType.BuildClass,
                CodeType.Socket => ReType.UpdataSocket,
                CodeType.Robot => ReType.UpdataRobot,
                CodeType.WebSocket => ReType.UpdataWebSocket,
                CodeType.Mqtt => ReType.UpdataMqtt,
                CodeType.Task => ReType.UpdataTask,
                _ => ReType.UpdataDll,
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
                Mode = ReType.GetApi
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
                Mode = ReType.AddClassFile,
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
                Mode = ReType.RemoveClassFile,
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
                Mode = ReType.CodeClass,
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
