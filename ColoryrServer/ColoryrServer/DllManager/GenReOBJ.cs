using System.IO;

namespace ColoryrServer.DllManager
{
    record GenReOBJ
    {
        public bool Isok { get; set; }
        public string Res { get; set; }
        public MemoryStream MS { get; set; }
        public MemoryStream MSPdb { get; set; }
    }
}
