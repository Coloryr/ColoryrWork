﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColoryrServer.Core.FileSystem
{
    public class HtmlFileObj
    {
        public byte[] Data { get; set; }
        public int Time { get; private set; }
        public void Reset()
        {
            Time = ServerMain.Config.Requset.TempTime;
        }
        public void Tick()
        {
            Time--;
        }
    }
}
