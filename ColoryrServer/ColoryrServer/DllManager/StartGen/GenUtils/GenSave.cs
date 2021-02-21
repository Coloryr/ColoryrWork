using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;

namespace ColoryrServer.DllManager.StartGen.GenUtils
{
    /// <summary>
    /// App资源存储
    /// </summary>
    internal record AppBuildSave
    {
        public byte[] Dll { get; set; }
        public byte[] Pdb { get; set; }
    }
    /// <summary>
    /// 编译后存储
    /// </summary>
    internal record DllBuildSave
    {
        public AssemblyLoadContext Assembly { get; set; }
        public Type DllType { get; set; }
        public Type NoteType { get; set; }
        public Dictionary<string, MethodInfo> MethodInfos { get; set; } = new();
    }
}
