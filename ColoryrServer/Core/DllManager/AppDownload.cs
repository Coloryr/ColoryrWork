using ColoryrServer.FileSystem;
using ColoryrServer.Http;
using ColoryrServer.Utils;
using Lib.App;
using System.Collections.Generic;
using System.IO;

namespace ColoryrServer.DllManager
{
    public class AppDownload
    {
        public static HttpReturn Download(DownloadObj obj)
        {
            var save = CSFile.GetApp(obj.UUID);
            if (save.Key != obj.Key)
                return new HttpReturn
                {
                    Data = StreamUtils.StringOBJ("Key或UUID错误"),
                    ReCode = 400
                };
            var item = DllStonge.GetApp(obj.UUID);
            if (item != null)
            {
                switch (obj.Type)
                {
                    case FileType.List:
                        var list = new Dictionary<string, string>(save.Xamls);
                        foreach (var item1 in save.Files)
                        {
                            list.Add(item1.Key, item1.Value);
                        }
                        return new HttpReturn
                        {
                            Data = StreamUtils.JsonOBJ(list)
                        };
                    case FileType.File:
                        if (obj.Name == "app.dll")
                        {
                            return new HttpReturn
                            {
                                Data = item.Dll
                            };
                        }
                        else if (obj.Name == "app.pdb")
                        {
                            return new HttpReturn
                            {
                                Data = item.Pdb
                            };
                        }
                        else
                        {
                            if (save.Xamls.ContainsKey(obj.Name))
                            {
                                return new HttpReturn
                                {
                                    Data = StreamUtils.StringOBJ(save.Xamls[obj.Name])
                                };
                            }
                            else if (save.Files.ContainsKey(obj.Name))
                            {
                                if (File.Exists(DllStonge.AppLocal + save.UUID + "\\" + obj.Name))
                                    return new HttpReturn
                                    {
                                        Data = File.OpenRead(DllStonge.AppLocal + save.UUID + "\\" + obj.Name),
                                        Res = ResType.Stream
                                    };
                                else
                                {
                                    return new HttpReturn
                                    {
                                        ReCode = 400,
                                        Data = "文件被删除",
                                        Res = ResType.String
                                    };
                                }
                            }
                            else
                            {
                                return new HttpReturn
                                {
                                    Data = "找不到文件",
                                    ReCode = 404,
                                    Res = ResType.String
                                };
                            }
                        }
                }
            }
            return new HttpReturn
            {
                Data = "Key或UUID错误",
                Res = ResType.String
            };
        }
    }
}
