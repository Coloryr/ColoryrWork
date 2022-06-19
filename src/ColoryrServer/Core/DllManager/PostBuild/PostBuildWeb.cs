using ColoryrServer.Core.FileSystem;
using ColoryrServer.Core.FileSystem.Html;
using ColoryrWork.Lib.Build.Object;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Collections.Generic;

namespace ColoryrServer.Core.DllManager.PostBuild;

internal static class PostBuildWeb
{
    public static ReMessage Add(BuildOBJ json)
    {
        ReMessage res;
        if (WebFileManager.GetHtml(json.UUID) == null)
        {
            var time = string.Format("{0:s}", DateTime.Now);
            var isVue = json.Code.ToLower() == "true";
            string uuid = json.UUID.Replace('\\', '/');
            if (!uuid.EndsWith('/'))
            {
                uuid += '/';
            }
            WebObj File2;
            if (isVue)
            {
                File2 = new()
                {
                    UUID = uuid,
                    CreateTime = time,
                    Text = "",
                    Codes = new()
                    {
                        { "index.html", Encoding.UTF8.GetString(DemoVueResource.index) },
                        { "package.json", Encoding.UTF8.GetString(DemoVueResource.package) },
                        { "package-lock.json", Encoding.UTF8.GetString(DemoVueResource.package_lock) },
                        { "tsconfig.json", Encoding.UTF8.GetString(DemoVueResource.tsconfig) },
                        { "tsconfig.node.json", Encoding.UTF8.GetString(DemoVueResource.tsconfig_node) },
                        { "vite.config.ts", Encoding.UTF8.GetString(DemoVueResource.vite_config) },
                        { "src/App.vue", Encoding.UTF8.GetString(DemoVueResource.App) },
                        { "src/env.d.ts", Encoding.UTF8.GetString(DemoVueResource.env_d) },
                        { "src/HelloWorld.vue", Encoding.UTF8.GetString(DemoVueResource.HelloWorld) },
                        { "src/main.ts", Encoding.UTF8.GetString(DemoVueResource.main) }
                    },
                    Files = new()
                    {
                        { "src/logo.png", DemoVueResource.logo }
                    },
                    IsVue = true
                };
            }
            else 
            {
                File2 = new()
                {
                    UUID = uuid,
                    CreateTime = time,
                    Text = "",
                    Codes = new()
                    {
                        { "index.html", DemoWebResource.HtmlDemoHtml.Replace(CodeDemo.Name, json.UUID) },
                        { "js.js", DemoWebResource.IndexDemoJS }
                    },
                    Files = new()
                };
            }
            
            WebFileManager.New(File2);
            res = new ReMessage
            {
                Build = true,
                Message = $"Web[{json.UUID}]已创建"
            };
            ServerMain.LogOut($"Web[{json.UUID}]创建");
        }
        else
            res = new ReMessage
            {
                Build = false,
                Message = $"Web[{json.UUID}]已存在"
            };

        return res;
    }

    public static CSFileList GetList()
    {
        var list = new CSFileList();
        foreach (var item in WebFileManager.HtmlCodeList)
        {
            list.List.Add(item.Key, item.Value);
        }

        return list;
    }

    public static WebObj GetCode(BuildOBJ json)
    {
        return WebFileManager.GetHtml(json.UUID);
    }

