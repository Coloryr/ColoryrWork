using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace ColoryrServer.DllManager
{
    /// <summary>
    /// App资源存储
    /// </summary>
    record AppSave
    {
        public byte[] Dll { get; set; }
        public byte[] Pdb { get; set; }
        public string Key { get; set; }
        public Dictionary<string, string> Xamls { get; set; } = new();
    }
    /// <summary>
    /// 编译后存储
    /// </summary>
    record AssemblySave
    {
        public AssemblyLoadContext Assembly { get; set; }
        public Type Type { get; set; }
        public Dictionary<string, MethodInfo> MethodInfos { get; set; } = new();
    }
}
