﻿using ColoryrServer.Core.Dll.DllLoad;
using ColoryrServer.Core.Http;
using ColoryrWork.Lib.Build.Object;
using System.Collections.Generic;
using System.Linq;

namespace ColoryrServer.Core.Dll;

public static class DllUseSave
{
    public static Dictionary<DllAssembly, List<DllAssembly>> UseLoad = new();

    public static void AddSave(DllAssembly key, DllAssembly item)
    {
        List<DllAssembly> list;
        if (UseLoad.ContainsKey(key))
        {
            list = UseLoad[key];
            var item1 = list.Find(a => a.Name == item.Name);
            if (item1 != null)
            {
                list.Remove(item1);
            }
        }
        else
        {
            list = new();
            UseLoad.Add(key, list);
        }
        list.Add(item);
    }

    public static void Update(DllAssembly name)
    {
        var list = UseLoad.Keys.Where(a => a.Name == name.Name);
        if (list.Any())
        {
            var dll = list.First();
            var list1 = UseLoad[dll];
            foreach (var item in list1)
            {
                Reload(item);
            }
            list1.Clear();
            UseLoad.Remove(dll);
        }
    }

    public static void Reload(DllAssembly name)
    {
        switch (name.DllType)
        {
            case CodeType.Dll:
                HttpInvokeRoute.Lock(name.Name);
                LoadDll.Reload(name.Name);
                break;
            case CodeType.Class:
                LoadClass.Reload(name.Name);
                break;
            case CodeType.Service:
                LoadService.Reload(name.Name);
                break;
            case CodeType.Robot:
                LoadRobot.Reload(name.Name);
                break;
            case CodeType.WebSocket:
                LoadWebSocket.Reload(name.Name);
                break;
            case CodeType.Socket:
                LoadSocket.Reload(name.Name);
                break;
            case CodeType.Mqtt:
                LoadMqtt.Reload(name.Name);
                break;
        }
    }
}
