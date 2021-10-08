﻿using ColoryrServer.DllManager;
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
using System.Collections.Specialized;
using System.IO.Pipelines;
using System.Text;
using HttpRequest = ColoryrServer.SDK.HttpRequest;

namespace ColoryrServer.ASP
{
    public class ASPHttpPOST
    {
        public static async Task<HttpReturn> HttpPOST(Stream stream, long Length, string Url, IHeaderDictionary Hashtable, MyContentType type, string uuid, string name)
        {
            var Temp = new Dictionary<string, dynamic>();
            string Str = "";
            switch (type)
            {
                case MyContentType.Json:
                    try
                    {
                        MemoryStream memoryStream = new();
                        var data = new byte[2000000];
                        long la = Length;
                        while (la > 0)
                        {
                            int a = await stream.ReadAsync(data);
                            la -= a;
                            memoryStream.Write(data, 0, a);
                        }
                        if (Hashtable[BuildKV.BuildK1] == "true")
                        {
                            var receivedData = DeCode.AES256(memoryStream.ToArray(), ServerMain.Config.AES.Key, ServerMain.Config.AES.IV);
                            Str = Encoding.UTF8.GetString(receivedData);
                        }
                        else
                        {
                            Str = Encoding.UTF8.GetString(memoryStream.ToArray());
                        }
                        Str = new StreamReader(stream, Encoding.UTF8).ReadToEnd();
                        JObject obj = JObject.Parse(Function.GetSrings(Str, "{"));
                        foreach (var item in obj)
                        {
                            Temp.Add(item.Key, item.Value);
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
                        if (Hashtable[UploadKV.UploadK] != "")
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

                var Dll = DllStonge.GetDll(uuid);
                if (Dll != null)
                {
                    NameValueCollection collection = new();
                    foreach (var item in Hashtable)
                    {
                        collection.Add(item.Key, item.Value);
                    }
                    var Http = new HttpRequest
                    {
                        Cookie = ASPHttpUtils.HaveCookie(Hashtable),
                        Parameter = Temp,
                        RowRequest = collection,
                        ContentType = type,
                        Stream = type == MyContentType.Other ? stream : null
                    };
                    var Data = DllRun.DllGo(Dll, Http, name);
                    return Data;
                }
            }

            return HttpStatic.Get(Url);
        }
    }
}