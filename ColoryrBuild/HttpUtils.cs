using Lib.Build;
using Lib.Build.Object;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;

namespace ColoryrBuild
{
    public class HttpUtils
    {
        private HttpClient httpClient;
        public HttpUtils()
        {
            var Handler = new HttpClientHandler();
            httpClient = new(Handler)
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
            httpClient.DefaultRequestHeaders.Add(BuildKV.BuildK, BuildKV.BuildV);
        }

        public bool CheckLogin(string data)
        {
            var obj = JObject.Parse(data);
            if (obj.ContainsKey("Build") && obj.ContainsKey("Message"))
            {
                var item1 = obj["Build"].ToString();
                var item2 = obj["Message"].ToString();
                if (item1 == "False" && item2 == "233")
                {
                    App.LogShow("登录", "登录失效");
                    App.Login();
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> AutoLogin()
        {
            try
            {
                var pack = new BuildOBJ
                {
                    User = App.Config.Name,
                    Token = App.Config.Token,
                    Mode = ReType.Check
                };
                HttpContent Content = new StringContent(JsonConvert.SerializeObject(pack));
                Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var temp = await httpClient.PostAsync(App.Config.Http, Content);
                var data = await temp.Content.ReadAsStringAsync();
                ReMessage res = JsonConvert.DeserializeObject<ReMessage>(data);
                if (res.Build == true)
                {
                    return true;
                }
                else
                {
                    App.LogShow("登录", "自动登录失败");
                }
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return false;
            }
        }

        public async Task<CSFileList> GetList(CodeType type)
        {
            try
            {
                var reType = type switch
                {
                    CodeType.Class => ReType.GetClass,
                    CodeType.IoT => ReType.GetIoT,
                    CodeType.Robot => ReType.GetRobot,
                    CodeType.WebSocket => ReType.GetWebSocket,
                    CodeType.App => ReType.GetApp,
                    _ => ReType.GetDll,
                };
                var pack = new BuildOBJ
                {
                    User = App.Config.Name,
                    Token = App.Config.Token,
                    Mode = reType
                };
                HttpContent Content = new StringContent(JsonConvert.SerializeObject(pack));
                Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var temp = await httpClient.PostAsync(App.Config.Http, Content);
                var data = await temp.Content.ReadAsStringAsync();
                if (!CheckLogin(data))
                {
                    return await GetList(type);
                }
                return JsonConvert.DeserializeObject<CSFileList>(data);
            }
            catch
            {
                return null;
            }
        }

        public async Task<ReMessage> Add(CodeType type, string name)
        {
            try
            {
                var reType = type switch
                {
                    CodeType.Class => ReType.AddClass,
                    CodeType.IoT => ReType.AddIoT,
                    CodeType.Robot => ReType.AddRobot,
                    CodeType.WebSocket => ReType.AddWebSocket,
                    CodeType.App => ReType.AddApp,
                    _ => ReType.AddDll,
                };
                var pack = new BuildOBJ
                {
                    User = App.Config.Name,
                    Token = App.Config.Token,
                    Mode = reType,
                    UUID = name
                };
                HttpContent Content = new StringContent(JsonConvert.SerializeObject(pack));
                Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var temp = await httpClient.PostAsync(App.Config.Http, Content);
                var data = await temp.Content.ReadAsStringAsync();
                if (!CheckLogin(data))
                {
                    return await Add(type, name);
                }
                return JsonConvert.DeserializeObject<ReMessage>(data);
            }
            catch
            {
                return null;
            }
        }

        public async Task<ReMessage> Remove(CodeType type, CSFileObj obj)
        {
            try
            {
                var reType = type switch
                {
                    CodeType.Class => ReType.RemoveClass,
                    CodeType.IoT => ReType.RemoveIoT,
                    CodeType.Robot => ReType.RemoveRobot,
                    CodeType.WebSocket => ReType.RemoveWebSocket,
                    CodeType.App => ReType.RemoveApp,
                    _ => ReType.RemoveDll,
                };
                var pack = new BuildOBJ
                {
                    User = App.Config.Name,
                    Token = App.Config.Token,
                    Mode = reType,
                    UUID = obj.UUID
                };
                HttpContent Content = new StringContent(JsonConvert.SerializeObject(pack));
                Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var temp = await httpClient.PostAsync(App.Config.Http, Content);
                var data = await temp.Content.ReadAsStringAsync();
                if (!CheckLogin(data))
                {
                    return await Remove(type, obj);
                }
                return JsonConvert.DeserializeObject<ReMessage>(data);
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> Login(string Pass)
        {
            try
            {
                var pack = new BuildOBJ
                {
                    User = App.Config.Name,
                    Code = Pass,
                    Mode = ReType.Login
                };
                HttpContent Content = new StringContent(JsonConvert.SerializeObject(pack));
                Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var temp = await httpClient.PostAsync(App.Config.Http, Content);
                var data = await temp.Content.ReadAsStringAsync();
                ReMessage res = JsonConvert.DeserializeObject<ReMessage>(data);
                if (res.Build == true)
                {
                    if (App.Config.SaveToken)
                    {
                        App.Config.Token = res.Message;
                    }
                    App.LogShow("登录", "登录成功");
                    return true;
                }
                else
                {
                    App.LogShow("登录", res.Message);
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<CSFileCode> GetCode(CodeType type, string name)
        {
            try
            {
                var reType = type switch
                {
                    CodeType.Class => ReType.CodeClass,
                    CodeType.IoT => ReType.CodeIoT,
                    CodeType.Robot => ReType.CodeRobot,
                    CodeType.WebSocket => ReType.CodeWebSocket,
                    _ => ReType.CodeDll,
                };
                var pack = new BuildOBJ
                {
                    User = App.Config.Name,
                    Token = App.Config.Token,
                    Mode = reType,
                    UUID = name
                };
                HttpContent Content = new StringContent(JsonConvert.SerializeObject(pack));
                Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var temp = await httpClient.PostAsync(App.Config.Http, Content);
                var data = await temp.Content.ReadAsStringAsync();
                if (data == "null")
                {
                    return null;
                }
                if (!CheckLogin(data))
                {
                    return await GetCode(type, name);
                }
                return JsonConvert.DeserializeObject<CSFileCode>(data);
            }
            catch
            {
                return null;
            }
        }

        public async Task<AppFileObj> GetAppCode(string name)
        {
            try
            {
                var pack = new BuildOBJ
                {
                    User = App.Config.Name,
                    Token = App.Config.Token,
                    Mode = ReType.CodeApp,
                    UUID = name
                };
                HttpContent Content = new StringContent(JsonConvert.SerializeObject(pack));
                Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var temp = await httpClient.PostAsync(App.Config.Http, Content);
                var data = await temp.Content.ReadAsStringAsync();
                if (!CheckLogin(data))
                {
                    return await GetAppCode(name);
                }
                return JsonConvert.DeserializeObject<AppFileObj>(data);
            }
            catch
            {
                return null;
            }
        }

        public async Task<ReMessage> BuildApp(AppFileObj obj, ReType type, string file, List<CodeEditObj> list)
        {
            try
            {
                var pack = new BuildOBJ
                {
                    User = App.Config.Name,
                    Token = App.Config.Token,
                    Mode = ReType.AppCsUpdata,
                    UUID = obj.UUID,
                    Version = obj.Version,
                    Text = obj.Text,
                    Temp = file,
                    Code = JsonConvert.SerializeObject(list)
                };
                HttpContent Content = new StringContent(JsonConvert.SerializeObject(pack));
                Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var temp = await httpClient.PostAsync(App.Config.Http, Content);
                var data = await temp.Content.ReadAsStringAsync();
                if (!CheckLogin(data))
                {
                    return await BuildApp(obj, type, file, list);
                }
                return JsonConvert.DeserializeObject<ReMessage>(data);
            }
            catch
            {
                return null;
            }
        }

        public async Task<ReMessage> AddAppFile(AppFileObj obj, ReType type, string file)
        {
            try
            {
                var pack = new BuildOBJ
                {
                    User = App.Config.Name,
                    Token = App.Config.Token,
                    Mode = ReType.AppAddCS,
                    UUID = obj.UUID,
                    Version = obj.Version,
                    Code = file
                };
                HttpContent Content = new StringContent(JsonConvert.SerializeObject(pack));
                Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var temp = await httpClient.PostAsync(App.Config.Http, Content);
                var data = await temp.Content.ReadAsStringAsync();
                if (!CheckLogin(data))
                {
                    return await AddAppFile(obj, type, file);
                }
                return JsonConvert.DeserializeObject<ReMessage>(data);
            }
            catch
            {
                return null;
            }
        }

        public async Task<ReMessage> Build(CSFileObj obj, CodeType type, List<CodeEditObj> list)
        {
            try
            {
                var reType = type switch
                {
                    CodeType.Class => ReType.UpdataClass,
                    CodeType.IoT => ReType.UpdataIoT,
                    CodeType.Robot => ReType.UpdataRobot,
                    CodeType.WebSocket => ReType.UpdataWebSocket,
                    _ => ReType.UpdataDll,
                };
                var pack = new BuildOBJ
                {
                    User = App.Config.Name,
                    Token = App.Config.Token,
                    Mode = reType,
                    UUID = obj.UUID,
                    Version = obj.Version,
                    Text = obj.Text,
                    Code = JsonConvert.SerializeObject(list)
                };
                HttpContent Content = new StringContent(JsonConvert.SerializeObject(pack));
                Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var temp = await httpClient.PostAsync(App.Config.Http, Content);
                var data = await temp.Content.ReadAsStringAsync();
                if (!CheckLogin(data))
                {
                    return await Build(obj, type, list);
                }
                return JsonConvert.DeserializeObject<ReMessage>(data);
            }
            catch
            {
                return null;
            }
        }

        public async Task<APIFileObj> GetApi()
        {
            try
            {
                var pack = new BuildOBJ
                {
                    User = App.Config.Name,
                    Token = App.Config.Token,
                    Mode = ReType.GetApi
                };
                HttpContent Content = new StringContent(JsonConvert.SerializeObject(pack));
                Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var temp = await httpClient.PostAsync(App.Config.Http, Content);
                var data = await temp.Content.ReadAsStringAsync();
                if (!CheckLogin(data))
                {
                    return await GetApi();
                }
                return JsonConvert.DeserializeObject<APIFileObj>(data);
            }
            catch
            {
                return null;
            }
        }
    }
}
