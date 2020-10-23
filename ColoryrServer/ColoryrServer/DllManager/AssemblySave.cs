using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;

namespace ColoryrServer.DllManager
{
    class AssemblySave
    {
        public AssemblyLoadContext Assembly { get; set; }
        public Type Type { get; set; }
        public Dictionary<string, MethodInfo> MethodInfos { get; set; } = new Dictionary<string, MethodInfo>();
    }
}
