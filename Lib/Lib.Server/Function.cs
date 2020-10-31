using System.Collections.Generic;
using System.IO;

namespace Lib.Server
{
    class Function
    {
        public static string GetSrings(string a, string b = null, string c = null)
        {
            int x, y;
            if (b != null)
                x = a.IndexOf(b) + b.Length - 1;
            else
                x = 0;
            if (c != null)
            {
                y = a.IndexOf(c, x + 1);
                if (y - x <= 0)
                    return a;
                else
                    return a.Substring(x, y - x);
            }
            else
                return a.Substring(x);
        }
        public static List<FileInfo> GetPathFileName(string path)
        {
            var files = new List<FileInfo>();
            var root = new DirectoryInfo(path);
            foreach (FileInfo f in root.GetFiles())
            {
                files.Add(f);
            }
            return files;
        }
        public static List<FileInfo> GetDirectoryFileName(string path)
        {
            var files = new List<FileInfo>();
            files.AddRange(GetPathFileName(path));
            var root = new DirectoryInfo(path);
            foreach (DirectoryInfo d in root.GetDirectories())
            {
                files.AddRange(GetDirectoryFileName(d.FullName));
            }
            return files;
        }
        public static string GetLast(string a, string b = "/")
        {
            var index = a.LastIndexOf(b);
            if (index < 0)
                return a;
            return a.Substring(index + 1, a.Length - index - 1);
        }
        public static int Constr(string a, char b)
        {
            char[] c = a.ToCharArray();
            int con = 0;
            for (int i = 0; i < a.Length; i++)
            {
                if (c[i] == b)
                {
                    con++;
                }
            }
            return con;
        }
        public static List<DirectoryInfo> GetPathName(string appLocal)
        {
            var files = new List<DirectoryInfo>();
            var root = new DirectoryInfo(appLocal);
            foreach (var f in root.GetDirectories())
            {
                files.Add(f);
            }
            return files;
        }
    }
}
