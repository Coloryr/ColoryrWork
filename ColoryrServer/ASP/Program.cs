using ColoryrServer.FileSystem;
using ColoryrServer.Http;
using ColoryrServer.SDK;
using Newtonsoft.Json.Linq;
using ColoryrServer.DllManager;
using Lib.App;
using Lib.Build;
using Lib.Build.Object;
using Lib.Server;
using System.Text;
using HttpRequest = Microsoft.AspNetCore.Http.HttpRequest;
using HttpResponse = Microsoft.AspNetCore.Http.HttpResponse;

namespace ColoryrServer.ASP
{
    internal class ASPServer
    {
        public static ASPConfig Config { get; set; }

        private static WebApplication Web;

        public static void Main()
        {
            var builder = WebApplication.CreateBuilder();
            Web = builder.Build();

            ServerMain.ConfigUtil = new ASPConfigUtils();
            ServerMain.Start();

            //Web.UseHttpsRedirection();
            //Web.UseRouting();

            Web.MapGet("/", GetIndex);
            Web.MapGet("/{name}", GetWeb);
            Web.MapGet("/{uuid}/{name}", GetGetWeb1);
            Web.MapGet(ServerMain.Config.Requset.WebAPI + "/{uuid}", GetBack);
            Web.MapGet(ServerMain.Config.Requset.WebAPI + "/{uuid}/{name}", GetBack);

            Web.MapPost("/", PostBuild);
            Web.MapPost("/{name}", GetWeb);
            Web.MapPost("/{uuid}/{name}", GetGetWeb1);
            Web.MapPost(ServerMain.Config.Requset.WebAPI + "/{uuid}", POSTBack);
            Web.MapPost(ServerMain.Config.Requset.WebAPI + "/{uuid}/{name}", POSTBack);

            foreach (var item in ServerMain.Config.Http)
            {
                Web.Urls.Add("http://" + item.IP + ":" +
                item.Port + "/");
                ServerMain.LogOut($"Http服务器监听{item.IP}:{item.Port}");
            }

            Web.Run();

            ServerMain.LogOut("正在关闭服务器");
            ServerMain.Stop();
        }

        private static async Task PostBuild(HttpContext context)
        {
            HttpRequest Request = context.Request;
            HttpResponse Response = context.Response;
            HttpReturn httpReturn;
            if (Request.Headers[BuildKV.BuildK] == BuildKV.BuildV)
            {
                MemoryStream memoryStream = new();
                var data = new byte[2000000];
                long la = (long)Request.ContentLength;
                string Str;
                while (la > 0)
                {
                    int a = await Request.Body.ReadAsync(data);
                    la -= a;
                    memoryStream.Write(data, 0, a);
                }
                if (Request.Headers[BuildKV.BuildK1] == "true")
                {
                    var receivedData = DeCode.AES256(memoryStream.ToArray(), ServerMain.Config.AES.Key, ServerMain.Config.AES.IV);
                    Str = Encoding.UTF8.GetString(receivedData);
                }
                else
                {
                    Str = Encoding.UTF8.GetString(memoryStream.ToArray());
                }
                JObject obj = JObject.Parse(Function.GetSrings(Str, "{"));
                var Json = obj.ToObject<BuildOBJ>();
                var List = ServerMain.Config.User.Where(a => a.Username == Json.User);
                if (List.Any())
                {
                    await Response.BodyWriter.WriteAsync(DllBuild.StartBuild(Json, List.First()).Data);
                }
                else
                {
                    await Response.WriteAsJsonAsync(new ReMessage
                    {
                        Build = false,
                        Message = "账户错误"
                    });
                }
            }
            else if (Request.Headers[APPKV.APPK] == APPKV.APPV)
            {
                MemoryStream memoryStream = new();
                var data = new byte[2000000];
                long la = (long)Request.ContentLength;
                string Str;
                while (la > 0)
                {
                    int a = await Request.Body.ReadAsync(data);
                    la -= a;
                    memoryStream.Write(data, 0, a);
                }
                if (Request.Headers[BuildKV.BuildK1] == "true")
                {
                    var receivedData = DeCode.AES256(memoryStream.ToArray(), ServerMain.Config.AES.Key, ServerMain.Config.AES.IV);
                    Str = Encoding.UTF8.GetString(receivedData);
                }
                else
                {
                    Str = Encoding.UTF8.GetString(memoryStream.ToArray());
                }
                JObject obj = JObject.Parse(Function.GetSrings(Str, "{"));
                var Json = obj.ToObject<DownloadObj>();
                await Response.BodyWriter.WriteAsync(AppDownload.Download(Json).Data);
            }
            else
            {
                Response.ContentType = ServerContentType.HTML;
                await Response.BodyWriter.WriteAsync(HtmlUtils.HtmlIndex);
            }
        }

