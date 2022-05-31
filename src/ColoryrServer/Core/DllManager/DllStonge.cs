﻿using ColoryrServer.Core.DllManager;
using ColoryrServer.Core.DllManager.DllLoad;
using ColoryrServer.DllManager.StartGen.GenType;
using ColoryrServer.DllManager.StartGen.GenUtils;
using ColoryrServer.FileSystem;
using ColoryrWork.Lib.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace ColoryrServer.DllManager;

public class DllStonge
{
    private static readonly ConcurrentDictionary<string, DllBuildSave> DllList = new();
    private static readonly ConcurrentDictionary<string, DllBuildSave> ClassList = new();
    private static readonly ConcurrentDictionary<string, DllBuildSave> SocketList = new();
    private static readonly ConcurrentDictionary<string, DllBuildSave> WebSocketList = new();
    private static readonly ConcurrentDictionary<string, DllBuildSave> RobotList = new();
    private static readonly ConcurrentDictionary<string, DllBuildSave> MqttList = new();
    private static readonly ConcurrentDictionary<string, DllBuildSave> TaskList = new();

    public static readonly string DllLocal = ServerMain.RunLocal + @"Dll/Dll/";
    public static readonly string ClassLocal = ServerMain.RunLocal + @"Dll/Class/";
    public static readonly string SocketLocal = ServerMain.RunLocal + @"Dll/Socket/";
    public static readonly string WebSocketLocal = ServerMain.RunLocal + @"Dll/WebSocket/";
    public static readonly string RobotLocal = ServerMain.RunLocal + @"Dll/Robot/";
    public static readonly string MqttLocal = ServerMain.RunLocal + @"Dll/Mqtt/";
    public static readonly string TaskLocal = ServerMain.RunLocal + @"Dll/Task/";

    private static void RemoveAll(string dir)
    {
        if (File.Exists(dir + ".dll"))
        {
            File.Delete(dir + ".dll");
        }
        if (File.Exists(dir + ".pdb"))
        {
            File.Delete(dir + ".pdb");
        }
    }

    public static DllBuildSave FindClass(AssemblyName name) 
    {
        if (ClassList.TryGetValue(name.Name, out var item))
        {
            return item;
        }
        return null;
    }

    public static void AddDll(string uuid, DllBuildSave save)
    {
        if (DllList.TryRemove(uuid, out var item))
        {
            item.Unload();
            item.DllType = null;
            item.MethodInfos.Clear();
        }

        DllList.TryAdd(uuid, save);
    }
    public static void RemoveDll(string uuid)
    {
        if (DllList.TryRemove(uuid, out var item))
        {
            item.Unload();
            item.DllType = null;
            item.MethodInfos.Clear();
        }
        RemoveAll(DllLocal + uuid);
    }
    public static DllBuildSave GetDll(string uuid)
    {
        if (DllList.TryGetValue(uuid, out var save))
        {
            return save;
        }
        else
            return null;
    }

    public static void AddClass(string uuid, DllBuildSave save)
    {
        if (ClassList.TryRemove(uuid, out var item))
        {
            item.Unload();
            item.DllType = null;
            item.MethodInfos.Clear();
        }
        else
        {
            ClassList.TryAdd(uuid, save);
        }
        DllUseSave.Update(save);
    }
    public static void RemoveClass(string uuid)
    {
        if (ClassList.TryRemove(uuid, out var item))
        {
            item.Unload();
            item.DllType = null;
            item.MethodInfos.Clear();
        }
        RemoveAll(ClassLocal + uuid);
    }
    public static DllBuildSave GetClass(string uuid)
    {
        if (ClassList.TryGetValue(uuid, out var save))
        {
            return save;
        }
        else
            return null;
    }

    public static void AddSocket(string uuid, DllBuildSave save)
    {
        if (SocketList.ContainsKey(uuid))
        {
            SocketList[uuid].Unload();
            SocketList[uuid].DllType = null;
            SocketList[uuid].MethodInfos.Clear();
            SocketList[uuid] = save;
        }
        else
        {
            SocketList.TryAdd(uuid, save);
        }
    }
    public static void RemoveSocket(string uuid)
    {
        if (SocketList.TryRemove(uuid, out var item))
        {
            item.Unload();
            item.DllType = null;
            item.MethodInfos.Clear();
        }
        RemoveAll(SocketLocal + uuid);
    }

    public static void AddWebSocket(string uuid, DllBuildSave save)
    {
        if (WebSocketList.ContainsKey(uuid))
        {
            WebSocketList[uuid].Unload();
            WebSocketList[uuid].DllType = null;
            WebSocketList[uuid].MethodInfos.Clear();
            WebSocketList[uuid] = save;
        }
        else
        {
            WebSocketList.TryAdd(uuid, save);
        }
    }
    public static void RemoveWebSocket(string uuid)
    {
        if (WebSocketList.TryRemove(uuid, out var item))
        {
            item.Unload();
            item.DllType = null;
            item.MethodInfos.Clear();
        }
        RemoveAll(WebSocketLocal + uuid);
    }
    public static List<DllBuildSave> GetWebSocket()
    {
        return new List<DllBuildSave>(WebSocketList.Values);
    }

