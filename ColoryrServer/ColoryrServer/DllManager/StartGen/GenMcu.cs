using Lib.Build.Object;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ColoryrServer.DllManager
{
    class GenMcu
    {
        public static GenReOBJ StartGen(McuFileObj File)
        {
            string dir = DllStonge.McuLocal + File.UUID + "\\";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            Task.Run(() =>
            {
                foreach (var item in File.Codes)
                {
                    using (var FileStream = new FileStream(
                    dir + item.Key + ".lua", FileMode.OpenOrCreate))
                    {
                        FileStream.Write(Encoding.UTF8.GetBytes(item.Value));
                        FileStream.Flush();
                    }
                }
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
