﻿using System.Collections.Generic;

namespace ColoryrServer.Http
{
    internal record HttpReturn
    {
        public string Cookie;
        public object Data;
        public bool IsObj = true;
        public Dictionary<string, string> Head;
        public int ReCode = 200;
    }
}