    public static void AddRobot(string uuid, DllBuildSave save)
    {
        if (RobotList.ContainsKey(uuid))
        {
            RobotList[uuid].Unload();
            RobotList[uuid].DllType = null;
            RobotList[uuid].MethodInfos.Clear();
            RobotList[uuid] = save;
        }
        else
        {
            RobotList.TryAdd(uuid, save);
        }
    }
    public static void RemoveRobot(string uuid)
    {
        if (RobotList.TryRemove(uuid, out var item))
        {
            item.Unload();
            item.DllType = null;
            item.MethodInfos.Clear();
        }
        RemoveAll(RobotLocal + uuid);
    }
    public static List<DllBuildSave> GetRobot()
    {
        return new List<DllBuildSave>(RobotList.Values);
    }

    public static void AddMqtt(string uuid, DllBuildSave save)
    {
        if (MqttList.ContainsKey(uuid))
        {
            MqttList[uuid].Unload();
            MqttList[uuid].DllType = null;
            MqttList[uuid].MethodInfos.Clear();
            MqttList[uuid] = save;
        }
        else
        {
            MqttList.TryAdd(uuid, save);
        }
    }
    public static void RemoveMqtt(string uuid)
    {
        if (MqttList.TryRemove(uuid, out var item))
        {
            item.Unload();
            item.DllType = null;
            item.MethodInfos.Clear();
        }
        RemoveAll(MqttLocal + uuid);
    }
    public static List<DllBuildSave> GetMqtt()
    {
        return new List<DllBuildSave>(MqttList.Values);
    }

    public static void AddTask(string uuid, DllBuildSave save)
    {
        if (TaskList.ContainsKey(uuid))
        {
            TaskList[uuid].Unload();
            TaskList[uuid].DllType = null;
            TaskList[uuid].MethodInfos.Clear();
            TaskList[uuid] = save;
        }
        else
        {
            TaskList.TryAdd(uuid, save);
        }
    }
    public static void RemoveTask(string uuid)
    {
        if (TaskList.TryRemove(uuid, out var item))
        {
            item.Unload();
            item.DllType = null;
            item.MethodInfos.Clear();
        }
        RemoveAll(RobotLocal + uuid);
    }
    public static DllBuildSave GetTask(string uuid)
    {
        return TaskList.TryGetValue(uuid, out var dll) ? dll : null;
    }
    public static void Start()
    {
        if (!Directory.Exists(DllLocal))
        {
            Directory.CreateDirectory(DllLocal);
        }
        if (!Directory.Exists(ClassLocal))
        {
            Directory.CreateDirectory(ClassLocal);
        }
        if (!Directory.Exists(SocketLocal))
        {
            Directory.CreateDirectory(SocketLocal);
        }
        if (!Directory.Exists(WebSocketLocal))
        {
            Directory.CreateDirectory(WebSocketLocal);
        }
        if (!Directory.Exists(RobotLocal))
        {
            Directory.CreateDirectory(RobotLocal);
        }
        if (!Directory.Exists(MqttLocal))
        {
            Directory.CreateDirectory(MqttLocal);
        }
        if (!Directory.Exists(TaskLocal))
        {
            Directory.CreateDirectory(TaskLocal);
        }
        if (!Directory.Exists(MqttLocal))
        {
            Directory.CreateDirectory(MqttLocal);
        }

        var dirs = Function.GetPathFileName(ClassLocal);
        foreach (var item in dirs)
        {
            try
            {
                if (item.FullName.Contains(".pdb"))
                    continue;
                LoadClass.LoadFile(item);
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        }
        dirs = Function.GetPathFileName(DllLocal);
        foreach (var item in dirs)
        {
            try
            {
                if (item.FullName.Contains(".pdb"))
                    continue;
                LoadDll.LoadFile(item);
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        }
        dirs = Function.GetPathFileName(SocketLocal);
        foreach (var item in dirs)
        {
            try
            {
                if (item.FullName.Contains(".pdb"))
                    continue;
                LoadSocket.LoadFile(item);
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        }
        dirs = Function.GetPathFileName(WebSocketLocal);
        foreach (var item in dirs)
        {
            try
            {
                if (item.FullName.Contains(".pdb"))
                    continue;
                LoadWebSocket.LoadFile(item);
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        }
        dirs = Function.GetPathFileName(RobotLocal);
        foreach (var item in dirs)
        {
            try
            {
                if (item.FullName.Contains(".pdb"))
                    continue;
                LoadRobot.LoadFile(item);
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        }
        dirs = Function.GetPathFileName(MqttLocal);
        foreach (var item in dirs)
        {
            try
            {
                if (item.FullName.Contains(".pdb"))
                    continue;
                LoadMqtt.LoadFile(item);
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        }
        dirs = Function.GetPathFileName(TaskLocal);
        foreach (var item in dirs)
        {
            try
            {
                if (item.FullName.Contains(".pdb"))
                    continue;
                LoadTask.LoadFile(item);
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        }
    }
}
