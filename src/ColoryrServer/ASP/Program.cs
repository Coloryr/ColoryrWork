using ColoryrServer.DllManager;
using ColoryrServer.FileSystem;
using ColoryrServer.Http;
using ColoryrServer.SDK;
using HttpMultipartParser;
using ColoryrWork.Lib.Build;
using ColoryrWork.Lib.Build.Object;
using ColoryrWork.Lib.Server;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Newtonsoft.Json;
using HttpRequest = Microsoft.AspNetCore.Http.HttpRequest;
using HttpResponse = Microsoft.AspNetCore.Http.HttpResponse;
using ColoryrWork.Lib.App;

namespace ColoryrServer.ASP
{
    internal static class ASPServer
    {
        public static ASPConfig Config { get; set; }

        private static WebApplication Web;
        private static IHttpClients Clients;

        private const string https = "https";
        private const string http = "http";
        private static Dictionary<string, X509Certificate2> Ssls = new();
        private static X509Certificate2 DefaultSsl;

        public static void Main()
        {
            var builder = WebApplication.CreateBuilder();
            builder.Logging.ClearProviders();
            builder.Logging.AddProvider(new ColoryrLoggerProvider());

            ServerMain.ConfigUtil = new ASPConfigUtils();
            ServerMain.Start();

            if (Config.Ssl)
            {
                builder.Services.AddCertificateForwarding(options =>
                {
                    options.CertificateHeader = "X-SSL-CERT";
                    options.HeaderConverter = (headerValue) =>
                    {
                        if (!string.IsNullOrWhiteSpace(headerValue))
                        {
                            byte[] bytes = StringToByteArray(headerValue);
                            var clientCertificate = new X509Certificate2(bytes);
                            return clientCertificate;
                        }
                        return null;
                    };
                });
                ServerMain.LogOut("正在加载SSL证书");
                builder.WebHost.UseKestrel(options =>
                        options.ConfigureHttpsDefaults(i =>
                              i.ServerCertificateSelector = ASPServer.Ssl));
                foreach (var item in Config.Ssls)
                {
                    if (File.Exists(item.Value.SslLocal))
                    {
                        try
                        {
                            var ssl = new X509Certificate2(item.Value.SslLocal, item.Value.SslPassword);
                            if (item.Key == "default")
                                DefaultSsl = ssl;
                            else
                                Ssls.Add(item.Key, ssl);
                        }
                        catch (Exception e)
                        {
                            ServerMain.LogError(e);
                        }
                    }
                    else
                    {
                        ServerMain.LogError($"SSL证书找不到:{item.Value.SslLocal}");
                    }
                }
            }
            string[] urls = new string[ServerMain.Config.Http.Count];
            for (int a = 0; a < ServerMain.Config.Http.Count; a++)
            {
                var item = ServerMain.Config.Http[a];
                urls[a] = $"{(Config.Ssl ? https : http)}://{item.IP}:{item.Port}/";
                ServerMain.LogOut($"Http服务器监听{item.IP}:{item.Port}");
            }

            builder.WebHost.UseUrls(urls);

            builder.Services.AddHttpClient<HttpClients>();
            builder.Services.AddTransient<IHttpClients, HttpClients>();

            bool Run = true;
            if (!Config.NoInput)
                new Thread(() =>
                {
                    while (Run)
                    {
                        if (Run)
                            ServerMain.Command(Console.ReadLine());
                    }
                }).Start();

            Web = builder.Build();
            Clients = Web.Services.GetRequiredService<IHttpClients>();

            Web.MapGet("/", Config.RoteEnable ? RoteGetIndex : GetIndex);

            Web.MapGet("/{**name}", Config.RoteEnable ? RoteGetWeb : GetStatic);
            Web.MapPost("/{**name}", Config.RoteEnable ? RoteGetWeb : GetStatic);

            Web.MapGet(ServerMain.Config.Requset.WebAPI + "/{uuid}", GetBack);
            Web.MapGet(ServerMain.Config.Requset.WebAPI + "/{uuid}/{name}", GetBack);

            Web.MapPost("/", PostBuild);

            Web.MapPost(ServerMain.Config.Requset.WebAPI + "/{uuid}", POSTBack);
            Web.MapPost(ServerMain.Config.Requset.WebAPI + "/{uuid}/{name}", POSTBack);

            Web.MapGet(ServerMain.Config.Requset.Web + "/{**name}", GetWeb);
            Web.MapPost(ServerMain.Config.Requset.Web + "/{**name}", GetWeb);

            Web.Run();

            Run = false;
            ServerMain.LogOut("正在关闭服务器");
            ServerMain.Stop();
            ServerMain.LogOut("按下回车键退出");
        }

