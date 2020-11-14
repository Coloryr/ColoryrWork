using System;
using System.Collections.Generic;
using System.Text;

namespace Lib.Build
{
    public enum EditFun
    {
        Add, Remove, Edit
    }
    class CodeEditObj
    {
        public EditFun Fun { get; set; }
        public int Line { get; set; }
        public string Code { get; set; }
    }
}
