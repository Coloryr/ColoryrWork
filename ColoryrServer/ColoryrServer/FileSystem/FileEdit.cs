using Lib.Build;
using OracleInternal.SqlAndPlsqlParser.LocalParsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColoryrServer.FileSystem
{
    internal class FileEdit
    {
        public static string StartEdit(string old, List<CodeEditObj> editText)
        {
            var temp = old.Split("\n");
            var list = new List<string>(temp);
            foreach (var item in editText)
            {
                switch (item.Fun)
                {
                    case EditFun.Add:
                        list.Insert(item.Line, item.Code);
                        break;
                    case EditFun.Edit:
                        list[item.Line] = item.Code;
                        break;
                    case EditFun.Remove:
                        list.RemoveAt(item.Line);
                        break;
                }
            }
            old = "";
            foreach (var item in list)
            {
                old += item + "\n";
            }
            return old;
        }
    }
}