    public static ReMessage Updata(BuildOBJ json)
    {
        var File2 = WebFileManager.GetHtml(json.UUID);
        if (File2 == null)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"没有这个Web[{json.UUID}]"
            };
        }
        if (File2.Version != json.Version)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Web[{json.UUID}]版本号错误"
            };
        }

        var list = JsonConvert.DeserializeObject<List<CodeEditObj>>(json.Code);
        if (!File2.Codes.TryGetValue(json.Temp, out var code))
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Web[{json.UUID}]不存在文件[{json.Temp}]"
            };
        }
        code = FileEdit.StartEdit(code, list);
        File2.Text = json.Text;

        WebFileManager.StorageCode(File2, json.Temp, code);

        return new ReMessage
        {
            Build = true,
            Message = $"Web[{json.UUID}]文件[{json.Temp}]已修改",
            UseTime = "0",
            Time = File2.UpdateTime
        };
    }

    public static ReMessage AddCode(BuildOBJ json)
    {
        var File2 = WebFileManager.GetHtml(json.UUID);
        if (File2 == null)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Web[{json.UUID}]没有找到"
            };
        }
        if (File2.Codes.ContainsKey(json.Code))
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Web[{json.UUID}]的源码文件[{json.Code}]已存在"
            };
        }

        WebFileManager.AddItem(File2, json.Code, true, "");
        return new ReMessage
        {
            Build = true,
            Message = $"Web[{json.UUID}]已添加源码文件[{json.Code}]"
        };
    }

    public static ReMessage RemoveFile(BuildOBJ json)
    {
        var File2 = WebFileManager.GetHtml(json.UUID);
        if (File2 == null)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Web[{json.UUID}]没有找到"
            };
        }
        if (json.Code == "index.html")
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Web[{json.UUID}]的主文件不允许删除"
            };
        }
        bool isCode = false;
        if (File2.Codes.ContainsKey(json.Code))
        {
            isCode = true;
        }
        else if (!File2.Files.ContainsKey(json.Code))
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Web[{json.UUID}]的文件[{json.Code}]不存在"
            };
        }

        WebFileManager.RemoveItem(File2, json.Code, isCode);
        return new ReMessage
        {
            Build = true,
            Message = $"Web[{json.UUID}]已删除文件[{json.Code}]"
        };
    }

    public static ReMessage WebAddFile(BuildOBJ json)
    {
        var File2 = WebFileManager.GetHtml(json.UUID);
        if (File2 == null)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Web[{json.UUID}]没有找到"
            };
        }
        if (File2.Files.ContainsKey(json.Code))
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Web[{json.UUID}]的文件[{json.Code}]已存在"
            };
        }

        WebFileManager.AddItem(File2, json.Code, false, data: Convert.FromBase64String(json.Temp));
        return new ReMessage
        {
            Build = true,
            Message = $"Web[{json.UUID}]已添加文件[{json.Code}]"
        };
    }

    public static ReMessage Remove(BuildOBJ json)
    {
        var File2 = WebFileManager.GetHtml(json.UUID);
        if (File2 == null)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Web[{json.UUID}]没有找到"
            };
        }

        WebFileManager.DeleteAll(File2);
        return new ReMessage
        {
            Build = true,
            Message = $"Web[{json.UUID}]已删除"
        };
    }

    public static ReMessage SetIsVue(BuildOBJ json)
    {
        var File2 = WebFileManager.GetHtml(json.UUID);
        if (File2 == null)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Web[{json.UUID}]没有找到"
            };
        }

        WebFileManager.SetIsVue(File2, json.Code.ToLower() is "true");
        return new ReMessage
        {
            Build = true,
            Message = $"Web[{json.UUID}]已设置Vue模式"
        };
    }

    public static ReMessage Build(BuildOBJ json)
    {
        var File2 = WebFileManager.GetHtml(json.UUID);
        if (File2 == null)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Web[{json.UUID}]没有找到"
            };
        }
        if (!File2.IsVue)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Web[{json.UUID}]是非Vue项目，不可构建"
            };
        }

        return null;
    }

    public static ReMessage Download(BuildOBJ json)
    {
        var File2 = WebFileManager.GetHtml(json.UUID);
        if (File2 == null)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Web[{json.UUID}]没有找到"
            };
        }

        if (!File2.Files.ContainsKey(json.Code))
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Web[{json.UUID}]不存在文件{json.Code}"
            };
        }

        return new ReMessage
        {
            Build = true,
            Message = Convert.ToBase64String(File2.Files[json.Code])
        };
    }
}
