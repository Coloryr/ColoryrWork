using ColoryrWork.Lib.Build.Object;
using System.Diagnostics;

namespace ColoryrServer.Core.FileSystem.Vue;

public class VueBuild
{
    private const string CMDBuild = "vue build";

    private Process CMDProcess = new();//创建进程对象
    private WebObj web;

    public VueBuild(WebObj web)
    {
        this.web = web;
    }

    public void Init()
    {
        CMDProcess.StartInfo.FileName = "cmd.exe";//设置打开cmd命令窗口
        CMDProcess.StartInfo.UseShellExecute = false;//不使用操作系统shell启动进程的值
        CMDProcess.StartInfo.RedirectStandardInput = true;//设置可以从标准输入流读取值
        CMDProcess.StartInfo.RedirectStandardOutput = true;//设置可以向标准输出流写入值
        CMDProcess.StartInfo.RedirectStandardError = true;//设置可以显示输入输出流中出现的错误
        CMDProcess.StartInfo.CreateNoWindow = true;//设置在新窗口中启动进程
        CMDProcess.Start();//启动进程

        CMDProcess.OutputDataReceived += Process_OutputDataReceived;
    }

    private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        ServerMain.LogOut(e.Data);
    }

    public void Build()
    {
        CMDProcess.StandardInput.WriteLine(CMDBuild);
    }


}