        public static X509Certificate2? Ssl(ConnectionContext? context, string? url)
        {
            if (url != null && Ssls.TryGetValue(url, out var item))
                return item;
            return DefaultSsl;
        }

        private static async Task PostBuild(HttpContext context)
        {
            HttpRequest Request = context.Request;
            HttpResponse Response = context.Response;
            if (Request.Headers[BuildKV.BuildK] == BuildKV.BuildV)
            {
                string Str;
                MemoryStream stream = new();
                await Request.Body.CopyToAsync(stream);
                if (Request.Headers[BuildKV.BuildK1] == "true")
                {
                    var receivedData = DeCode.AES256(stream.ToArray(), ServerMain.Config.AES.Key, ServerMain.Config.AES.IV);
                    Str = Encoding.UTF8.GetString(receivedData);
                }
                else
                {
                    Str = Encoding.UTF8.GetString(stream.ToArray());
                }
                JObject obj = JObject.Parse(Function.GetSrings(Str, "{"));
                var Json = obj.ToObject<BuildOBJ>();
                var List = ServerMain.Config.User.Where(a => a.Username == Json?.User);
                if (List.Any())
                {
                    var obj1 = DllBuild.StartBuild(Json, List.First()).Data;
                    await Response.WriteAsync(JsonConvert.SerializeObject(obj1));
                }
                else
                {
                    var obj1 = new ReMessage
                    {
                        Build = false,
                        Message = "账户或密码错误"
                    };
                    await Response.WriteAsync(JsonConvert.SerializeObject(obj1));
                }
            }
            else if (Request.Headers[APPKV.APPK] == APPKV.APPV)
            {
                string Str;
                MemoryStream stream = new();
                await Request.Body.CopyToAsync(stream);
                if (Request.Headers[BuildKV.BuildK1] == "true")
                {
                    var receivedData = DeCode.AES256(stream.ToArray(), ServerMain.Config.AES.Key, ServerMain.Config.AES.IV);
                    Str = Encoding.UTF8.GetString(receivedData);
                }
                else
                {
                    Str = Encoding.UTF8.GetString(stream.ToArray());
                }
                JObject obj = JObject.Parse(Function.GetSrings(Str, "{"));
                var Json = obj.ToObject<DownloadObj>();
                var httpReturn = AppDownload.Download(Json);
                switch (httpReturn.Res)
                {
                    case ResType.String:
                        await Response.WriteAsync(httpReturn.Data as string, httpReturn.Encoding);
                        break;
                    case ResType.Byte:
                        var bytes = httpReturn.Data as byte[];
                        await Response.BodyWriter.WriteAsync(bytes);
                        break;
                    case ResType.Json:
                        var obj1 = httpReturn.Data;
                        await Response.WriteAsync(JsonConvert.SerializeObject(obj1));
                        break;
                    case ResType.Stream:
                        var stream1 = httpReturn.Data as Stream;
                        if (stream1 == null)
                        {
                            Response.StatusCode = 500;
                            await Response.WriteAsync("stream in null", httpReturn.Encoding);
                        }
                        else
                        {
                            stream1.Seek(httpReturn.Pos, SeekOrigin.Begin);
                            Response.StatusCode = 206;
                            await stream.CopyToAsync(Response.Body);
                        }
                        break;
                }
            }
            else
            {
                Response.ContentType = ServerContentType.HTML;
                await Response.BodyWriter.WriteAsync(HtmlUtils.BaseDir.HtmlIndex);
            }
        }

        private static string[] data1 = Array.Empty<string>();

        private static async Task RoteGetIndex(HttpContext context)
        {
            HttpRequest Request = context.Request;
            HttpResponse Response = context.Response;
            if (Config.UrlRotes.TryGetValue(Request.Host.Host, out var rote1))
            {
                await RoteDo(Request, data1, rote1, Response);
            }
            else
            {
                Response.ContentType = ServerContentType.HTML;
                await Response.BodyWriter.WriteAsync(HtmlUtils.BaseDir.HtmlIndex);
            }
        }

        private static async Task GetIndex(HttpContext context)
        {
            HttpResponse Response = context.Response;
            Response.ContentType = ServerContentType.HTML;
            await Response.BodyWriter.WriteAsync(HtmlUtils.BaseDir.HtmlIndex);
        }

