using ColoryrBuild.Windows;
using ColoryrWork.Lib.Build.Object;
using System;
using System.ComponentModel;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace ColoryrBuild.Views;

/// <summary>
/// ServerConfigView.xaml 的交互逻辑
/// </summary>
public partial class ServerConfigView : UserControl
{
    public static Action Stop;
    private bool IsRun = false;
    private record EventObj
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    private record SocketConfigObj : INotifyPropertyChanged
    {
        private string _ip;
        private int _port;
        public string IP { get { return _ip; } set { _ip = value; OnPropertyChanged(nameof(IP)); } }
        public int Port { get { return _port; } set { _port = value; OnPropertyChanged(nameof(Port)); } }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    private record SelfObj : INotifyPropertyChanged
    {
        private SocketConfig _socket { get; set; }
        private SocketConfig _mqtt { get; set; }
        private SocketConfig _websocket { get; set; }
        private SocketConfig _robot { get; set; }

        public SocketConfig Socket
        {
            get { return _socket; }
            set { _socket = value; OnPropertyChanged(nameof(Socket)); }
        }
        public SocketConfig Mqtt
        {
            get { return _mqtt; }
            set { _mqtt = value; OnPropertyChanged(nameof(Mqtt)); }
        }
        public SocketConfig WebSocket
        {
            get { return _websocket; }
            set { _websocket = value; OnPropertyChanged(nameof(WebSocket)); }
        }
        public SocketConfig Robot
        {
            get { return _robot; }
            set { _robot = value; OnPropertyChanged(nameof(Robot)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    private SelfObj Obj;
    private Thread thread;
    public ServerConfigView()
    {
        Obj = new();
        InitializeComponent();
        DataContext = Obj;
        MainWindow.CallRefresh += Refresh;
        IsRun = true;
        thread = new(Run);
        thread.Start();
        Stop = () =>
        {
            IsRun = false;
        };
    }

    private async void Run()
    {
        while (IsRun)
        {
            try
            {
                if (App.IsLogin)
                {
                    var res = await App.HttpUtils.GetLog();
                    if (res != null && res.Build)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            ServerLog.AppendText(res.Message);
                        });
                    }
                }
                Thread.Sleep(5000);
            }
            catch (Exception e)
            {
                Dispatcher.Invoke(() =>
                {
                    InfoWindow.Show("运行错误", e.ToString());
                });
            }
        }
    }

    private void Refresh()
    {
        GetHttpList();
        GetSocketConfig();
        GetRobotConfig();
        GetUserConfig();
    }

    private async void GetUserConfig()
    {
        var res = await App.HttpUtils.GetAllUser();
        if (res == null)
        {
            InfoWindow.Show("获取配置", "获取服务器Socket配置错误");
            return;
        }
        UserList.Items.Clear();
        foreach (var item in res.List)
        {
            UserList.Items.Add(item);
        }
    }

    private async void GetRobotConfig()
    {
        var res = await App.HttpUtils.GetRobotConfig();
        if (res == null)
        {
            InfoWindow.Show("获取配置", "获取服务器Socket配置错误");
            return;
        }
        Obj.Robot = new()
        {
            IP = res.IP,
            Port = res.Port
        };
    }

    private async void GetSocketConfig()
    {
        var res = await App.HttpUtils.GetSocketConfig();
        if (res == null)
        {
            InfoWindow.Show("获取配置", "获取服务器Socket配置错误");
            return;
        }
        Obj.Socket = res.Socket;
        Obj.Mqtt = res.Mqtt;
        Obj.WebSocket = res.WebSocket;
    }

    private async void GetHttpList()
    {
        var res = await App.HttpUtils.GetHttpConfigList();
        if (res == null)
        {
            InfoWindow.Show("获取配置", "获取服务器Http配置错误");
            return;
        }
        HttpList.Items.Clear();
        HttpRouteList.Items.Clear();
        HttpUrlRouteList.Items.Clear();
        foreach (var item in res.HttpList)
        {
            HttpList.Items.Add(item);
        }
        foreach (var item in res.RouteList)
        {
            HttpRouteList.Items.Add(new KVConfig()
            {
                Key = item.Key,
                Value = item.Value.Url
            });
        }
        foreach (var item in res.UrlRouteList)
        {
            HttpUrlRouteList.Items.Add(new KVConfig()
            {
                Key = item.Key,
                Value = item.Value.Url
            });
        }
        EnableRoute.IsChecked = res.EnableRoute;
    }

    private async void AddHttpClick(object sender, RoutedEventArgs e)
    {
        string ip;
        int port;
        var res = new HttpEditWindow("127.0.0.1", 80).Set(out ip, out port);
        if (!res)
            return;

        var res1 = await App.HttpUtils.AddHttpConfig(ip, port);
        if (res1 == null)
        {
            InfoWindow.Show("添加配置", "添加服务器Http配置错误");
            return;
        }
        else if (!res1.Build)
        {
            InfoWindow.Show("添加配置", res1.Message);
            return;
        }

        HttpList.Items.Add(new SocketConfig()
        {
            IP = ip,
            Port = port
        });
    }

    private async void DeleteHttpClick(object sender, RoutedEventArgs e)
    {
        var item = HttpList.SelectedItem as SocketConfig;
        if (item == null)
            return;

        var res1 = await App.HttpUtils.AddHttpConfig(item.IP, item.Port);
        if (res1 == null)
        {
            InfoWindow.Show("删除配置", "删除服务器Http配置错误");
            return;
        }
        else if (!res1.Build)
        {
            InfoWindow.Show("删除配置", res1.Message);
            return;
        }

        HttpList.SelectedItem = null;
        HttpList.Items.Remove(item);
    }

    private async void AddRouteClick(object sender, RoutedEventArgs e)
    {
        string key;
        RouteConfigObj obj;
        var res = new RouteWindow().Set(out key, out obj);
        if (!res)
            return;
        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(obj.Url))
        {
            InfoWindow.Show("添加反代", "请输入合适的值");
            return;
        }

        var res1 = await App.HttpUtils.AddHttpRoute(key, obj);
        if (res1 == null)
        {
            InfoWindow.Show("添加反代", "服务器返回错误");
            return;
        }
        if (res1 == null)
        {
            InfoWindow.Show("添加配置", "添加服务器反代配置错误");
            return;
        }
        else if (!res1.Build)
        {
            InfoWindow.Show("添加配置", res1.Message);
            return;
        }

        HttpRouteList.Items.Add(new KVConfig()
        {
            Key = key,
            Value = obj.Url
        });
    }

    private async void DeleteRouteClick(object sender, RoutedEventArgs e)
    {
        var item = HttpRouteList.SelectedItem as KVConfig;
        if (item == null)
            return;

        var res1 = await App.HttpUtils.RemoveHttpRoute(item.Key);
        if (res1 == null)
        {
            InfoWindow.Show("删除配置", "删除服务器反代配置错误");
            return;
        }
        else if (!res1.Build)
        {
            InfoWindow.Show("删除配置", res1.Message);
            return;
        }

        HttpRouteList.SelectedItem = null;
        HttpRouteList.Items.Remove(item);
    }

    private async void AddUrlRouteClick(object sender, RoutedEventArgs e)
    {
        string key;
        RouteConfigObj obj;
        var res = new RouteWindow().Set(out key, out obj);
        if (!res)
            return;
        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(obj.Url))
        {
            InfoWindow.Show("添加反代", "请输入合适的值");
            return;
        }
        var res1 = await App.HttpUtils.AddHttpUrlRoute(key, obj);
        if (res1 == null)
        {
            InfoWindow.Show("添加反代", "服务器返回错误");
            return;
        }
        if (res1 == null)
        {
            InfoWindow.Show("添加配置", "添加服务器Url反代配置错误");
            return;
        }
        else if (!res1.Build)
        {
            InfoWindow.Show("添加配置", res1.Message);
            return;
        }

        HttpUrlRouteList.Items.Add(new KVConfig()
        {
            Key = key,
            Value = obj.Url
        });
    }

