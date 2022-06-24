using ColoryrWork.Lib.Build;
using ColoryrWork.Lib.Build.Object;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ColoryrBuild.PostBuild;

public partial class HttpUtils : HttpUtilsBase
{
    public async Task<SocketObj> GetSocketConfig()
    {
        try
        {
            var pack = new BuildOBJ
            {
                User = App.Config.Name,
                Token = App.Config.Token,
                Mode = ReType.GetServerSocketConfig
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
                Mode = ReType.GetServerHttpConfigList
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
                Mode = ReType.AddServerHttpConfig,
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
                Mode = ReType.RemoveServerHttpConfig,
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
                Mode = ReType.AddServerHttpRoute,
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
                Mode = ReType.AddServerHttpRoute,
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
                Mode = ReType.AddServerHttpUrlRoute,
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
                Mode = ReType.RemoveServerHttpUrlRoute,
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

    public async Task Reboot()
    {
        try
        {
            var pack = new BuildOBJ
            {
                User = App.Config.Name,
                Token = App.Config.Token,
                Mode = ReType.ServerReboot
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