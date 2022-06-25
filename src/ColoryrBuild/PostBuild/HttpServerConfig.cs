using ColoryrWork.Lib.Build;
using ColoryrWork.Lib.Build.Object;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ColoryrBuild.PostBuild;

public partial class HttpUtils : HttpUtilsBase
{
    public async Task<RobotObj> GetRobotConfig()
    {
        try
        {
            var pack = new BuildOBJ
            {
                User = App.Config.Name,
                Token = App.Config.Token,
                Mode = PostBuildType.GetRobotConfig
            };
            HttpContent Content = new ByteArrayContent(AES(JsonConvert.SerializeObject(pack)));
            Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var temp = await httpClient.PostAsync(App.Config.Http, Content);
            var data = await temp.Content.ReadAsStringAsync();
            if (!CheckLogin(data))
            {
                return await GetRobotConfig();
            }
            return JsonConvert.DeserializeObject<RobotObj>(data);
        }
        catch
        {
            return null;
        }
    }
    public async Task<SocketObj> GetSocketConfig()
    {
        try
        {
            var pack = new BuildOBJ
            {
                User = App.Config.Name,
                Token = App.Config.Token,
                Mode = PostBuildType.GetServerSocketConfig
            };
            HttpContent Content = new ByteArrayContent(AES(JsonConvert.SerializeObject(pack)));
            Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var temp = await httpClient.PostAsync(App.Config.Http, Content);
            var data = await temp.Content.ReadAsStringAsync();
            if (!CheckLogin(data))
            {
                return await GetSocketConfig();
            }
            return JsonConvert.DeserializeObject<SocketObj>(data);
        }
        catch
        {
            return null;
        }
    }
    public async Task<HttpListObj> GetHttpConfigList()
    {
        try
        {
            var pack = new BuildOBJ
            {
                User = App.Config.Name,
                Token = App.Config.Token,
                Mode = PostBuildType.GetServerHttpConfigList
            };
            HttpContent Content = new ByteArrayContent(AES(JsonConvert.SerializeObject(pack)));
            Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var temp = await httpClient.PostAsync(App.Config.Http, Content);
            var data = await temp.Content.ReadAsStringAsync();
            if (!CheckLogin(data))
            {
                return await GetHttpConfigList();
            }
            return JsonConvert.DeserializeObject<HttpListObj>(data);
        }
        catch
        {
            return null;
        }
    }

    public async Task<ReMessage> AddHttpConfig(string ip, int port)
    {
        try
        {
            var pack = new BuildOBJ
            {
                User = App.Config.Name,
                Token = App.Config.Token,
                Mode = PostBuildType.AddServerHttpConfig,
                Code = ip,
                Version = port
            };
            HttpContent Content = new ByteArrayContent(AES(JsonConvert.SerializeObject(pack)));
            Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var temp = await httpClient.PostAsync(App.Config.Http, Content);
            var data = await temp.Content.ReadAsStringAsync();
            if (!CheckLogin(data))
            {
                return await AddHttpConfig(ip, port);
            }
            return JsonConvert.DeserializeObject<ReMessage>(data);
        }
        catch
        {
            return null;
        }
    }

    public async Task<ReMessage> RemoveHttpConfig(string ip, int port)
    {
        try
        {
            var pack = new BuildOBJ
            {
                User = App.Config.Name,
                Token = App.Config.Token,
                Mode = PostBuildType.RemoveServerHttpConfig,
                Code = ip,
                Version = port
            };
            HttpContent Content = new ByteArrayContent(AES(JsonConvert.SerializeObject(pack)));
            Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var temp = await httpClient.PostAsync(App.Config.Http, Content);
            var data = await temp.Content.ReadAsStringAsync();
            if (!CheckLogin(data))
            {
                return await RemoveHttpConfig(ip, port);
            }
            return JsonConvert.DeserializeObject<ReMessage>(data);
        }
        catch
        {
            return null;
        }
    }

