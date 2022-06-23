using ColoryrBuild.Windows;
using ColoryrWork.Lib.Build.Object;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ColoryrBuild.Views;

/// <summary>
/// ServerConfigView.xaml 的交互逻辑
/// </summary>
public partial class ServerConfigView : UserControl
{
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

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    private SelfObj Obj;
    public ServerConfigView()
    {
        Obj = new();
        InitializeComponent();
        DataContext = Obj;
        MainWindow.CallRefresh += Refresh;
    }

    private void Refresh()
    {
        GetHttpList();
        GetSocketConfig();
    }

    private async void GetSocketConfig() 
    {
        var res = await App.HttpUtils.GetSocketConfig();
        if (res == null)
        {
            _ = new InfoWindow("获取配置", "获取服务器Socket配置错误");
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
            _ = new InfoWindow("获取配置", "获取服务器Http配置错误");
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
            _ = new InfoWindow("添加配置", "添加服务器Http配置错误");
            return;
        }
        else if(!res1.Build)
        {
            _ = new InfoWindow("添加配置", res1.Message);
            return;
        }

        HttpList.Items.Add(new SocketConfig()
        {
            IP = ip,
            Port =  port
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
            _ = new InfoWindow("删除配置", "删除服务器Http配置错误");
            return;
        }
        else if (!res1.Build)
        {
            _ = new InfoWindow("删除配置", res1.Message);
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
        var res1 = await App.HttpUtils.AddHttpRoute(key, obj);
        if (res1 == null)
        {
            _ = new InfoWindow("添加反代", "服务器返回错误");
            return;
        }
        if (res1 == null)
        {
            _ = new InfoWindow("添加配置", "添加服务器反代配置错误");
            return;
        }
        else if (!res1.Build)
        {
            _ = new InfoWindow("添加配置", res1.Message);
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
            _ = new InfoWindow("删除配置", "删除服务器反代配置错误");
            return;
        }
        else if (!res1.Build)
        {
            _ = new InfoWindow("删除配置", res1.Message);
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
        var res1 = await App.HttpUtils.AddHttpUrlRoute(key, obj);
        if (res1 == null)
        {
            _ = new InfoWindow("添加反代", "服务器返回错误");
            return;
        }
        if (res1 == null)
        {
            _ = new InfoWindow("添加配置", "添加服务器Url反代配置错误");
            return;
        }
        else if (!res1.Build)
        {
            _ = new InfoWindow("添加配置", res1.Message);
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
            _ = new InfoWindow("删除配置", "删除服务器Url反代配置错误");
            return;
        }
        else if (!res1.Build)
        {
            _ = new InfoWindow("删除配置", res1.Message);
            return;
        }

        HttpUrlRouteList.SelectedItem = null;
        HttpUrlRouteList.Items.Remove(item);
    }

    private void AddRobotClick(object sender, RoutedEventArgs e) 
    {
        
    }

    private void DeleteRobotClick(object sender, RoutedEventArgs e)
    {

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