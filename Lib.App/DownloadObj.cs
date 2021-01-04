﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Lib.App
{
    public enum FileType
    { 
        List, File
    }
    public class DownloadObj
    {
        public string UUID { get; set; }
        public string Key { get; set; }
        public FileType Type { get; set; }
        public string Name { get; set; }
    }
}