        private static async Task GetWeb(HttpContext context)
        {
            HttpRequest Request = context.Request;
            HttpResponse Response = context.Response;
            HttpReturn httpReturn;
            var name = context.GetRouteValue("name") as string;
            if (name == null)
                return;
            var arg = name.Split('/');
            if (arg.Length == 2)
            {
                httpReturn = HttpStatic.Get(arg[0], arg[1]);
                Response.ContentType = httpReturn.ContentType;
                Response.StatusCode = httpReturn.ReCode;
                await Response.BodyWriter.WriteAsync(httpReturn.Data as byte[]);
            }
            else
            {
                httpReturn = HttpStatic.Get(name);
                Response.ContentType = httpReturn.ContentType;
                Response.StatusCode = httpReturn.ReCode;
                await Response.BodyWriter.WriteAsync(httpReturn.Data as byte[]);
            }
        }

        private static async Task GetBack(HttpContext context)
        {
            HttpRequest Request = context.Request;
            HttpResponse Response = context.Response;
            HttpReturn httpReturn;
            var uuid = context.GetRouteValue("uuid") as string;
            var name = context.GetRouteValue("name") as string;
            var Dll = DllStonge.GetDll(uuid);
            if (Dll != null)
            {
                var Temp = new Dictionary<string, dynamic>();
                if (Request.QueryString.HasValue)
                {
                    var b = Request.QueryString.ToUriComponent()[1..];
                    foreach (string a in b.Split('&'))
                    {
                        var item = a.Split("=");
                        Temp.Add(item[0], item[1]);
                    }
                }
                NameValueCollection collection = new();
                foreach (var item in Request.Headers)
                {
                    collection.Add(item.Key, item.Value);
                }
                httpReturn = DllRun.DllGo(Dll, new()
                {
                    Cookie = ASPHttpUtils.HaveCookie(Request.Headers.Cookie),
                    RowRequest = collection,
                    Parameter = Temp,
                    Method = Request.Method,
                    ContentType = MyContentType.XFormData
                }, name);
            }
            else
            {
                httpReturn = new()
                {
                    Data = HtmlUtils.BaseDir.Html404,
                    ContentType = ServerContentType.HTML,
                    ReCode = 200
                };
            }
            Response.ContentType = httpReturn.ContentType;
            Response.StatusCode = httpReturn.ReCode;
            if (httpReturn.Head != null)
                foreach (var Item in httpReturn.Head)
                {
                    Response.Headers.Add(Item.Key, Item.Value);
                }
            if (httpReturn.Cookie != null)
                Response.Cookies.Append("cs", httpReturn.Cookie);
            switch (httpReturn.Res)
            {
                case ResType.String:
                    await Response.WriteAsync(httpReturn.Data as string, httpReturn.Encoding);
                    break;
                case ResType.Byte:
                    var bytes = httpReturn.Data as byte[];
                    await Response.BodyWriter.WriteAsync(bytes);
                    break;
                case ResType.Json:
                    var obj1 = httpReturn.Data;
                    await Response.WriteAsync(JsonConvert.SerializeObject(obj1));
                    break;
                case ResType.Stream:
                    var stream = httpReturn.Data as Stream;
                    if (stream == null)
                    {
                        Response.StatusCode = 500;
                        await Response.WriteAsync("stream in null", httpReturn.Encoding);
                    }
                    else
                    {
                        stream.Seek(httpReturn.Pos, SeekOrigin.Begin);
                        Response.StatusCode = 206;
                        await stream.CopyToAsync(Response.Body);
                    }
                    break;
            }
        }

