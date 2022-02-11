using ColoryrWork.Lib.Build;
using System.Collections.Generic;

namespace ColoryrServer.FileSystem
{
    internal class FileEdit
    {
        public static string StartEdit(string old, List<CodeEditObj> editText)
        {
            var temp = old.Replace("\r", "").Split("\n");
            int arg = 0;
            var list = new List<string>(temp);
            foreach (var item in editText)
            {
                switch (item.Fun)
                {
                    case EditFun.Add:
                        list.Insert(item.Line + arg, item.Code);
                        break;
                    case EditFun.Edit:
                        list[item.Line + arg] = item.Code;
                        break;
                    case EditFun.Remove:
                        list.RemoveAt(item.Line + arg);
                        arg--;
                        break;
                }
            }
            old = "";
            foreach (var item in list)
            {
                old += item + "\n";
            }
            return old[0..^1];
        }
    }
}
