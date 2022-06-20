using ColoryrServer.Core;
using ColoryrServer.Core.FileSystem;
using ColoryrServer.Core.FileSystem.Html;
using ColoryrServer.Core.Http;
using ColoryrServer.SDK;
using ColoryrWork.Lib.Build;
using ColoryrWork.Lib.Build.Object;
using ColoryrWork.Lib.Server;
using HttpMultipartParser;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using HttpRequest = Microsoft.AspNetCore.Http.HttpRequest;
using HttpResponse = Microsoft.AspNetCore.Http.HttpResponse;

namespace ColoryrServer.ASP;

internal static class ASPServer
{
    internal static ASPConfig Config { get; set; }
    internal static IHttpClients Clients;

    private static WebApplication Web;

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
        string[] urls = new string[Config.Http.Count];
        for (int a = 0; a < Config.Http.Count; a++)
        {
            var item = Config.Http[a];
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

        Web.MapGet("/", Config.RoteEnable ? HttpGet.RoteGetIndex : HttpGet.GetIndex);

        Web.MapGet("/{**name}", Config.RoteEnable ? HttpGet.RouteGet : HttpGet.Get);
        //Web.MapPost("/{**name}", Config.RoteEnable ? RoteGetWeb : GetStatic);

        Web.MapPost("/", PostBuild);

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
                var obj1 = Core.DllManager.PostBuild.PostBuild.StartBuild(Json, List.First<Core.FileSystem.UserConfig>()).Data;
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
        else
        {
            Response.ContentType = ServerContentType.HTML;
            await Response.BodyWriter.WriteAsync(WebBinManager.BaseDir.HtmlIndex);
        }
    }

    private static async Task POSTBack(HttpContext context)
    {
        HttpRequest Request = context.Request;
        HttpResponse Response = context.Response;
        HttpReturn httpReturn = null;
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

        var Dll = DllStongeManager.GetDll(uuid);
        if (Dll != null)
        {
            NameValueCollection collection = new();
            foreach (var item in Request.Headers)
            {
                collection.Add(item.Key, item.Value);
            }

            //httpReturn = DllRun.DllGo(Dll, new()
            //{
            //    Cookie = ASPHttpUtils.HaveCookie(Request.Headers.Cookie),
            //    Parameter = temp,
            //    RowRequest = collection,
            //    ContentType = type,
            //    Method = Request.Method,
            //    Stream = type == MyContentType.Other ? Request.Body : null
            //}, name);
        }
        else
        {
            httpReturn = new HttpReturn
            {
                Data = WebBinManager.BaseDir.Html404,
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
            foreach (var item in httpReturn.Cookie)
            {
                Response.Cookies.Append(item.Key, item.Value);
            }
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

