using ColoryrBuild.Windows;
using ColoryrWork.Lib.Build.Object;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace ColoryrBuild.PostBuild;

public partial class HttpBuild : HttpUtilsBase
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
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = PostBuildType.Check
        });
        if (data == null)
            return false;
        ReMessage res = JsonConvert.DeserializeObject<ReMessage>(data);
        if (res?.Build == true)
        {
            return true;
        }
        else
        {
            _ = new InfoWindow("自动登录错误", $"自动登录错误:{res?.Message}");
        }
        return false;
    }
    /// <summary>
    /// 手动登录
    /// </summary>
    /// <param name="Pass">密钥</param>
    /// <returns>登录结果</returns>
    public async Task<bool> Login(string Pass)
    {
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Code = Pass,
            Mode = PostBuildType.Login
        });
        if (data == null)
            return false;
        ReMessage res = JsonConvert.DeserializeObject<ReMessage>(data);
        if (res?.Build == true)
        {
            App.Config.Token = res.Message;
            App.LogShow("登录", "登录成功");
            return true;
        }
        else
        {
            _ = new InfoWindow("登录错误", $"登录错误:{res.Message}");
        }
        return false;
    }
}
