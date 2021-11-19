using ColoryrServer.Core.FileSystem;
using System;

var dir = new StaticDir(AppContext.BaseDirectory + "web/");

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