    private async void DeleteUrlRouteClick(object sender, RoutedEventArgs e)
    {
        var item = HttpUrlRouteList.SelectedItem as KVConfig;
        if (item == null)
            return;

        var res1 = await App.HttpUtils.RemoveHttpUrlRoute(item.Key);
        if (res1 == null)
        {
            InfoWindow.Show("删除配置", "删除服务器Url反代配置错误");
            return;
        }
        else if (!res1.Build)
        {
            InfoWindow.Show("删除配置", res1.Message);
            return;
        }

        HttpUrlRouteList.SelectedItem = null;
        HttpUrlRouteList.Items.Remove(item);
    }

    private async void AddUserClick(object sender, RoutedEventArgs e)
    {
        string user, password;
        var res = new Input1Window("新建用户", "用户名", "密码").Set(out user, out password);
        if (!res)
            return;
        if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password))
        {
            InfoWindow.Show("添加用户", "请输入用户信息");
            return;
        }

        var res1 = await App.HttpUtils.AddUser(user, password);
        if (res1 == null)
        {
            InfoWindow.Show("添加用户", "添加用户错误");
            return;
        }
        else if (!res1.Build)
        {
            InfoWindow.Show("添加用户", res1.Message);
            return;
        }
        GetUserConfig();
    }

    private async void DeleteUserClick(object sender, RoutedEventArgs e)
    {
        var item = UserList.SelectedItem as UserObj;
        if (item == null)
            return;

        var res = await App.HttpUtils.RemoveUser(item.User);
        if (res == null)
        {
            InfoWindow.Show("删除用户", "删除用户错误");
            return;
        }
        else if (!res.Build)
        {
            InfoWindow.Show("删除用户", res.Message);
            return;
        }

        UserList.Items.Remove(item);
    }

    private async void ButtonClick2(object sender, RoutedEventArgs e)
    {
        var res = new ChoseWindow("重启服务器", "服务器将在5秒后重启？").Set();
        if (!res)
            return;
        await App.HttpUtils.Reboot();
    }

    private async void SocketButtonClick(object sender, RoutedEventArgs e)
    {
        string ip = Obj.Socket.IP;
        int port = Obj.Socket.Port;
        if (IPAddress.TryParse(ip, out _) == false)
        {
            InfoWindow.Show("Socket设定", "参数非法");
            return;
        }
        if (string.IsNullOrWhiteSpace(ip) || port > 0xFFFF || port < 0)
        {
            InfoWindow.Show("Socket设定", "参数非法");
            return;
        }

        var res1 = await App.HttpUtils.SetSocket(ip, port, "socket");
        if (res1 == null)
        {
            InfoWindow.Show("修改Socket配置", "修改服务器Socket配置错误");
            return;
        }
        else if (!res1.Build)
        {
            InfoWindow.Show("修改Socket配置", res1.Message);
            return;
        }
    }

    private async void MqttButtonClick(object sender, RoutedEventArgs e)
    {
        string ip = Obj.Mqtt.IP;
        int port = Obj.Mqtt.Port;
        if (IPAddress.TryParse(ip, out _) == false)
        {
            InfoWindow.Show("Mqtt设定", "参数非法");
            return;
        }
        if (string.IsNullOrWhiteSpace(ip) || port > 0xFFFF || port < 0)
        {
            InfoWindow.Show("Mqtt设定", "参数非法");
            return;
        }

        var res1 = await App.HttpUtils.SetSocket(ip, port, "socket");
        if (res1 == null)
        {
            InfoWindow.Show("修改Mqtt配置", "修改服务器Mqtt配置错误");
            return;
        }
        else if (!res1.Build)
        {
            InfoWindow.Show("修改Mqtt配置", res1.Message);
            return;
        }
    }

    private async void WebSocketButtonClick(object sender, RoutedEventArgs e)
    {
        string ip = Obj.WebSocket.IP;
        int port = Obj.WebSocket.Port;
        if (IPAddress.TryParse(ip, out _) == false)
        {
            InfoWindow.Show("WebSocket设定", "参数非法");
            return;
        }
        if (string.IsNullOrWhiteSpace(ip) || port > 0xFFFF || port < 0)
        {
            InfoWindow.Show("WebSocket设定", "参数非法");
            return;
        }

        var res1 = await App.HttpUtils.SetSocket(ip, port, "socket");
        if (res1 == null)
        {
            InfoWindow.Show("修改WebSocket配置", "修改服务器WebSocket配置错误");
            return;
        }
        else if (!res1.Build)
        {
            InfoWindow.Show("修改WebSocket配置", res1.Message);
            return;
        }
    }

    private void ButtonClick3(object sender, RoutedEventArgs e)
    {
        GetRobotConfig();
    }

    private async void EnableRoute_Click(object sender, RoutedEventArgs e)
    {
        bool check = (bool)EnableRoute.IsChecked;
        var res = await App.HttpUtils.SetServerEnable(check, "Route");
        if (res == null)
        {
            EnableRoute.IsChecked = !check;
            InfoWindow.Show("路由设置", "服务器错误");
        }
        else if (!res.Build)
        {
            EnableRoute.IsChecked = !check;
            InfoWindow.Show("路由设置", res.Message);
        }
    }

    private void ButtonClick4(object sender, RoutedEventArgs e)
    {
        GetUserConfig();
    }

    private async void FixMode_Click(object sender, RoutedEventArgs e)
    {
        bool check = (bool)FixMode.IsChecked;
        var res = await App.HttpUtils.SetServerEnable(check, "FixMode");
        if (res == null)
        {
            FixMode.IsChecked = !check;
            InfoWindow.Show("维护模式设置", "服务器错误");
        }
        else if (!res.Build)
        {
            FixMode.IsChecked = !check;
            InfoWindow.Show("维护模式设置", res.Message);
        }
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        var res = await App.HttpUtils.Rebuild();
        if (res == null)
        {
            InfoWindow.Show("重新编译", "服务器错误");
        }
        else if (!res.Build)
        {
            InfoWindow.Show("重新编译", res.Message);
        }
    }

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
        ServerLog.Text = "";
    }

    private async void RobotButtonClick(object sender, RoutedEventArgs e)
    {
        string ip = Obj.Robot.IP;
        int port = Obj.Robot.Port;
        if (IPAddress.TryParse(ip, out _) == false)
        {
            InfoWindow.Show("Robot设定", "参数非法");
            return;
        }
        if (string.IsNullOrWhiteSpace(ip) || port > 0xFFFF || port < 0)
        {
            InfoWindow.Show("Robot设定", "参数非法");
            return;
        }

        var res1 = await App.HttpUtils.SetRobotConfig(ip, port);
        if (res1 == null)
        {
            InfoWindow.Show("修改Robot配置", "修改服务器Robot配置错误");
            return;
        }
        else if (!res1.Build)
        {
            InfoWindow.Show("修改Robot配置", res1.Message);
            return;
        }
    }

    private void ButtonClick1(object sender, RoutedEventArgs e)
    {
        GetSocketConfig();
    }

    private void ButtonClick(object sender, RoutedEventArgs e)
    {
        GetHttpList();
    }
}