        private static async Task RoteDo(HttpRequest Request, string[] arg, Rote rote, HttpResponse Response, int start = 1)
        {
            HttpClient ProxyRequest = Clients.GetOne();
            HttpRequestMessage message = new();
            message.Method = new HttpMethod(Request.Method);
            string url = "";
            if (arg.Length >= start)
            {
                for (int a = start; a < arg.Length; a++)
                {
                    url += $"/{arg[a]}";
                }
            }

            message.RequestUri = new Uri($"{rote.Url}{url}");
            if (Request.Method is "POST")
                message.Content = new StreamContent(Request.Body);
            else
                message.Content = new StringContent("");

            foreach (var item in Request.Headers)
            {
                try
                {
                    if (item.Key.StartsWith("Content"))
                    {
                        message.Content.Headers.Add(item.Key, item.Value as IEnumerable<string>);
                    }
                    else
                        message.Headers.Add(item.Key, item.Value as IEnumerable<string>);
                }
                catch
                {

                }
            }
            foreach (var item in rote.Heads)
            {
                message.Headers.Add(item.Key, item.Value);
            }

            if (url.EndsWith(".php"))
            {
                var res = await ProxyRequest.SendAsync(message, HttpCompletionOption.ResponseHeadersRead);
                Response.StatusCode = (int)res.StatusCode;
                if (res.Content.Headers.ContentType != null)
                    Response.ContentType = res.Content.Headers.ContentType.ToString();

                foreach (var item in res.Headers)
                {
                    if (item.Key is "Transfer-Encoding")
                        continue;
                    StringValues values = new(item.Value.ToArray());
                    Response.Headers.Add(item.Key, values);
                }

                await res.Content.CopyToAsync(Response.Body);
            }
            else
            {
                var res = await ProxyRequest.SendAsync(message, HttpCompletionOption.ResponseHeadersRead);

                Response.StatusCode = (int)res.StatusCode;
                if (res.Content == null)
                {
                    ServerMain.LogError("Content is null");
                    return;
                }
                if (res.Content.Headers == null)
                {
                    ServerMain.LogError("Headers is null");
                    return;
                }
                if (res.Content.Headers.ContentType != null)
                {
                    Response.ContentType = res.Content.Headers.ContentType.ToString();
                }

                foreach (var item in res.Headers)
                {
                    StringValues values = new(item.Value.ToArray());
                    Response.Headers.Add(item.Key, values);
                }
                await HttpClientUtils.CopyToAsync(res.Content.ReadAsStream(), Response.Body, CancellationToken.None);
            }
        }

        private static async Task RoteGetWeb(HttpContext context)
        {
            HttpRequest Request = context.Request;
            HttpResponse Response = context.Response;
            HttpReturn httpReturn;
            var name = context.GetRouteValue("name") as string;
            if (name == null)
                return;
            var arg = name.Split('/');
            if (Config.UrlRotes.TryGetValue(Request.Host.Host, out var rote1))
            {
                await RoteDo(Request, arg, rote1, Response, 0);
            }
            else if (Config.Rotes.TryGetValue(arg[0], out var rote))
            {
                await RoteDo(Request, arg, rote, Response);
            }
            else if (arg.Length == 2)
            {
                httpReturn = HttpStatic.Get(arg[0], arg[1]);
                Response.ContentType = httpReturn.ContentType;
                Response.StatusCode = httpReturn.ReCode;
                await Response.BodyWriter.WriteAsync(httpReturn.Data as byte[]);
            }
            else
            {
                httpReturn = HttpStatic.Get(name);
                Response.ContentType = httpReturn.ContentType;
                Response.StatusCode = httpReturn.ReCode;
                await Response.BodyWriter.WriteAsync(httpReturn.Data as byte[]);
            }
        }

        private static async Task GetStatic(HttpContext context)
        {
            HttpRequest Request = context.Request;
            HttpResponse Response = context.Response;
            HttpReturn httpReturn;
            var name = context.GetRouteValue("name") as string;
            if (name == null)
                return;

            if (Config.Requset.Stream)
            {
                var a = name.LastIndexOf('.');
                if (a != -1)
                {
                    string type = name[a..];
                    if (Config.Requset.StreamType.Contains(type))
                    {
                        NameValueCollection collection = new();
                        foreach (var item in Request.Headers)
                        {
                            collection.Add(item.Key, item.Value);
                        }

                        var res = HttpStatic.GetStream(new()
                        {
                            Cookie = ASPHttpUtils.HaveCookie(Request.Headers.Cookie),
                            RowRequest = collection
                        }, name);

                        if (res == null)
                        {
                            Response.ContentType = ServerContentType.HTML;
                            Response.StatusCode = 200;
                            await Response.BodyWriter.WriteAsync(HtmlUtils.BaseDir.Html404);
                            return;
                        }

                        var stream = res.Data;
                        if (stream == null)
                        {
                            Response.StatusCode = 500;
                            await Response.WriteAsync("stream in null", Encoding.UTF8);
                        }
                        else
                        {
                            stream.Seek(res.Pos, SeekOrigin.Begin);
                            foreach (var item in res.Head)
                            {
                                Response.Headers.Add(item.Key, item.Value);
                            }
                            Response.StatusCode = 206;
                            Response.ContentType = ServerContentType.GetType(type);
                            await stream.CopyToAsync(Response.Body, 1024);
                        }

                        return;
                    }
                }
            }

            var arg = name.Split('/');
            httpReturn = HttpStatic.GetStatic(arg);
            Response.ContentType = httpReturn.ContentType;
            Response.StatusCode = httpReturn.ReCode;
            await Response.BodyWriter.WriteAsync(httpReturn.Data as byte[]);
        }

