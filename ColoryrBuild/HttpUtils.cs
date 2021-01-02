using Lib.Build;
using Lib.Build.Object;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ColoryrBuild
{
    public class HttpUtils
    {
        private HttpClient httpClient;
        public HttpUtils()
        {
            var Handler = new HttpClientHandler();
            httpClient = new(Handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            httpClient.DefaultRequestHeaders.Add(BuildKV.BuildK, BuildKV.BuildV);
        }

        public async Task<bool> Login()
        {
            var pack = new BuildOBJ
            {
                User = App.Config.Name,
                Token = App.Config.Token,
                Mode = ReType.Login
            };
            HttpContent Content = new StringContent(JsonConvert.SerializeObject(pack));
            Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var temp = await httpClient.PostAsync(App.Config.Http, Content);
            var data = await temp.Content.ReadAsStringAsync();
            ReMessage res = JsonConvert.DeserializeObject<ReMessage>(data);
            if (res.Build == true)
            {
                if (App.Config.SaveToken)
                {
                    App.Config.Token = res.Message;
                }
                App.ShowA("登录", "登录成功");
                return true;
            }
            else
            {
                App.ShowB("登录", res.Message);
            }
            return false;
        }
    }
}
