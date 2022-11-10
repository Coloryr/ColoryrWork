using ColoryrServer.Core.Database;
using ColoryrServer.Core.FileSystem;
using ColoryrServer.Core.FileSystem.Managers;
using ColoryrServer.Core.Http;
using ColoryrServer.Core.Utils;
using ColoryrWork.Lib.Build.Object;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ColoryrServer.Core.BuilderPost;

internal static class PostBuildWeb
{
    public static ReMessage Add(BuildOBJ json)
    {
        if (WebFileManager.GetHtml(json.UUID) != null)
            new ReMessage
            {
                Build = false,
                Message = $"Web[{json.UUID}]已存在"
            };
        var time = string.Format("{0:s}", DateTime.Now);
        var isVue = json.Code.ToLower() == "true";
        string uuid = json.UUID.Replace('\\', '/');
        if (uuid.EndsWith('/'))
        {
            uuid = uuid[..^1];
        }
        if (HttpInvokeRoute.CheckBase(uuid))
            return new ReMessage
            {
                Build = false,
                Message = $"Web[{uuid}]路由冲突"
            };
        ServerMain.LogOut($"[{json.User}]正在创建Web[{json.UUID}]");
        WebObj obj;
        if (isVue)
        {
            obj = new()
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
                        { "vite.config.ts", Encoding.UTF8.GetString(DemoVueResource.vite_config).Replace("{dir}", $"/{uuid}/") },
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
            obj = new()
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

        WebFileManager.New(obj);
        ServerMain.LogOut($"[{json.User}]创建Web[{json.UUID}]完成");

        return new ReMessage
        {
            Build = true,
            Message = $"Web[{json.UUID}]已创建"
        };
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
        var obj = WebFileManager.GetHtml(json.UUID);
        if (obj == null)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"没有这个Web[{json.UUID}]"
            };
        }
        if (obj.Version != json.Version)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Web[{json.UUID}]版本号错误"
            };
        }

        var list = JsonConvert.DeserializeObject<List<CodeEditObj>>(json.Code);
        if (!obj.Codes.TryGetValue(json.Temp, out var code))
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Web[{json.UUID}]不存在文件[{json.Temp}]"
            };
        }
        code = FileUtils.StartEdit(code, list);
        obj.Text = json.Text;

        WebFileManager.StorageCode(obj, json.Temp, code);
        if (!obj.IsVue)
        {
            HttpInvokeRoute.ReloadFile(obj.UUID, json.Temp);
        }

        return new ReMessage
        {
            Build = true,
            Message = $"Web[{json.UUID}]文件[{json.Temp}]已修改",
            UseTime = "0",
            Time = obj.UpdateTime
        };
    }

    public static ReMessage AddCode(BuildOBJ json)
    {
        var obj = WebFileManager.GetHtml(json.UUID);
        if (obj == null)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Web[{json.UUID}]没有找到"
            };
        }
        if (obj.Codes.ContainsKey(json.Code))
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Web[{json.UUID}]的源码文件[{json.Code}]已存在"
            };
        }

        WebFileManager.AddItem(obj, json.Code, true, "");
        return new ReMessage
        {
            Build = true,
            Message = $"Web[{json.UUID}]已添加源码文件[{json.Code}]"
        };
    }

    public static ReMessage RemoveFile(BuildOBJ json)
    {
        var obj = WebFileManager.GetHtml(json.UUID);
        if (obj == null)
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
        if (obj.Codes.ContainsKey(json.Code))
        {
            isCode = true;
        }
        else if (!obj.Files.ContainsKey(json.Code))
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Web[{json.UUID}]的文件[{json.Code}]不存在"
            };
        }

        WebFileManager.RemoveItem(obj, json.Code, isCode);
        return new ReMessage
        {
            Build = true,
            Message = $"Web[{json.UUID}]已删除文件[{json.Code}]"
        };
    }

    public static ReMessage WebAddFile(BuildOBJ json)
    {
        var obj = WebFileManager.GetHtml(json.UUID);
        if (obj == null)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Web[{json.UUID}]没有找到"
            };
        }
        if (obj.Files.ContainsKey(json.Code))
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Web[{json.UUID}]的文件[{json.Code}]已存在"
            };
        }

        WebFileManager.AddItem(obj, json.Code, false, data: Convert.FromBase64String(json.Temp));
        return new ReMessage
        {
            Build = true,
            Message = $"Web[{json.UUID}]已添加文件[{json.Code}]"
        };
    }

    public static ReMessage Remove(BuildOBJ json)
    {
        var obj = WebFileManager.GetHtml(json.UUID);
        if (obj == null)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Web[{json.UUID}]没有找到"
            };
        }

        WebFileManager.DeleteAll(obj);
        return new ReMessage
        {
            Build = true,
            Message = $"Web[{json.UUID}]已删除"
        };
    }

    public static ReMessage SetIsVue(BuildOBJ json)
    {
        var obj = WebFileManager.GetHtml(json.UUID);
        if (obj == null)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Web[{json.UUID}]没有找到"
            };
        }

        WebFileManager.SetIsVue(obj, json.Code.ToLower() is "true");
        return new ReMessage
        {
            Build = true,
            Message = $"Web[{json.UUID}]已设置Vue模式"
        };
    }

    public static ReMessage Build(BuildOBJ json)
    {
        var obj = WebFileManager.GetHtml(json.UUID);
        if (obj == null)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Web[{json.UUID}]没有找到"
            };
        }
        if (!obj.IsVue)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Web[{json.UUID}]是非Vue项目，不可构建"
            };
        }

        if (VueBuildManager.IsBuildNow(obj.UUID))
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Web[{json.UUID}]项目正在构建"
            };
        }

        VueBuildManager.StartBuild(obj);

        return new ReMessage
        {
            Build = true,
            Message = $"Web[{json.UUID}]运行构建操作"
        };
    }

    public static ReMessage Download(BuildOBJ json)
    {
        var obj = WebFileManager.GetHtml(json.UUID);
        if (obj == null)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Web[{json.UUID}]没有找到"
            };
        }

        if (!obj.Files.ContainsKey(json.Code))
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
            Message = Convert.ToBase64String(obj.Files[json.Code])
        };
    }

    public static ReMessage BuildRes(BuildOBJ json)
    {
        var obj = WebFileManager.GetHtml(json.UUID);
        if (obj == null)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Web[{json.UUID}]没有找到"
            };
        }
        if (!obj.IsVue)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Web[{json.UUID}]是非Vue项目，没有构建信息"
            };
        }

        if (!VueBuildManager.IsBuildDone(obj.UUID))
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Web[{json.UUID}]项目正在构建"
            };
        }

        string res = VueBuildManager.GetBuildRes(obj.UUID);

        return new ReMessage
        {
            Build = true,
            Message = $"Web[{json.UUID}]项目构建结果",
            Time = res
        };
    }

    public static ReMessage ZIP(BuildOBJ json)
    {
        var obj = WebFileManager.GetHtml(json.UUID);
        if (obj == null)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Web[{json.UUID}]没有找到"
            };
        }

        var temp = Convert.FromBase64String(json.Code);
        string local = WebFileManager.WebCodeLocal + json.UUID;
        Directory.Delete(local, true);
        Directory.CreateDirectory(local);
        foreach (var item in obj.Codes)
        {
            WebFileManager.RemoveItem(obj, item.Key, true);
        }
        foreach (var item in obj.Files)
        {
            WebFileManager.RemoveItem(obj, item.Key, false);
        }
        obj.Codes.Clear();
        obj.Files.Clear();
        ZipUtils.ZipDeCompress(temp, local);
        var dir = new DirectoryInfo(local);
        foreach (var item in Utils.FileUtils.GetDirectoryFile(dir))
        {
            string name = item.FullName.Replace(dir.FullName, "")[1..];
            if (name.EndsWith(".html") || name.EndsWith(".css")
            || name.EndsWith(".js") || name.EndsWith(".json")
            || name.EndsWith(".txt") || name.EndsWith(".vue")
            || name.EndsWith(".ts"))
            {
                string code = File.ReadAllText(item.FullName);
                obj.Codes.Add(name, code);
                WebFileManager.StorageCode(obj, name, code);
            }
            else
            {
                var data = File.ReadAllBytes(item.FullName);
                obj.Files.Add(name, data);
                WebFileManager.StorageFile(obj, name, data);
            }
        }
        obj.Up();
        WebDatabase.Storage(obj);

        return new ReMessage
        {
            Build = true,
            Message = $"Web[{json.UUID}]代码压缩包上传成功"
        };
    }
}
