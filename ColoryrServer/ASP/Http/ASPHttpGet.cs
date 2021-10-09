using ColoryrServer.DllManager;
using ColoryrServer.FileSystem;
using ColoryrServer.Http;
using ColoryrServer.SDK;
using System.Collections.Specialized;
using HttpRequest = ColoryrServer.SDK.HttpRequest;

namespace ColoryrServer.ASP
{
    public class ASPHttpGet
    {
        public static HttpReturn HttpGET(string Url, IHeaderDictionary Hashtable, QueryString Data, string uuid, string name = "")
        {
            if (Url.StartsWith("//"))
            {
                Url = Url[1..];
            }
            var Dll = DllStonge.GetDll(uuid);
            if (Dll != null)
            {
                var Temp = new Dictionary<string, dynamic>();
                if (Data.HasValue)
                {
                    foreach (string a in Data.Value.Split('&'))
                    {
                        var item = a.Split("=");
                        Temp.Add(item[0], item[1]);
                    }
                }
                NameValueCollection collection = new();
                foreach (var item in Hashtable)
                {
                    collection.Add(item.Key, item.Value);
                }
                var Http = new HttpRequest
                {
                    Cookie = ASPHttpUtils.HaveCookie(Hashtable),
                    RowRequest = collection,
                    Parameter = Temp,
                    ContentType = MyContentType.XFormData
                };
                var Data1 = DllRun.DllGo(Dll, Http, name);
                return Data1;
            }

            return new HttpReturn
            {
                Data = HtmlUtils.Html404,
                ContentType = ServerContentType.HTML,
                ReCode = 200
            };
        }
    }
}