        private static async Task GetIndex(HttpContext context)
        {
            HttpResponse Response = context.Response;
            Response.ContentType = ServerContentType.HTML;
            await Response.BodyWriter.WriteAsync(HtmlUtils.HtmlIndex);
        }

        private static async Task GetBack(HttpContext context)
        {
            HttpRequest Request = context.Request;
            HttpResponse Response = context.Response;
            HttpReturn httpReturn;
            var uuid = context.GetRouteValue("uuid") as string;
            var name = context.GetRouteValue("name") as string;
            httpReturn = ASPHttpGet.HttpGET(Request.Path, Request.Headers, Request.QueryString, uuid, name);
            Response.ContentType = httpReturn.ContentType;
            Response.StatusCode = httpReturn.ReCode;
            if (httpReturn.Head != null)
                foreach (var Item in httpReturn.Head)
                {
                    Response.Headers.Add(Item.Key, Item.Value);
                }
            if (httpReturn.Cookie != null)
                Response.Cookies.Append("cs", httpReturn.Cookie);
            if (httpReturn.Data1 == null)
                await Response.BodyWriter.WriteAsync(httpReturn.Data);
            else
            {
                Response.ContentLength = httpReturn.Data1.Length;
                httpReturn.Data1.Seek(httpReturn.Pos, SeekOrigin.Begin);
                Response.StatusCode = 206;
                await httpReturn.Data1.CopyToAsync(Response.Body);
            }
        }

        private static async Task GetWeb(HttpContext context)
        {
            HttpResponse Response = context.Response;
            HttpReturn httpReturn;
            var name = context.GetRouteValue("name") as string;
            httpReturn = HttpStatic.Get(name);
            Response.ContentType = httpReturn.ContentType;
            Response.StatusCode = httpReturn.ReCode;
            await Response.BodyWriter.WriteAsync(httpReturn.Data);
        }

        private static async Task GetGetWeb1(HttpContext context)
        {
            HttpResponse Response = context.Response;
            HttpReturn httpReturn;
            var uuid = context.GetRouteValue("uuid") as string;
            var name = context.GetRouteValue("name") as string;
            httpReturn = HttpStatic.Get(uuid, name);
            Response.ContentType = httpReturn.ContentType;
            Response.StatusCode = httpReturn.ReCode;
            await Response.BodyWriter.WriteAsync(httpReturn.Data);
        }

        private static async Task POSTBack(HttpContext context)
        {
            HttpRequest Request = context.Request;
            HttpResponse Response = context.Response;
            HttpReturn httpReturn;
            var uuid = context.GetRouteValue("uuid") as string;
            var name = context.GetRouteValue("name") as string;

            MyContentType type = MyContentType.Other;
            if (Request.ContentType != null)
            {
                if (Request.ContentType.StartsWith(ServerContentType.POSTXFORM))
                {
                    type = MyContentType.XFormData;
                }
                else if (Request.ContentType.StartsWith(ServerContentType.POSTFORMDATA))
                {
                    type = MyContentType.MFormData;
                }
                else if (Request.ContentType.StartsWith(ServerContentType.JSON))
                {
                    type = MyContentType.Json;
                }
                else
                {
                    type = MyContentType.Other;
                }
            }

            httpReturn = await ASPHttpPOST.HttpPOST(Request.Body, (long)Request.ContentLength, Request.Path, Request.Headers, type, uuid, name);
            Response.ContentType = httpReturn.ContentType;
            Response.StatusCode = httpReturn.ReCode;
            if (httpReturn.Head != null)
                foreach (var Item in httpReturn.Head)
                {
                    Response.Headers.Add(Item.Key, Item.Value);
                }
            if (httpReturn.Cookie != null)
                Response.Cookies.Append("cs", httpReturn.Cookie);
            if (httpReturn.Data1 == null)
                await Response.BodyWriter.WriteAsync(httpReturn.Data);
            else
            {
                Response.ContentLength = httpReturn.Data1.Length;
                httpReturn.Data1.Seek(httpReturn.Pos, SeekOrigin.Begin);
                Response.StatusCode = 206;
                await httpReturn.Data1.CopyToAsync(Response.Body);
            }
        }
    }
}

