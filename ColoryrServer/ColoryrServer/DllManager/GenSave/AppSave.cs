using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColoryrServer.DllManager
{
    class AppSave
    {
        public byte[] Dll { get; set; }
        public byte[] Pdb { get; set; }
        public string Key { get; set; }
        public Dictionary<string, string> Xamls { get; set; }
    }
}
