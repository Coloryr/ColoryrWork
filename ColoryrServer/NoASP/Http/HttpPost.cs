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
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;

namespace ColoryrServer.NoASP
{
    class HttpPost
    {
        public static HttpReturn HttpPOST(Stream stream, long Length, string Url, NameValueCollection Hashtable, MyContentType type)
        {
            var Temp = new Dictionary<string, dynamic>();
            string Str = "";
            switch (type)
            {
                case MyContentType.Json:
                    try
                    {
                        if (Hashtable[BuildKV.BuildK] == BuildKV.BuildV)
                        {
                            if (Hashtable[BuildKV.BuildK1].ToLower() == "true")
                            {
                                MemoryStream memoryStream = new();
                                var data = new byte[2000000];
                                long la = Length;
                                while (la > 0)
                                {
                                    int a = stream.Read(data);
                                    la -= a;
                                    memoryStream.Write(data, 0, a);
                                }
                                var receivedData = DeCode.AES256(memoryStream.ToArray(), ServerMain.Config.AES.Key, ServerMain.Config.AES.IV);
                                Str = Encoding.UTF8.GetString(receivedData);
                            }
                            else
                            {
                                Str = new StreamReader(stream, Encoding.UTF8).ReadToEnd();
                            }
                            JObject obj = JObject.Parse(Function.GetSrings(Str, "{"));
                            var Json = obj.ToObject<BuildOBJ>();
                            var List = ServerMain.Config.User.Where(a => a.Username == Json.User);
                            if (List.Any())
                            {
                                return DllBuild.StartBuild(Json, List.First());
                            }
                            else
                            {
                                return new HttpReturn
                                {
                                    Data = StreamUtils.JsonOBJ(new ReMessage
                                    {
                                        Build = false,
                                        Message = "账户错误"
                                    })
                                };
                            }
                        }
                        else if (Hashtable[APPKV.APPK] == APPKV.APPV)
                        {
                            Str = new StreamReader(stream, Encoding.UTF8).ReadToEnd();
                            JObject obj = JObject.Parse(Function.GetSrings(Str, "{"));
                            var Json = obj.ToObject<DownloadObj>();
                            return AppDownload.Download(Json);
                        }
                        else
                        {
                            Str = new StreamReader(stream, Encoding.UTF8).ReadToEnd();
                            JObject obj = JObject.Parse(Function.GetSrings(Str, "{"));
                            foreach (var item in obj)
                            {
                                Temp.Add(item.Key, item.Value);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        ServerMain.LogError(e);
                        return new HttpReturn
                        {
                            Data = StreamUtils.JsonOBJ(new GetMeesage
                            {
                                Res = 123,
                                Text = "Json解析发生错误",
                                Data = e
                            })
                        };
                    }
                    break;
                case MyContentType.XFormData:
                    Str = new StreamReader(stream, Encoding.UTF8).ReadToEnd();
                    foreach (string Item in Str.Split('&'))
                    {
                        if (Item.Contains("="))
                        {
                            string[] KV = Item.Split('=');
                            Temp.Add(KV[0], KV[1]);
                        }
                    }
                    break;
                case MyContentType.MFormData:
                    var parser = MultipartFormDataParser.Parse(stream);
                    if (parser == null)
                    {
                        return new HttpReturn
                        {
                            Data = StreamUtils.JsonOBJ(new GetMeesage
                            {
                                Res = 123,
                                Text = "表单解析发生错误"
                            })
                        };
                    }
                    foreach (var item in parser.Parameters)
                    {
                        Temp.Add(item.Name, item.Data);
                    }
                    foreach (var item in parser.Files)
                    {
                        Temp.Add(item.Name, new HttpMultipartFile()
                        {
                            Data = item.Data,
                            FileName = item.FileName
                        });
                    }
                    break;
                case MyContentType.Other:
                    if (Hashtable[BuildKV.BuildK] == BuildKV.BuildV)
                    {
                        if (Hashtable[UploadKV.UploadK] != null)
                        {
                            string data = Hashtable[UploadKV.UploadK];
                            var item = Tools.ToObject<UploadObj>(data);
                            var app = CSFile.GetApp(item.UUID);
                            if (app == null)
                            {
                                return new HttpReturn
                                {
                                    Data = StreamUtils.JsonOBJ(new GetMeesage
                                    {
                                        Res = 123,
                                        Text = "UUID未找到"
                                    })
                                };
                            }
                            else
                            {
                                if (CSFile.AddFileApp(app, item, stream))
                                {
                                    return new HttpReturn
                                    {
                                        Data = StreamUtils.JsonOBJ(new GetMeesage
                                        {
                                            Res = 100,
                                            Text = "上传成功"
                                        })
                                    };
                                }
                                else
                                {
                                    return new HttpReturn
                                    {
                                        Data = StreamUtils.JsonOBJ(new GetMeesage
                                        {
                                            Res = 200,
                                            Text = "上传失败"
                                        })
                                    };
                                }
                            }
                        }
                        else
                        {
                            return new HttpReturn
                            {
                                Data = StreamUtils.JsonOBJ(new GetMeesage
                                {
                                    Res = 123,
                                    Text = "上传错误"
                                })
                            };
                        }
                    }
                    break;
            }

            if (Url.StartsWith(ServerMain.Config.Requset.WebAPI))
            {
                string UUID = "0";
                string FunctionName = null;
                Url = Url.Substring(ServerMain.Config.Requset.WebAPI.Length);
                int thr = Url.IndexOf('?', 0);
                if (Function.Constr(Url, '/') >= 2)
                {
                    int tow = Url.IndexOf('/', 2);
                    if (thr == -1)
                    {
                        UUID = Function.GetSrings(Url, "/", "/").Remove(0, 1);
                        FunctionName = Url[tow..].Remove(0, 1);
                    }
                    else if (tow < thr)
                    {
                        UUID = Function.GetSrings(Url, "/", "/").Remove(0, 1);
                        FunctionName = Url[tow..thr];
                    }
                    else
                    {
                        UUID = Function.GetSrings(Url, "/", "?").Remove(0, 1);
                    }
                }
                else
                {
                    if (thr != -1)
                        UUID = Function.GetSrings(Url, "/", "?").Remove(0, 1);
                    else
                        UUID = Function.GetSrings(Url, "/").Remove(0, 1);
                }

                var Dll = DllStonge.GetDll(UUID);
                if (Dll != null)
                {
                    var Http = new HttpRequest
                    {
                        Cookie = HttpUtils.HaveCookie(Hashtable),
                        Parameter = Temp,
                        RowRequest = Hashtable,
                        ContentType = type,
                        Stream = type == MyContentType.Other ? stream : null
                    };
                    var Data = DllRun.DllGo(Dll, Http, FunctionName);
                    return Data;
                }
            }

            return HttpStatic.Get(Url);
        }
    }
}
