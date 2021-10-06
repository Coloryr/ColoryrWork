using ColoryrServer.DataBase;
using ColoryrServer.DllManager;
using ColoryrServer.DllManager.StartGen.GenUtils;
using ColoryrServer.FileSystem;
using ColoryrServer.Html;
using ColoryrServer.Http;
using ColoryrServer.IoT;
using ColoryrServer.MQTT;
using ColoryrServer.Robot;
using ColoryrServer.SDK;
using HttpRequest = Microsoft.AspNetCore.Http.HttpRequest;
using HttpResponse = Microsoft.AspNetCore.Http.HttpResponse;

namespace ColoryrServer.ASP
{
    internal class ASPServer
    {
        private static WebApplication Web;

        public static void Main()
        {
            var builder = WebApplication.CreateBuilder();
            Web = builder.Build();

            ServerMain.Start();

            //Web.UseHttpsRedirection();
            //Web.UseRouting();

            Web.MapGet("/", Get);
            Web.MapGet("/{name}", Get);
            Web.MapGet("/{uuid}/{name}", Get1);
            Web.MapGet(ServerMain.Config.Requset.WebAPI + "/{uuid}", GetBack);
            Web.MapGet(ServerMain.Config.Requset.WebAPI + "/{uuid}/{name}", GetBack);

            Web.MapPost("/", Get);
            Web.MapPost("/{name}", Get);
            Web.MapPost("/{uuid}/{name}", Get1);

            Web.MapPost(ServerMain.Config.Requset.WebAPI + "/{uuid}", POST);
            Web.MapPost(ServerMain.Config.Requset.WebAPI + "/{uuid}/{name}", POST);

            foreach (var item in ServerMain.Config.Http)
            {
                Web.Urls.Add("http://" + item.IP + ":" +
                item.Port + "/");
                ServerMain.LogOut($"Http·þÎñÆ÷¼àÌý{item.IP}:{item.Port}");
            }

            Web.Run();
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

        private static async Task Get(HttpContext context)
        {
            HttpRequest Request = context.Request;
            HttpResponse Response = context.Response;
            HttpReturn httpReturn;
            httpReturn = HttpStatic.Get(Request.Path);
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

        private static async Task Get1(HttpContext context)
        {
            HttpRequest Request = context.Request;
            HttpResponse Response = context.Response;
            HttpReturn httpReturn;
            var uuid = context.GetRouteValue("uuid") as string;
            var name = context.GetRouteValue("name") as string;
            httpReturn = HttpStatic.Get(uuid, name);
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

        private static async Task POST(HttpContext context)
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

    