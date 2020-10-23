using Lib.Build.Object;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace ColoryrServer.DllManager
{
    class GenDll
    {
        //private static string GenAppKey(string uuid)
        //{

        //    SHA1 SHA1 = new SHA1CryptoServiceProvider();//创建SHA1对象
        //    byte[] sha1Bytes = SHA1.ComputeHash(Encoding.UTF8.GetBytes("Color_yr_APP:" + uuid + "_TEST_"));//Hash运算

        //    SHA1.Dispose();//释放当前实例使用的所有资源
        //    return BitConverter.ToString(sha1Bytes).Replace("-", "");
        //}

        private const string CodeBody =
            @"
namespace ColoryrSDK {
public class app_{0}
{
{1} 
}
}";
        private static bool CheckCode(string a)
        {
            foreach (string code in ServerMain.Config.NoCode)
            {
                if (a.Contains(code))
                    return false;
            }
            return true;
        }
        public static GenReOBJ StartGen(User User, CSFileCode CodeFile)
        {
            SyntaxTree Code;
            if (!User.Admin || CodeFile.User != null)
            {
                if (!CheckCode(CodeFile.Code))
                {
                    return new GenReOBJ
                    {
                        Isok = false,
                        Res = "非法的代码"
                    };
                }
                string using_ = null;
                foreach (string Item in ServerMain.Config.Include)
                {
                    using_ += Item;
                }
                Code = CSharpSyntaxTree.ParseText(using_ + CodeBody.Replace("{0}", CodeFile.UUID).Replace("{1}", CodeFile.Code));
                CodeFile.User = User.Username;
            }
            else
            {
                Code = CSharpSyntaxTree.ParseText(CodeFile.Code);
            }
            var Res = GenTask.StartGen(CodeFile.UUID, Code);
            if (!Res.Isok)
            {
                return Res;
            }

            Res.MS.Seek(0, SeekOrigin.Begin);
            Res.MSPdb.Seek(0, SeekOrigin.Begin);

            var AssemblySave = new AssemblySave();
            AssemblySave.Assembly = new AssemblyLoadContext(CodeFile.UUID, true);
            AssemblySave.Assembly.LoadFromStream(Res.MS, Res.MSPdb);
            var list = AssemblySave.Assembly.Assemblies.First().GetTypes()
                           .Where(x => x.Name == "app_" + CodeFile.UUID);

            if (list.Count() == 0)
                return new GenReOBJ
                {
                    Isok = false,
                    Res = "UUID错误"
                };

            AssemblySave.Type = list.First();
            foreach (var Item in AssemblySave.Type.GetMethods())
            {
                if (Item.Name == "Main" || Item.Name == "GetType" || Item.Name == "ToString"
                    || Item.Name == "Equals" || Item.Name == "GetHashCode")
                    continue;
                AssemblySave.MethodInfos.Add(Item.Name, Item);
            }

            if (AssemblySave.MethodInfos.Count() == 0)
                return new GenReOBJ
                {
                    Isok = false,
                    Res = "没有方法"
                };

            DllStonge.AddDll(CodeFile.UUID, AssemblySave);

            Task.Factory.StartNew(() =>
            {
                Res.MS.Seek(0, SeekOrigin.Begin);
                Res.MSPdb.Seek(0, SeekOrigin.Begin);

                using (var FileStream = new FileStream(
                    DllStonge.DllLocal + CodeFile.UUID + ".dll", FileMode.OpenOrCreate))
                {
                    FileStream.Write(Res.MS.ToArray());
                    FileStream.Flush();
                }

                using (var FileStream = new FileStream(
                    DllStonge.DllLocal + CodeFile.UUID + ".pdb", FileMode.OpenOrCreate))
                {
                    FileStream.Write(Res.MSPdb.ToArray());
                    FileStream.Flush();
                }

                CSFile.StorageDll(CodeFile);
                Config.Save();

                Res.MSPdb.Close();
                Res.MSPdb.Dispose();

                Res.MS.Close();
                Res.MS.Dispose();
                GC.Collect();
            });

            return new GenReOBJ
            {
                Isok = true,
                Res = "编译完成"
            };
        }
    }
}
