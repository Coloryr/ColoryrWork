using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ColoryrWork.Lib.Build.Object;

namespace ColoryrBuild.PostBuild;

public partial class HttpUtils : HttpUtilsBase
{
    public async Task<HttpListObj> GetHttpList()
    {
        try
        {
            var pack = new BuildOBJ
            {
                User = App.Config.Name,
                Token = App.Config.Token,
                Mode = ReType.GetServerHttpList
            };
            HttpContent Content = new ByteArrayContent(AES(JsonConvert.SerializeObject(pack)));
            Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var temp = await httpClient.PostAsync(App.Config.Http, Content);
            var data = await temp.Content.ReadAsStringAsync();
            if (!CheckLogin(data))
            {
                return await GetHttpList();
            }
            return JsonConvert.DeserializeObject<HttpListObj>(data);
        }
        catch
        {
            return null;
        }
    }
}