using ColoryrBuild.PostBuild;
using ColoryrBuild.Views;
using ColoryrBuild.Windows;
using ColoryrWork.Lib.Build;
using ColoryrWork.Lib.Build.Object;
using DiffPlex.DiffBuilder.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ColoryrBuild;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public const string Version = "2.0.0";
    /// <summary>
    /// 运行路径
    /// </summary>
    public static string RunLocal { get; private set; }

    public static HttpUtils HttpUtils;
    public static bool IsLogin { get; private set; }
    public static ConfigObj Config { get; private set; }
    public static MainWindow MainWindow_;
    public static Login LoginWindow_;

    private record CodeInfo
    {
        public string UUID { get; set; }
        public CodeType Type { get; set; }
    }

    private static Dictionary<CodeInfo, UserControl> CodeEdits = new();
    private static Logs Logs;

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        RunLocal = AppDomain.CurrentDomain.BaseDirectory;
        CodeSave.Start();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        Config = ConfigSave.Config(new ConfigObj
        {
            Name = "",
            Token = "",
            Http = "https://",
            AES = false,
            Key = "Key",
            IV = "IV"
        }, RunLocal + "Config.json");
        Logs = new Logs(RunLocal);
        HttpUtils = new();
        DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
    }

    public static void Close()
    {
        Current.Shutdown();
    }

    public static async Task<bool> AutoLogin()
    {
        IsLogin = await HttpUtils.AutoLogin();
        return IsLogin;
    }

    public static void Login()
    {
        IsLogin = false;
        if (LoginWindow_ == null)
            LoginWindow_ = new Login();

        LoginWindow_.ShowDialog();
    }

    public static DiffPaneModel StartContrast(CSFileCode obj, string old)
        => MainWindow_.Start(obj, old);

    public static DiffPaneModel StartContrast(CodeType type, string uuid, string new_, string old)
        => MainWindow_.Start(type, uuid, new_, old);

    public static void LogShow(string v1, string v2)
    {
        string data = v1 + "|" + v2;
        Logs.LogWrite(data);
        var date = DateTime.Now;
        string time = date.ToLongTimeString().ToString();
        LogView.AddLog?.Invoke($"[{time}]{data}");
    }

    private void NotifyIcon_Click(object sender, EventArgs e)
    {
        MainWindow_?.Activate();
    }

    public static async Task<bool> StartLogin(string Pass)
    {
        IsLogin = await HttpUtils.Login(Pass);
        ConfigSave.Save(Config, RunLocal + "Config.json");
        return IsLogin;
    }

    public static void AddEdit(CSFileObj code, CodeType type)
    {
        var info = new CodeInfo
        {
            UUID = code.UUID,
            Type = type
        };
        string name = type.ToString() + code.UUID;
        if (CodeEdits.ContainsKey(info))
        {
            var temp = CodeEdits[info] as IEditView;
            ColoryrBuild.MainWindow.SwitchTo(CodeEdits[info]);
            temp.GetCode();
        }
        else
        {
            UserControl view;
            if (type == CodeType.Web)
            {
                view = new CodeWebEditView(code);
            }
            else
            {
                view = new CodeCSEditView(code, type);
            }
            CodeEdits.Add(info, view);
            ColoryrBuild.MainWindow.AddCodeEdit(view);
        }
    }

    public static void CloseEdit(UserControl view)
    {
        var view1 = view as IEditView;
        var info = new CodeInfo
        {
            UUID = view1.Obj.UUID,
            Type = view1.Type
        };
        if (CodeEdits.ContainsKey(info))
        {
            CodeEdits.Remove(info);
        }
        ColoryrBuild.MainWindow.CloseCodeEdit(view);
    }

    private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        try
        {
            e.Handled = true;
            MessageBox.Show("捕获未处理异常:" + e.Exception.ToString());
        }
        catch (Exception ex)
        {
            MessageBox.Show("发生错误" + ex.ToString());
        }

    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        StringBuilder sbEx = new StringBuilder();
        if (e.IsTerminating)
        {
            sbEx.Append("发生错误，将关闭\n");
        }
        sbEx.Append("捕获未处理异常：");
        if (e.ExceptionObject is Exception)
        {
            sbEx.Append(((Exception)e.ExceptionObject).ToString());
        }
        else
        {
            sbEx.Append(e.ExceptionObject);
        }
        MessageBox.Show(sbEx.ToString());
    }

    private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        MessageBox.Show("捕获线程内未处理异常：" + e.Exception.ToString());
        e.SetObserved();
    }
}
