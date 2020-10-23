using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;

namespace SendBuild
{
    class Http
    {
        private static HttpClient Client = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(10)
        };
        public static async Task<JObject> GetAsync(object data)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(App.Config.Url))
                {
                    return null;
                }
                if (App.Config.Url.StartsWith("https://"))
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                }
                else
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                HttpContent Content = new StringContent(JsonConvert.SerializeObject(data));
                Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage Response = await Client.PostAsync(App.Config.Url, Content);
                Response.EnsureSuccessStatusCode();//用来抛异常的
                var obj = JObject.Parse(await Response.Content.ReadAsStringAsync());
                if (obj.ContainsKey("res") && obj["res"].ToString() == "123")
                {
                    MessageBox.Show(obj["data"].ToString(), obj["text"].ToString());
                    return null;
                }
                return obj;
            }
            catch
            {
                App.Show("编译服务器错误");
                return null;
            }
        }
    }
}
