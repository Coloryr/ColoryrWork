using ColoryrServer.DllManager;
using ColoryrServer.FileSystem;
using ColoryrServer.Http;
using ColoryrServer.SDK;
using ColoryrServer.Utils;
using HttpMultipartParser;
using Lib.App;
using Lib.Build;
using Lib.Build.Object;
using Lib.Server;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using HttpRequest = Microsoft.AspNetCore.Http.HttpRequest;
using HttpResponse = Microsoft.AspNetCore.Http.HttpResponse;

namespace ColoryrServer.ASP
{
    class ColoryrLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
                            Exception exception, Func<TState, Exception, string> formatter)
        {
            if (eventId.Id == 100 || eventId.Id == 101)
                return;
            if (logLevel is LogLevel.Warning or LogLevel.Error)
                ServerMain.LogError($"{logLevel}-{eventId.Id} {formatter(state, exception)}");
            else
                ServerMain.LogOut($"{logLevel}-{eventId.Id} {formatter(state, exception)}");
        }
    }

    class ColoryrLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, ColoryrLogger> _loggers = new();

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new ColoryrLogger());
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }

    internal class ASPServer
    {
        public static ASPConfig Config { get; set; }

        private static WebApplication Web;
        private static IHttpClients Clients;

        private const string https = "https";
        private const string http = "http";

        public static void Main()
        {
            var builder = WebApplication.CreateBuilder();
            builder.Logging.ClearProviders();
            builder.Logging.AddProvider(new ColoryrLoggerProvider());

            ServerMain.ConfigUtil = new ASPConfigUtils();
            ServerMain.Start();

            //Web.UseRouting();

            if (Config.Ssl)
            {
                builder.Services.AddCertificateForwarding(options =>
                {
                    options.CertificateHeader = "X-SSL-CERT";
                    options.HeaderConverter = (headerValue) =>
                    {
                        X509Certificate2 clientCertificate = null;

                        if (!string.IsNullOrWhiteSpace(headerValue))
                        {
                            byte[] bytes = StringToByteArray(headerValue);
                            clientCertificate = new X509Certificate2(bytes);
                        }

                        return clientCertificate;
                    };
                });
                ServerMain.LogOut("正在加载SSL证书");
                if (File.Exists(Config.SslLocal))
                {
                    builder.WebHost.UseKestrel(options =>
                    {
                        options.ConfigureHttpsDefaults(i =>
                        {
                            try
                            {
                                i.ServerCertificate = new X509Certificate2(Config.SslLocal, Config.SslPassword);
                            }
                            catch (Exception e)
                            {
                                ServerMain.LogError(e);
                            }
                        });
                    });
                }
                else
                {
                    ServerMain.LogError("SSL证书找不到");
                }
            }
            foreach (var item in ServerMain.Config.Http)
            {
                builder.WebHost.UseUrls($"{(Config.Ssl ? https : http)}://{item.IP}:{item.Port}/");
                ServerMain.LogOut($"Http服务器监听{item.IP}:{item.Port}");
            }

            builder.Services.AddHttpClient<HttpClients>(option =>
            {

            });
            builder.Services.AddTransient<IHttpClients, HttpClients>();

            bool Run = true;
            if (!Config.NoInput)
                new Thread(() =>
                {
                    while (Run)
                    {
                        ServerMain.Command(Console.ReadLine());
                    }
                }).Start();

            Web = builder.Build();
            Clients = Web.Services.GetRequiredService<IHttpClients>();

            Web.MapGet("/", GetIndex);
            Web.MapGet("/{**name}", GetWeb);
            //Web.MapGet("/{uuid}/{name}", GetGetWeb1);
            Web.MapGet(ServerMain.Config.Requset.WebAPI + "/{uuid}", GetBack);
            Web.MapGet(ServerMain.Config.Requset.WebAPI + "/{uuid}/{name}", GetBack);

            Web.MapPost("/", PostBuild);
            Web.MapPost("/{**name}", GetWeb);
           // Web.MapPost("/{uuid}/{name}", GetGetWeb1);
            Web.MapPost(ServerMain.Config.Requset.WebAPI + "/{uuid}", POSTBack);
            Web.MapPost(ServerMain.Config.Requset.WebAPI + "/{uuid}/{name}", POSTBack);

            Web.Run();

            Run = false;
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
            HttpRequest Request = context.Request;
            HttpResponse Response = context.Response;
            HttpReturn httpReturn;
            var name = context.GetRouteValue("name") as string;
            if (name == null)
                return;
            var arg = name.Split('/');
            if (Config.Rotes.TryGetValue(arg[0], out var rote))
            {
                using HttpClient ProxyRequest = Clients.GetOne();
                HttpRequestMessage message = new();
                message.Method = new HttpMethod(Request.Method);
                string url = "";
                if (arg.Length > 1)
                {
                    for (int a = 1; a < arg.Length; a++)
                    {
                        url += $"/{arg[a]}";
                    }
                }

                message.RequestUri = new Uri($"{rote.Url}{url}");
                if (Request.Method is "POST")
                    message.Content = new StreamContent(Request.Body);
                else
                    message.Content = new ByteArrayContent(Array.Empty<byte>());

                foreach (var item in Request.Headers)
                {
                    if (item.Key.StartsWith("Content"))
                    {
                        message.Content.Headers.Add(item.Key, (IEnumerable<string>)item.Value);
                    }
                    else
                        message.Headers.Add(item.Key, (IEnumerable<string>)item.Value);
                }

                if (url.EndsWith(".php"))
                {
                    var res = await ProxyRequest.SendAsync(message);
                    Response.StatusCode = (int)res.StatusCode;
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
                    var res = await ProxyRequest.SendAsync(message);

                    Response.StatusCode = (int)res.StatusCode;
                    Response.ContentType = res.Content.Headers.ContentType.ToString();

                    foreach (var item in res.Headers)
                    {

                        StringValues values = new(item.Value.ToArray());
                        Response.Headers.Add(item.Key, values);
                    }
                    await res.Content.CopyToAsync(Response.Body);
                }
            }
            else if (arg.Length == 2)
            {
                httpReturn = HttpStatic.Get(arg[0], arg[1]);
                Response.ContentType = httpReturn.ContentType;
                Response.StatusCode = httpReturn.ReCode;
                await Response.BodyWriter.WriteAsync(httpReturn.Data);
            }
            else
            {
                httpReturn = HttpStatic.Get(name);
                Response.ContentType = httpReturn.ContentType;
                Response.StatusCode = httpReturn.ReCode;
                await Response.BodyWriter.WriteAsync(httpReturn.Data);
            }   
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
                        //boundary??
                        //var temp1 = Request.ContentType.Remove(0, ServerContentType.POSTFORMDATA.Length);
                        //MemoryStream stream = new();
                        //await Request.Body.CopyToAsync(stream);
                        //var temp2 = Encoding.UTF8.GetString(stream.ToArray());
                        //stream.Seek(0, SeekOrigin.Begin);
                        //var parser = await MultipartFormDataParser.ParseAsync(stream);
                        //if (parser == null)
                        //{
                        //    httpReturn = new HttpReturn
                        //    {
                        //        Data = StreamUtils.JsonOBJ(new GetMeesage
                        //        {
                        //            Res = 123,
                        //            Text = "表单解析发生错误"
                        //        })
                        //    };
                        //    await Response.BodyWriter.WriteAsync(httpReturn.Data);
                        //    return;
                        //}

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
                                FileName = item.FileName
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        ServerMain.LogError(e);
                        httpReturn = new HttpReturn
                        {
                            Data = StreamUtils.JsonOBJ(new GetMeesage
                            {
                                Res = 123,
                                Text = "表单解析发生错误，请检查数据"
                            })
                        };
                        await Response.BodyWriter.WriteAsync(httpReturn.Data);
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
                    Cookie = ASPHttpUtils.HaveCookie(Request.Headers),
                    Parameter = temp,
                    RowRequest = collection,
                    ContentType = type,
                    Stream = type == MyContentType.Other ? Request.Body : null
                }, name);
            }
            else
            {
                httpReturn = new HttpReturn
                {
                    Data = HtmlUtils.Html404,
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