    public async Task<ReMessage> AddHttpRoute(string key, RouteConfigObj obj)
    {
        try
        {
            var pack = new BuildOBJ
            {
                User = App.Config.Name,
                Token = App.Config.Token,
                Mode = PostBuildType.AddServerHttpRoute,
                Temp = key,
                Code = JsonUtils.ToString(obj)
            };
            HttpContent Content = new ByteArrayContent(AES(JsonConvert.SerializeObject(pack)));
            Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var temp = await httpClient.PostAsync(App.Config.Http, Content);
            var data = await temp.Content.ReadAsStringAsync();
            if (!CheckLogin(data))
            {
                return await AddHttpRoute(key, obj);
            }
            return JsonConvert.DeserializeObject<ReMessage>(data);
        }
        catch
        {
            return null;
        }
    }

    public async Task<ReMessage> RemoveHttpRoute(string key)
    {
        try
        {
            var pack = new BuildOBJ
            {
                User = App.Config.Name,
                Token = App.Config.Token,
                Mode = PostBuildType.RemoveServerHttpRoute,
                Code = key
            };
            HttpContent Content = new ByteArrayContent(AES(JsonConvert.SerializeObject(pack)));
            Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var temp = await httpClient.PostAsync(App.Config.Http, Content);
            var data = await temp.Content.ReadAsStringAsync();
            if (!CheckLogin(data))
            {
                return await RemoveHttpRoute(key);
            }
            return JsonConvert.DeserializeObject<ReMessage>(data);
        }
        catch
        {
            return null;
        }
    }

    public async Task<ReMessage> AddHttpUrlRoute(string key, RouteConfigObj obj)
    {
        try
        {
            var pack = new BuildOBJ
            {
                User = App.Config.Name,
                Token = App.Config.Token,
                Mode = PostBuildType.AddServerHttpUrlRoute,
                Temp = key,
                Code = JsonUtils.ToString(obj)
            };
            HttpContent Content = new ByteArrayContent(AES(JsonConvert.SerializeObject(pack)));
            Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var temp = await httpClient.PostAsync(App.Config.Http, Content);
            var data = await temp.Content.ReadAsStringAsync();
            if (!CheckLogin(data))
            {
                return await AddHttpUrlRoute(key, obj);
            }
            return JsonConvert.DeserializeObject<ReMessage>(data);
        }
        catch
        {
            return null;
        }
    }

    public async Task<ReMessage> RemoveHttpUrlRoute(string key)
    {
        try
        {
            var pack = new BuildOBJ
            {
                User = App.Config.Name,
                Token = App.Config.Token,
                Mode = PostBuildType.RemoveServerHttpUrlRoute,
                Code = key
            };
            HttpContent Content = new ByteArrayContent(AES(JsonConvert.SerializeObject(pack)));
            Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var temp = await httpClient.PostAsync(App.Config.Http, Content);
            var data = await temp.Content.ReadAsStringAsync();
            if (!CheckLogin(data))
            {
                return await RemoveHttpUrlRoute(key);
            }
            return JsonConvert.DeserializeObject<ReMessage>(data);
        }
        catch
        {
            return null;
        }
    }

    public async Task<ReMessage> SetServerEnable(bool enable, string type) 
    {
        try
        {
            var pack = new BuildOBJ
            {
                User = App.Config.Name,
                Token = App.Config.Token,
                Mode = PostBuildType.SetServerEnable,
                Code = enable.ToString(),
                Text = type
            };
            HttpContent Content = new ByteArrayContent(AES(JsonConvert.SerializeObject(pack)));
            Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var temp = await httpClient.PostAsync(App.Config.Http, Content);
            var data = await temp.Content.ReadAsStringAsync();
            if (!CheckLogin(data))
{
                return await SetServerEnable(enable, type);
            }
            return JsonConvert.DeserializeObject<ReMessage>(data);
        }
        catch
        {
            return null;
        }
    }

    public async Task Reboot()
    {
        try
        {
            var pack = new BuildOBJ
            {
                User = App.Config.Name,
                Token = App.Config.Token,
                Mode = PostBuildType.ServerReboot
            };
            HttpContent Content = new ByteArrayContent(AES(JsonConvert.SerializeObject(pack)));
            Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var temp = await httpClient.PostAsync(App.Config.Http, Content);
            var data = await temp.Content.ReadAsStringAsync();
            if (!CheckLogin(data))
            {
                await Reboot();
            }
        }
        catch
        {

        }
    }
}