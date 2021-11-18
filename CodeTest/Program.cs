using ColoryrServer.Core.FileSystem;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

var dir = new StaticDir(AppContext.BaseDirectory);

while (true)
{
    string data = Console.ReadLine();
    var arg1 = data.Split(' ');
    switch (arg1[0])
    {
        case "stop":
            dir.Stop();
            return;
    }
}

