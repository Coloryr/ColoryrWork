﻿using ColoryrServer.Core.Dll.DllLoad;
using ColoryrServer.Core.Managers;
using ColoryrWork.Lib.Build.Object;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ColoryrServer.Core.Dll.Gen;

internal static class GenWebSocket
{
    /// <summary>
    /// 编译WebSocket
    /// </summary>
    /// <param name="obj">构建信息</param>
    /// <returns>编译结果</returns>
    public static GenReOBJ StartGen(CSFileCode obj, string user)
    {
        ServerMain.LogOut($"开始编译WebSocket[{obj.UUID}]");
        bool release = obj.Code.Contains(@"//ColoryrServer_Release");
        var build = GenCode.StartGen(obj.UUID, new()
        {
            CSharpSyntaxTree.ParseText(obj.Code)
        }, release ? OptimizationLevel.Release : OptimizationLevel.Debug);
        obj.UpdateTime = DateTime.Now.ToString();
        CodeManager.StorageWebSocket(obj, user);
        if (!build.Isok)
        {
            build.Res = $"WebSocket[{obj.UUID}]" + build.Res;
            ServerMain.LogWarn($"编译WebSocket[{obj.UUID}]错误");
            return build;
        }

        build.MS.Seek(0, SeekOrigin.Begin);
        build.MSPdb.Seek(0, SeekOrigin.Begin);

        ServerMain.LogOut($"编译WebSocket[{obj.UUID}]完成");

        var res = LoadWebSocket.Load(obj.UUID, build.MS, build.MSPdb);
        if (res != null)
            return res;

        Task.Run(() =>
        {
            build.MS.Seek(0, SeekOrigin.Begin);
            build.MSPdb.Seek(0, SeekOrigin.Begin);

            FileSystem.Managers.DllFileManager.SaveWebSocket(obj.UUID, build);

            build.MSPdb.Close();
            build.MSPdb.Dispose();

            build.MS.Close();
            build.MS.Dispose();
            GC.Collect();
        });

        return new GenReOBJ
        {
            Isok = true,
            Res = $"WebSocket[{obj.UUID}]编译完成\n{build.Res}",
            Time = obj.UpdateTime
        };
    }
}
