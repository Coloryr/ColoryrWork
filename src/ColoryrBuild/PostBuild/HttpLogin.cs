using ColoryrBuild.Windows;
using ColoryrWork.Lib.Build.Object;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ColoryrBuild.PostBuild;

public partial class HttpUtils : HttpUtilsBase
{
    /// <summary>
    /// 检查登录
    /// </summary>
    /// <param name="data">服务器数据</param>
    /// <returns>是否有效</returns>
    public bool CheckLogin(string data)
    {
        var obj = JObject.Parse(data);
        if (obj.ContainsKey("Build") && obj.ContainsKey("Message"))
        {
            var item1 = obj["Build"].ToString();
            var item2 = obj["Message"].ToString();
            if (item1 == "False" && item2 == "233")
            {
                _ = new InfoWindow("自动登录", "自动登录失效");
                App.Login();
                return false;
            }
        }
        return true;
    }
    /// <summary>
    /// 自动登录
    /// </summary>
    /// <returns>登录结果</returns>
    public async Task<bool> AutoLogin()
    {
        try
        {
            var pack = new BuildOBJ
            {
                User = App.Config.Name,
                Token = App.Config.Token,
                Mode = ReType.Check
            };

            HttpContent Content = new ByteArrayContent(AES(JsonConvert.SerializeObject(pack)));
            Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var temp = await httpClient.PostAsync(App.Config.Http, Content);
            var data = await temp.Content.ReadAsStringAsync();
            ReMessage res = JsonConvert.DeserializeObject<ReMessage>(data);
            if (res?.Build == true)
            {
                return true;
            }
            else
            {
                App.LogShow("登录", "自动登录失败");
            }
            return false;
        }
        catch (Exception e)
        {
            _ = new InfoWindow("自动登录错误", e.ToString());
            return false;
        }
    }
    /// <summary>
    /// 手动登录
    /// </summary>
    /// <param name="Pass">密钥</param>
    /// <returns>登录结果</returns>
    public async Task<bool> Login(string Pass)
    {
        try
        {
            var pack = new BuildOBJ
            {
                User = App.Config.Name,
                Code = Pass,
                Mode = ReType.Login
            };
            HttpContent Content = new ByteArrayContent(AES(JsonConvert.SerializeObject(pack)));
            Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var temp = await httpClient.PostAsync(App.Config.Http, Content);
            var data = await temp.Content.ReadAsStringAsync();
            ReMessage res = JsonConvert.DeserializeObject<ReMessage>(data);
            if (res.Build == true)
            {
                App.Config.Token = res.Message;
                App.LogShow("登录", "登录成功");
                return true;
            }
            else
            {
                _ = new InfoWindow("登录错误", res.Message);
            }
            return false;
        }
        catch
        {
            return false;
        }
    }
}
