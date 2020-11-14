using Lib.Build.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ColoryrBuild
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool IsRe;

        private Dictionary<string, CSFileObj> DllMap = new Dictionary<string, CSFileObj>();
        private Dictionary<string, CSFileObj> ClassMap = new Dictionary<string, CSFileObj>();
        private Dictionary<string, CSFileObj> IoTMap = new Dictionary<string, CSFileObj>();
        private Dictionary<string, CSFileObj> WSMap = new Dictionary<string, CSFileObj>();
        private Dictionary<string, CSFileObj> RobotMap = new Dictionary<string, CSFileObj>();

        public MainWindow()
        {
            InitializeComponent();
            App.MainWindow_ = this;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DllRe();
            ClassRe();
            IoTRe();
            WSRe();
            RobotRe();
        }
        private async Task<CSFileList> GetList(ReType type)
        {
            var data = new BuildOBJ
            {
                Mode = type,
                User = App.Config.User,
                Token = App.Config.Token
            };
            var obj = await Http.GetAsync(data);
            if (obj == null)
            {
                App.Show("服务器错误");
                IsRe = false;
                return null;
            }
            var temp1 = obj.ToObject<ReMessage>();
            if (temp1.Message != null && temp1.Message == "233")
            {
                App.Show("登录失效");
                App.isLogin = false;
                App.SatrtLogin();
            }
            return obj.ToObject<CSFileList>();
        }
        public async void DllRe()
        {
            if (IsRe)
                return;
            IsRe = true;
            var temp = await GetList(ReType.GetDll);
            if (temp == null)
                return;
            DllMap.Clear();
            DllMap = temp.list;
            if (DllMap != null)
                Dispatcher.Invoke(() =>
                    {
                        CSList.Items.Clear();
                        foreach (var item in DllMap)
                        {
                            CSList.Items.Add(item.Value);
                        }
                    });
            else
                DllMap = new Dictionary<string, CSFileObj>();
            IsRe = false;
        }
        public async void ClassRe()
        {
            if (IsRe)
                return;
            IsRe = true;
            var temp = await GetList(ReType.GetClass);
            ClassMap.Clear();
            ClassMap = temp.list;
            if (ClassMap != null)
                Dispatcher.Invoke(() =>
                {
                    ClassList.Items.Clear();
                    foreach (var item in ClassMap)
                    {
                        ClassList.Items.Add(item.Value);
                    }
                });
            else
                ClassMap = new Dictionary<string, CSFileObj>();
            IsRe = false;
        }
        public async void IoTRe()
        {
            if (IsRe)
                return;
            IsRe = true;
            var temp = await GetList(ReType.GetIoT);
            IoTMap.Clear();
            IoTMap = temp.list;
            if (IoTMap != null)
                Dispatcher.Invoke(() =>
                {
                    IoTList.Items.Clear();
                    foreach (var item in IoTMap)
                    {
                        IoTList.Items.Add(item.Value);
                    }
                });
            else
                IoTMap = new Dictionary<string, CSFileObj>();
            IsRe = false;
        }
        public async void WSRe()
        {
            if (IsRe)
                return;
            IsRe = true;
            var temp = await GetList(ReType.GetWebSocket);
            WSMap.Clear();
            WSMap = temp.list;
            if (WSMap != null)
                Dispatcher.Invoke(() =>
                {
                    WSList.Items.Clear();
                    foreach (var item in WSMap)
                    {
                        WSList.Items.Add(item.Value);
                    }
                });
            else
                IoTMap = new Dictionary<string, CSFileObj>();
            IsRe = false;
        }
        public async void RobotRe()
        {
            if (IsRe)
                return;
            IsRe = true;
            var temp = await GetList(ReType.GetRobot);
            RobotMap.Clear();
            RobotMap = temp.list;
            if (IoTMap != null)
                Dispatcher.Invoke(() =>
                {
                    RobotList.Items.Clear();
                    foreach (var item in RobotMap)
                    {
                        RobotList.Items.Add(item.Value);
                    }
                });
            else
                RobotMap = new Dictionary<string, CSFileObj>();
            IsRe = false;
        }
        private void DllAdd(object sender, RoutedEventArgs e)
        {
            App.OpenCodeWin(new CSFileCode()
            {
                Type = CodeType.Dll
            });
        }
        private void ClassAdd(object sender, RoutedEventArgs e)
        {
            App.OpenCodeWin(new CSFileCode()
            {
                Type = CodeType.Class
            });
        }
        private void IoTAdd(object sender, RoutedEventArgs e)
        {
            App.OpenCodeWin(new CSFileCode()
            {
                Type = CodeType.IoT
            });
        }
        private void WSAdd(object sender, RoutedEventArgs e)
        {
            App.OpenCodeWin(new CSFileCode()
            {
                Type = CodeType.WebSocket
            });
        }
        private void RobotAdd(object sender, RoutedEventArgs e)
        {
            App.OpenCodeWin(new CSFileCode()
            {
                Type = CodeType.Robot
            });
        }

        private async void DllEdit(object sender, RoutedEventArgs e)
        {
            var Item = (CSFileObj)CSList.SelectedItem;
            if (Item == null)
                return;
            var SendData = new BuildOBJ
            {
                Mode = ReType.CodeDll,
                User = App.Config.User,
                Token = App.Config.Token,
                UUID = Item.UUID
            };
            var ReObj = await Http.GetAsync(SendData);
            if (ReObj == null)
                return;
            if (ReObj.ToObject<ReMessage>().Message == "233")
                App.SatrtLogin();
            else
                App.OpenCodeWin(ReObj.ToObject<CSFileCode>());
        }

        private async void ClassEdit(object sender, RoutedEventArgs e)
        {
            var Item = (CSFileObj)ClassList.SelectedItem;
            if (Item == null)
                return;
            var SendData = new BuildOBJ
            {
                Mode = ReType.CodeClass,
                User = App.Config.User,
                Token = App.Config.Token,
                UUID = Item.UUID
            };
            var ReObj = await Http.GetAsync(SendData);
            if (ReObj == null)
                return;
            if (ReObj.ToObject<ReMessage>().Message == "233")
                App.SatrtLogin();
            else
                App.OpenCodeWin(ReObj.ToObject<CSFileCode>());
        }
        private async void IoTEdit(object sender, RoutedEventArgs e)
        {
            var Item = (CSFileObj)IoTList.SelectedItem;
            if (Item == null)
                return;
            var SendData = new BuildOBJ
            {
                Mode = ReType.CodeIoT,
                User = App.Config.User,
                Token = App.Config.Token,
                UUID = Item.UUID
            };
            var ReObj = await Http.GetAsync(SendData);
            if (ReObj == null)
                return;
            if (ReObj.ToObject<ReMessage>().Message == "233")
                App.SatrtLogin();
            else
                App.OpenCodeWin(ReObj.ToObject<CSFileCode>());
        }
        private async void WSEdit(object sender, RoutedEventArgs e)
        {
            var Item = (CSFileObj)WSList.SelectedItem;
            if (Item == null)
                return;
            var SendData = new BuildOBJ
            {
                Mode = ReType.CodeWebSocket,
                User = App.Config.User,
                Token = App.Config.Token,
                UUID = Item.UUID
            };
            var ReObj = await Http.GetAsync(SendData);
            if (ReObj == null)
                return;
            if (ReObj.ToObject<ReMessage>().Message == "233")
                App.SatrtLogin();
            else
                App.OpenCodeWin(ReObj.ToObject<CSFileCode>());
        }
        private async void RobotEdit(object sender, RoutedEventArgs e)
        {
            var Item = (CSFileObj)RobotList.SelectedItem;
            if (Item == null)
                return;
            var SendData = new BuildOBJ
            {
                Mode = ReType.CodeRobot,
                User = App.Config.User,
                Token = App.Config.Token,
                UUID = Item.UUID
            };
            var ReObj = await Http.GetAsync(SendData);
            if (ReObj == null)
                return;
            if (ReObj.ToObject<ReMessage>().Message == "233")
                App.SatrtLogin();
            else
                App.OpenCodeWin(ReObj.ToObject<CSFileCode>());
        }

        private async void DllRemove(object sender, RoutedEventArgs e)
        {
            var Item = (CSFileObj)CSList.SelectedItem;
            if (Item == null)
                return;
            var SendData = new BuildOBJ
            {
                Mode = ReType.RemoveDll,
                User = App.Config.User,
                Token = App.Config.Token,
                UUID = Item.UUID
            };
            var ReObj = await Http.GetAsync(SendData);
            if (ReObj == null)
                return;
            var obj = ReObj.ToObject<ReMessage>();
            if (obj.Message == "233")
                App.SatrtLogin();
            else
            {
                if (obj.Build)
                {
                    App.Show("删除成功");
                }
                DllRe();
            }
        }
        private async void ClassRemove(object sender, RoutedEventArgs e)
        {
            var Item = (CSFileObj)ClassList.SelectedItem;
            if (Item == null)
                return;
            var SendData = new BuildOBJ
            {
                Mode = ReType.RemoveClass,
                User = App.Config.User,
                Token = App.Config.Token,
                UUID = Item.UUID
            };
            var ReObj = await Http.GetAsync(SendData);
            if (ReObj == null)
                return;
            var obj = ReObj.ToObject<ReMessage>();
            if (obj.Message == "233")
                App.SatrtLogin();
            else
            {
                if (obj.Build)
                {
                    App.Show("删除成功");
                }
                ClassRe();
            }
        }
        private async void IoTRemove(object sender, RoutedEventArgs e)
        {
            var Item = (CSFileObj)IoTList.SelectedItem;
            if (Item == null)
                return;
            var SendData = new BuildOBJ
            {
                Mode = ReType.RemoveIoT,
                User = App.Config.User,
                Token = App.Config.Token,
                UUID = Item.UUID
            };
            var ReObj = await Http.GetAsync(SendData);
            if (ReObj == null)
                return;
            var obj = ReObj.ToObject<ReMessage>();
            if (obj.Message == "233")
                App.SatrtLogin();
            else
            {
                if (obj.Build)
                {
                    App.Show("删除成功");
                }
                IoTRe();
            }
        }
        private async void WSRemove(object sender, RoutedEventArgs e)
        {
            var Item = (CSFileObj)WSList.SelectedItem;
            if (Item == null)
                return;
            var SendData = new BuildOBJ
            {
                Mode = ReType.RemoveWebSocket,
                User = App.Config.User,
                Token = App.Config.Token,
                UUID = Item.UUID
            };
            var ReObj = await Http.GetAsync(SendData);
            if (ReObj == null)
                return;
            var obj = ReObj.ToObject<ReMessage>();
            if (obj.Message == "233")
                App.SatrtLogin();
            else
            {
                if (obj.Build)
                {
                    App.Show("删除成功");
                }
                WSRe();
            }
        }
        private async void RobotRemove(object sender, RoutedEventArgs e)
        {
            var Item = (CSFileObj)RobotList.SelectedItem;
            if (Item == null)
                return;
            var SendData = new BuildOBJ
            {
                Mode = ReType.RemoveRobot,
                User = App.Config.User,
                Token = App.Config.Token,
                UUID = Item.UUID
            };
            var ReObj = await Http.GetAsync(SendData);
            if (ReObj == null)
                return;
            var obj = ReObj.ToObject<ReMessage>();
            if (obj.Message == "233")
                App.SatrtLogin();
            else
            {
                if (obj.Build)
                {
                    App.Show("删除成功");
                }
                RobotRe();
            }
        }

        private void ReClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            switch (button.Name)
            {
                case "Re1":
                    DllRe();
                    break;
                case "Re2":
                    ClassRe();
                    break;
                case "Re3":
                    IoTRe();
                    break;
                case "Re4":
                    WSRe();
                    break;
                case "Re5":
                    RobotRe();
                    break;
            }

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.Quit();
        }

        private async void BuildClick(object sender, RoutedEventArgs e)
        {
            CSFileObj Item = null;
            ReType type = ReType.AddDll;
            switch (Tab.SelectedIndex)
            {
                case 0:
                    if (CSList.SelectedItem != null)
                    {
                        Build1.IsEnabled = false;
                        Item = (CSFileObj)CSList.SelectedItem;
                        type = ReType.AddDll;
                    }
                    break;
                case 1:
                    if (ClassList.SelectedItem != null)
                    {
                        Build2.IsEnabled = false;
                        Item = (CSFileObj)ClassList.SelectedItem;
                        type = ReType.AddClass;
                    }
                    break;
                case 2:
                    if (IoTList.SelectedItem != null)
                    {
                        Build3.IsEnabled = false;
                        Item = (CSFileObj)IoTList.SelectedItem;
                        type = ReType.AddIoT;
                    }
                    break;
                case 3:
                    if (WSList.SelectedItem != null)
                    {
                        Build3.IsEnabled = false;
                        Item = (CSFileObj)WSList.SelectedItem;
                        type = ReType.AddWebSocket;
                    }
                    break;
                case 4:
                    if (RobotList.SelectedItem != null)
                    {
                        Build3.IsEnabled = false;
                        Item = (CSFileObj)RobotList.SelectedItem;
                        type = ReType.AddRobot;
                    }
                    break;
                default:
                    return;
            }
            if (Item == null)
                return;
            var CodeObj = CodeSave.GetFile(Item);
            if (CodeObj == null)
            {
                App.Show("代码不存在");
                return;
            }
            var SendData = new BuildOBJ
            {
                UUID = CodeObj.UUID,
                Code = CodeObj.Code,
                User = App.Config.User,
                Token = App.Config.Token,
                Text = CodeObj.Text,
                Mode = type
            };
            var ReObj = await Http.GetAsync(SendData);
            if (ReObj == null)
            {
                App.Show("编译服务器返回错误");
            }
            else
            {
                var temp = ReObj.ToObject<ReMessage>();
                if (temp.Build == true)
                {
                    App.Show("编译成功，用时" + temp.UseTime + "ms");
                    App.MainWindow_.DllRe();
                }
                else
                {
                    if (temp.Message == "233")
                    {
                        App.Show("登陆超时");
                        App.SatrtLogin();
                    }
                    else
                    {
                        App.Show("编译失败");
                        System.Windows.Forms.MessageBox.Show(temp.Message);
                    }
                }
            }
            Build1.IsEnabled = true;
            Build2.IsEnabled = true;
            Build3.IsEnabled = true;
            Build4.IsEnabled = true;
            Build5.IsEnabled = true;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            switch (Tab.SelectedIndex)
            {
                case 0:
                    string text = Search.Text;
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        CSList.Items.Clear();
                        foreach (var item in DllMap)
                        {
                            CSList.Items.Add(item.Value);
                        }
                        break;
                    }
                    var list = DllMap.Select(i => i.Value).Where(a => (a.UUID != null && a.UUID.Contains(text))
                    || (a.Text != null && a.Text.Contains(text))).ToList();
                    if (list.Count == 0)
                        return;
                    CSList.Items.Clear();
                    foreach (var item in list)
                    {
                        CSList.Items.Add(item);
                    }
                    break;
                case 1:
                    text = Search1.Text;
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        ClassList.Items.Clear();
                        foreach (var item in ClassMap)
                        {
                            ClassList.Items.Add(item.Value);
                        }
                        break;
                    }
                    list = ClassMap.Select(i => i.Value).Where(a => (a.UUID != null && a.UUID.Contains(text))
                    || (a.Text != null && a.Text.Contains(text))).ToList();
                    if (list.Count == 0)
                        return;
                    ClassList.Items.Clear();
                    foreach (var item in list)
                    {
                        ClassList.Items.Add(item);
                    }
                    break;
                case 2:
                    text = Search2.Text;
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        IoTList.Items.Clear();
                        foreach (var item in IoTMap)
                        {
                            IoTList.Items.Add(item.Value);
                        }
                        break;
                    }
                    list = IoTMap.Select(i => i.Value).Where(a => (a.UUID != null && a.UUID.Contains(text))
                    || (a.Text != null && a.Text.Contains(text))).ToList();
                    if (list.Count == 0)
                        return;
                    IoTList.Items.Clear();
                    foreach (var item in list)
                    {
                        IoTList.Items.Add(item);
                    }
                    break;
                case 3:
                    text = Search3.Text;
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        WSList.Items.Clear();
                        foreach (var item in WSMap)
                        {
                            WSList.Items.Add(item.Value);
                        }
                        break;
                    }
                    list = WSMap.Select(i => i.Value).Where(a => (a.UUID != null && a.UUID.Contains(text))
                    || (a.Text != null && a.Text.Contains(text))).ToList();
                    if (list.Count == 0)
                        return;
                    WSList.Items.Clear();
                    foreach (var item in list)
                    {
                        WSList.Items.Add(item);
                    }
                    break;
                case 4:
                    text = Search4.Text;
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        RobotList.Items.Clear();
                        foreach (var item in RobotMap)
                        {
                            RobotList.Items.Add(item.Value);
                        }
                        break;
                    }
                    list = RobotMap.Select(i => i.Value).Where(a => (a.UUID != null && a.UUID.Contains(text))
                    || (a.Text != null && a.Text.Contains(text))).ToList();
                    if (list.Count == 0)
                        return;
                    RobotList.Items.Clear();
                    foreach (var item in list)
                    {
                        RobotList.Items.Add(item);
                    }
                    break;
            }
        }
    }
}
