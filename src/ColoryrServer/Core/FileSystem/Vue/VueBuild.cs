using ColoryrServer.Core.FileSystem.Html;
using ColoryrWork.Lib.Build.Object;
using System;
using System.Diagnostics;

namespace ColoryrServer.Core.FileSystem.Vue;

public class VueBuild
{
    private const string CMDBuild = "npm run build";

    private Process process = new();//创建进程对象
    private WebObj web;

    public string Res { get; private set; }

    public VueBuild(WebObj web)
    {
        this.web = web;
    }

    public void Init()
    {
        process.StartInfo.FileName = "cmd.exe";//设置打开cmd命令窗口
        process.StartInfo.UseShellExecute = false;//不使用操作系统shell启动进程的值
        process.StartInfo.RedirectStandardInput = true;//设置可以从标准输入流读取值
        process.StartInfo.RedirectStandardOutput = true;//设置可以向标准输出流写入值
        process.StartInfo.RedirectStandardError = true;//设置可以显示输入输出流中出现的错误
        process.StartInfo.CreateNoWindow = true;//设置在新窗口中启动进程
        process.StartInfo.WorkingDirectory = WebFileManager.WebCodeLocal + web.UUID;
        process.EnableRaisingEvents = true;

        process.OutputDataReceived += Process_OutputDataReceived;
        process.ErrorDataReceived += Process_OutputDataReceived;
        process.Exited += CMDProcess_Exited;

        process.Start();//启动进程

        process.BeginErrorReadLine();
        process.BeginOutputReadLine();
    }

    private void CMDProcess_Exited(object sender, EventArgs e)
    {
        VueBuildManager.BuildDone(web);
    }

    private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        Res += e.Data + Environment.NewLine;
    }

    public void Build()
    {
        process.StandardInput.WriteLine(CMDBuild);
        process.StandardInput.WriteLine("exit");
    }
}