        private static async Task POSTBack(HttpContext context)
        {
            HttpRequest Request = context.Request;
            HttpResponse Response = context.Response;
            HttpReturn httpReturn;
            var uuid = context.GetRouteValue("uuid") as string;
            var name = context.GetRouteValue("name") as string;

            MyContentType type = MyContentType.XFormData;
            var temp = new Dictionary<string, dynamic>();
            if (Request.ContentType != null)
            {
                if (Request.ContentType is ServerContentType.POSTXFORM)
                {
                    type = MyContentType.XFormData;
                    foreach (var item in Request.Form)
                    {
                        temp.Add(item.Key, item.Value);
                    }
                    foreach (var item in Request.Form.Files)
                    {
                        temp.Add(item.Name, item);
                    }
                }
                else if (Request.ContentType.StartsWith(ServerContentType.POSTFORMDATA))
                {
                    try
                    {
                        var parser = await MultipartFormDataParser.ParseAsync(Request.Body);
                        foreach (var item in parser.Parameters)
                        {
                            temp.Add(item.Name, item.Data);
                        }
                        foreach (var item in parser.Files)
                        {
                            temp.Add(item.Name, new HttpMultipartFile()
                            {
                                Data = item.Data,
                                FileName = item.FileName,
                                ContentType = item.ContentType,
                                ContentDisposition = item.ContentDisposition
                            });
                        }
                        type = MyContentType.MFormData;
                    }
                    catch (Exception e)
                    {
                        ServerMain.LogError(e);
                        var obj1 = new GetMeesage
                        {
                            Res = 123,
                            Text = "表单解析发生错误，请检查数据"
                        };
                        Response.StatusCode = 500;
                        await Response.WriteAsync(JsonConvert.SerializeObject(obj1));
                        return;
                    }
                }
                else if (Request.ContentType.StartsWith(ServerContentType.JSON))
                {
                    MemoryStream stream = new();
                    await Request.Body.CopyToAsync(stream);
                    var Str = Encoding.UTF8.GetString(stream.ToArray());
                    JObject obj = JObject.Parse(Function.GetSrings(Str, "{"));
                    foreach (var item in obj)
                    {
                        temp.Add(item.Key, item.Value);
                    }
                    type = MyContentType.Json;
                }
                else
                {
                    type = MyContentType.Other;
                }
            }

            var Dll = DllStonge.GetDll(uuid);
            if (Dll != null)
            {
                NameValueCollection collection = new();
                foreach (var item in Request.Headers)
                {
                    collection.Add(item.Key, item.Value);
                }

                httpReturn = DllRun.DllGo(Dll, new()
                {
                    Cookie = ASPHttpUtils.HaveCookie(Request.Headers.Cookie),
                    Parameter = temp,
                    RowRequest = collection,
                    ContentType = type,
                    Method = Request.Method,
                    Stream = type == MyContentType.Other ? Request.Body : null
                }, name);
            }
            else
            {
                httpReturn = new HttpReturn
                {
                    Data = HtmlUtils.BaseDir.Html404,
                    ContentType = ServerContentType.HTML
                };
            }
            Response.ContentType = httpReturn.ContentType;
            Response.StatusCode = httpReturn.ReCode;
            if (httpReturn.Head != null)
                foreach (var Item in httpReturn.Head)
                {
                    Response.Headers.Add(Item.Key, Item.Value);
                }
            if (httpReturn.Cookie != null)
                Response.Cookies.Append("cs", httpReturn.Cookie);
            switch (httpReturn.Res)
            {
                case ResType.String:
                    await Response.WriteAsync(httpReturn.Data as string, httpReturn.Encoding);
                    break;
                case ResType.Byte:
                    var bytes = httpReturn.Data as byte[];
                    await Response.BodyWriter.WriteAsync(bytes);
                    break;
                case ResType.Json:
                    var obj1 = httpReturn.Data;
                    await Response.WriteAsync(JsonConvert.SerializeObject(obj1));
                    break;
                case ResType.Stream:
                    var stream = httpReturn.Data as Stream;
                    if (stream == null)
                    {
                        Response.StatusCode = 500;
                        await Response.WriteAsync("stream in null", httpReturn.Encoding);
                    }
                    else
                    {
                        stream.Seek(httpReturn.Pos, SeekOrigin.Begin);
                        Response.StatusCode = 206;
                        await stream.CopyToAsync(Response.Body);
                    }
                    break;
            }
        }
        private static byte[] StringToByteArray(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];

            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return bytes;
        }
    }
}

