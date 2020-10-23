using Newtonsoft.Json;
using System.Text;

namespace ColoryrServer.Robot
{
    class BuildPack
    {
        internal static byte[] Build(object obj, byte index)
        {
            byte[] data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj) + " ");
            data[data.Length - 1] = index;
            return data;
        }
        internal static byte[] BuildImage(long qq, long id, long fid, string img, byte index)
        {
            string temp = "";
            if (id != 0)
            {
                temp += "id=" + id + "&";
            }
            if (fid != 0)
            {
                temp += "fid=" + fid + "&";
            }
            temp += "qq=" + qq + "&";
            temp += "img=" + img;
            byte[] data = Encoding.UTF8.GetBytes(temp + " ");
            data[data.Length - 1] = index;
            return data;
        }

        internal static byte[] BuildSound(long qq, long id, string sound, byte index)
        {
            string temp = "id=" + id + "&qq=" + qq + "&sound=" + sound;
            byte[] data = Encoding.UTF8.GetBytes(temp + " ");
            data[data.Length - 1] = index;
            return data;
        }
    }
}
