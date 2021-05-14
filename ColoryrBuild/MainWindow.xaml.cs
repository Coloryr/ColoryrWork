using ColoryrBuild.Windows;
using Lib.Build.Object;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace ColoryrBuild
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Dictionary<string, CSFileObj> DllList;
        public Dictionary<string, CSFileObj> ClassList;
        public Dictionary<string, CSFileObj> IoTList;
        public Dictionary<string, CSFileObj> RobotList;
        public Dictionary<string, CSFileObj> WebSocketList;
        public Dictionary<string, CSFileObj> MqttList;
        public Dictionary<string, CSFileObj> TaskList;
        public Dictionary<string, AppTempFileObj> AppList;
        public MainWindow()
        {
            InitializeComponent();
            App.MainWindow_ = this;
            GetApi();
            ReList();
        }

        private async void GetApi()
        {
            string dir = App.RunLocal + "CodeTEMP\\Api\\";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            foreach (var item in Directory.GetFiles(dir))
            {
                File.Delete(item);
            }
            var list = await App.HttpUtils.GetApi();
            foreach (var item in list.List)
            {
                File.WriteAllText(dir + item.Key + ".cs", item.Value);
            }
        }

        private void ReList()
        {
            ReDll();
            ReClass();
            ReIoT();
            ReRobot();
            ReWebSocket();
            ReApp();
            ReMqtt();
            ReTask();
        }

        private async void ReDll()
        {
            var list = await App.HttpUtils.GetList(CodeType.Dll);
            if (list == null)
            {
                App.LogShow("刷新", "DLL刷新失败");
                return;
            }
            DllList = list.List;
            ListDll.Items.Clear();
            foreach (var item in DllList)
            {
                ListDll.Items.Add(item.Value);
            }
            App.LogShow("刷新", "DLL刷新成功");
        }
        private async void ReClass()
        {
            var list = await App.HttpUtils.GetList(CodeType.Class);
            if (list == null)
            {
                App.LogShow("刷新", "Class刷新失败");
                return;
            }
            ClassList = list.List;
            ListClass.Items.Clear();
            foreach (var item in ClassList)
            {
                ListClass.Items.Add(item.Value);
            }
            App.LogShow("刷新", "Class刷新成功");
        }
        private async void ReIoT()
        {
            var list = await App.HttpUtils.GetList(CodeType.IoT);
            if (list == null)
            {
                App.LogShow("刷新", "IoT刷新失败");
                return;
            }
            IoTList = list.List;
            ListIoT.Items.Clear();
            foreach (var item in IoTList)
            {
                ListIoT.Items.Add(item.Value);
            }
            App.LogShow("刷新", "IoT刷新成功");
        }
        private async void ReRobot()
        {
            var list = await App.HttpUtils.GetList(CodeType.Robot);
            if (list == null)
            {
                App.LogShow("刷新", "Robot刷新失败");
                return;
            }
            RobotList = list.List;
            ListRobot.Items.Clear();
            foreach (var item in RobotList)
            {
                ListRobot.Items.Add(item.Value);
            }
            App.LogShow("刷新", "Robot刷新成功");
        }
        private async void ReWebSocket()
        {
            var list = await App.HttpUtils.GetList(CodeType.WebSocket);
            if (list == null)
            {
                App.LogShow("刷新", "WebSocket刷新失败");
                return;
            }
            WebSocketList = list.List;
            ListWebSocket.Items.Clear();
            foreach (var item in WebSocketList)
            {
                ListWebSocket.Items.Add(item.Value);
            }
            App.LogShow("刷新", "WebSocket刷新成功");
        }
        private async void ReApp()
        {
            var list = await App.HttpUtils.GetAppList();
            if (list == null)
            {
                App.LogShow("刷新", "App刷新失败");
                return;
            }
            AppList = list.List;
            ListApp.Items.Clear();
            foreach (var item in AppList)
            {
                ListApp.Items.Add(item.Value);
            }
            App.LogShow("刷新", "App刷新成功");
        }
        private async void ReMqtt()
        {
            var list = await App.HttpUtils.GetList(CodeType.Mqtt);
            if (list == null)
            {
                App.LogShow("刷新", "Mqtt刷新失败");
                return;
            }
            MqttList = list.List;
            ListMqtt.Items.Clear();
            foreach (var item in MqttList)
            {
                ListMqtt.Items.Add(item.Value);
            }
            App.LogShow("刷新", "Mqtt刷新成功");
        }
        private async void ReTask()
        {
            var list = await App.HttpUtils.GetList(CodeType.Task);
            if (list == null)
            {
                App.LogShow("刷新", "Task刷新失败");
                return;
            }
            TaskList = list.List;
            ListTask.Items.Clear();
            foreach (var item in TaskList)
            {
                ListTask.Items.Add(item.Value);
            }
            App.LogShow("刷新", "Task刷新成功");
        }
        public void Re(CodeType type)
        {
            switch (type)
            {
                case CodeType.Dll:
                    ReDll();
                    break;
                case CodeType.Class:
                    ReClass();
                    break;
                case CodeType.IoT:
                    ReIoT();
                    break;
                case CodeType.WebSocket:
                    ReWebSocket();
                    break;
                case CodeType.Robot:
                    ReRobot();
                    break;
                case CodeType.Task:
                    ReTask();
                    break;
                case CodeType.Mqtt:
                    ReMqtt();
                    break;
            }
        }
        private async void Add_Dll_Click(object sender, RoutedEventArgs e)
        {
            var data = new InputWindow("UUID设置").Set();
            if (string.IsNullOrWhiteSpace(data))
                return;
            var list = await App.HttpUtils.Add(CodeType.Dll, data);
            if (list == null)
            {
                App.LogShow("添加", "服务器返回错误");
                return;
            }
            App.LogShow("创建", list.Message);
            if (list.Build)
            {
                ReDll();
            }
        }
        private void Change_Dll_Click(object sender, RoutedEventArgs e)
        {
            if (ListDll.SelectedItem == null)
                return;
            var item = (CSFileObj)ListDll.SelectedItem;
            App.AddEdit(item, CodeType.Dll);
        }
        private async void Delete_Dll_Click(object sender, RoutedEventArgs e)
        {
            if (ListDll.SelectedItem == null)
                return;
            var item = (CSFileObj)ListDll.SelectedItem;
            var res = new ChoseWindow("删除确认", "是否要删除").Set();
            if (res)
            {
                var data = await App.HttpUtils.Remove(CodeType.Dll, item);
                if (data == null)
                {
                    App.LogShow("删除", "服务器返回错误");
                    return;
                }
                App.LogShow("删除", data.Message);
                if (data.Build)
                {
                    ReDll();
                }
            }
        }

        private void Re_Dll_Click(object sender, RoutedEventArgs e)
        {
            ReDll();
        }
        private void Clear_Dll_Click(object sender, RoutedEventArgs e)
        {
            InputDll.Text = "";
        }

        private async void Add_Class_Click(object sender, RoutedEventArgs e)
        {
            var data = new InputWindow("UUID设置").Set();
            if (string.IsNullOrWhiteSpace(data))
                return;
            var list = await App.HttpUtils.Add(CodeType.Class, data);
            if (list == null)
            {
                App.LogShow("添加", "服务器返回错误");
                return;
            }
            App.LogShow("创建", list.Message);
            if (list.Build)
            {
                ReClass();
            }
        }
        private void Change_Class_Click(object sender, RoutedEventArgs e)
        {
            if (ListClass.SelectedItem == null)
                return;
            var item = (CSFileObj)ListClass.SelectedItem;
            App.AddEdit(item, CodeType.Class);
        }
        private async void Delete_Class_Click(object sender, RoutedEventArgs e)
        {
            if (ListClass.SelectedItem == null)
                return;
            var item = (CSFileObj)ListClass.SelectedItem;
            var res = new ChoseWindow("删除确认", "是否要删除").Set();
            if (res)
            {
                var data = await App.HttpUtils.Remove(CodeType.Class, item);
                if (data == null)
                {
                    App.LogShow("删除", "服务器返回错误");
                    return;
                }
                App.LogShow("删除", data.Message);
                if (data.Build)
                {
                    ReClass();
                }
            }
        }
        private void Re_Class_Click(object sender, RoutedEventArgs e)
        {
            ReClass();
        }
        private void Clear_Class_Click(object sender, RoutedEventArgs e)
        {
            InputClass.Text = "";
        }

        private async void Add_IoT_Click(object sender, RoutedEventArgs e)
        {
            var data = new InputWindow("UUID设置").Set();
            if (string.IsNullOrWhiteSpace(data))
                return;
            var list = await App.HttpUtils.Add(CodeType.IoT, data);
            if (list == null)
            {
                App.LogShow("添加", "服务器返回错误");
                return;
            }
            App.LogShow("创建", list.Message);
            if (list.Build)
            {
                ReIoT();
            }
        }
        private void Change_IoT_Click(object sender, RoutedEventArgs e)
        {
            if (ListIoT.SelectedItem == null)
                return;
            var item = (CSFileObj)ListIoT.SelectedItem;
            App.AddEdit(item, CodeType.IoT);
        }
        private async void Delete_IoT_Click(object sender, RoutedEventArgs e)
        {
            if (ListIoT.SelectedItem == null)
                return;
            var item = (CSFileObj)ListIoT.SelectedItem;
            var res = new ChoseWindow("删除确认", "是否要删除").Set();
            if (res)
            {
                var data = await App.HttpUtils.Remove(CodeType.IoT, item);
                if (data == null)
                {
                    App.LogShow("删除", "服务器返回错误");
                    return;
                }
                App.LogShow("删除", data.Message);
                if (data.Build)
                {
                    ReIoT();
                }
            }
        }
        private void Re_IoT_Click(object sender, RoutedEventArgs e)
        {
            ReIoT();
        }
        private void Clear_IoT_Click(object sender, RoutedEventArgs e)
        {
            InputIoT.Text = "";
        }

        private async void Add_Robot_Click(object sender, RoutedEventArgs e)
        {
            var data = new InputWindow("UUID设置").Set();
            if (string.IsNullOrWhiteSpace(data))
                return;
            var list = await App.HttpUtils.Add(CodeType.Robot, data);
            if (list == null)
            {
                App.LogShow("添加", "服务器返回错误");
                return;
            }
            App.LogShow("创建", list.Message);
            if (list.Build)
            {
                ReRobot();
            }
        }
        private void Change_Robot_Click(object sender, RoutedEventArgs e)
        {
            if (ListRobot.SelectedItem == null)
                return;
            var item = (CSFileObj)ListRobot.SelectedItem;
            App.AddEdit(item, CodeType.Robot);
        }
        private async void Delete_Robot_Click(object sender, RoutedEventArgs e)
        {
            if (ListRobot.SelectedItem == null)
                return;
            var item = (CSFileObj)ListRobot.SelectedItem;
            var res = new ChoseWindow("删除确认", "是否要删除").Set();
            if (res)
            {
                var data = await App.HttpUtils.Remove(CodeType.Robot, item);
                if (data == null)
                {
                    App.LogShow("删除", "服务器返回错误");
                    return;
                }
                App.LogShow("删除", data.Message);
                if (data.Build)
                {
                    ReRobot();
                }
            }
        }
        private void Re_Robot_Click(object sender, RoutedEventArgs e)
        {
            ReRobot();
        }
        private void Clear_Robot_Click(object sender, RoutedEventArgs e)
        {
            InputRobot.Text = "";
        }

        private async void Add_WebSocket_Click(object sender, RoutedEventArgs e)
        {
            var data = new InputWindow("UUID设置").Set();
            if (string.IsNullOrWhiteSpace(data))
                return;
            var list = await App.HttpUtils.Add(CodeType.WebSocket, data);
            if (list == null)
            {
                App.LogShow("添加", "服务器返回错误");
                return;
            }
            App.LogShow("创建", list.Message);
            if (list.Build)
            {
                ReWebSocket();
            }
        }
        private void Change_WebSocket_Click(object sender, RoutedEventArgs e)
        {
            if (ListWebSocket.SelectedItem == null)
                return;
            var item = (CSFileObj)ListWebSocket.SelectedItem;
            App.AddEdit(item, CodeType.WebSocket);
        }
        private async void Delete_WebSocket_Click(object sender, RoutedEventArgs e)
        {
            if (ListWebSocket.SelectedItem == null)
                return;
            var item = (CSFileObj)ListWebSocket.SelectedItem;
            var res = new ChoseWindow("删除确认", "是否要删除").Set();
            if (res)
            {
                var data = await App.HttpUtils.Remove(CodeType.WebSocket, item);
                if (data == null)
                {
                    App.LogShow("删除", "服务器返回错误");
                    return;
                }
                App.LogShow("删除", data.Message);
                if (data.Build)
                {
                    ReWebSocket();
                }
            }
        }
        private void Re_WebSocket_Click(object sender, RoutedEventArgs e)
        {
            ReWebSocket();
        }
        private void Clear_WebSocket_Click(object sender, RoutedEventArgs e)
        {
            InputWebSocket.Text = "";
        }

        private async void Add_Mqtt_Click(object sender, RoutedEventArgs e)
        {
            var data = new InputWindow("UUID设置").Set();
            if (string.IsNullOrWhiteSpace(data))
                return;
            var list = await App.HttpUtils.Add(CodeType.Mqtt, data);
            if (list == null)
            {
                App.LogShow("添加", "服务器返回错误");
                return;
            }
            App.LogShow("创建", list.Message);
            if (list.Build)
            {
                ReMqtt();
            }
        }
        private void Change_Mqtt_Click(object sender, RoutedEventArgs e)
        {
            if (ListMqtt.SelectedItem == null)
                return;
            var item = (CSFileObj)ListMqtt.SelectedItem;
            App.AddEdit(item, CodeType.Mqtt);
        }
        private async void Delete_Mqtt_Click(object sender, RoutedEventArgs e)
        {
            if (ListMqtt.SelectedItem == null)
                return;
            var item = (CSFileObj)ListMqtt.SelectedItem;
            var res = new ChoseWindow("删除确认", "是否要删除").Set();
            if (res)
            {
                var data = await App.HttpUtils.Remove(CodeType.Mqtt, item);
                if (data == null)
                {
                    App.LogShow("删除", "服务器返回错误");
                    return;
                }
                App.LogShow("删除", data.Message);
                if (data.Build)
                {
                    ReMqtt();
                }
            }
        }
        private void Re_Mqtt_Click(object sender, RoutedEventArgs e)
        {
            ReMqtt();
        }
        private void Clear_Mqtt_Click(object sender, RoutedEventArgs e)
        {
            InputMqtt.Text = "";
        }

        private async void Add_Task_Click(object sender, RoutedEventArgs e)
        {
            var data = new InputWindow("UUID设置").Set();
            if (string.IsNullOrWhiteSpace(data))
                return;
            var list = await App.HttpUtils.Add(CodeType.Task, data);
            if (list == null)
            {
                App.LogShow("添加", "服务器返回错误");
                return;
            }
            App.LogShow("创建", list.Message);
            if (list.Build)
            {
                ReTask();
            }
        }
        private void Change_Task_Click(object sender, RoutedEventArgs e)
        {
            if (ListTask.SelectedItem == null)
                return;
            var item = (CSFileObj)ListTask.SelectedItem;
            App.AddEdit(item, CodeType.Task);
        }
        private async void Delete_Task_Click(object sender, RoutedEventArgs e)
        {
            if (ListTask.SelectedItem == null)
                return;
            var item = (CSFileObj)ListTask.SelectedItem;
            var res = new ChoseWindow("删除确认", "是否要删除").Set();
            if (res)
            {
                var data = await App.HttpUtils.Remove(CodeType.Task, item);
                if (data == null)
                {
                    App.LogShow("删除", "服务器返回错误");
                    return;
                }
                App.LogShow("删除", data.Message);
                if (data.Build)
                {
                    ReTask();
                }
            }
        }
        private void Re_Task_Click(object sender, RoutedEventArgs e)
        {
            ReTask();
        }
        private void Clear_Task_Click(object sender, RoutedEventArgs e)
        {
            InputTask.Text = "";
        }

        private async void Add_App_Click(object sender, RoutedEventArgs e)
        {
            var data = new InputWindow("UUID设置").Set();
            if (string.IsNullOrWhiteSpace(data))
                return;
            var list = await App.HttpUtils.Add(CodeType.App, data);
            if (list == null)
            {
                App.LogShow("添加", "服务器返回错误");
                return;
            }
            App.LogShow("创建", list.Message);
            if (list.Build)
            {
                ReApp();
            }
        }
        private void Change_App_Click(object sender, RoutedEventArgs e)
        {
            if (ListApp.SelectedItem == null)
                return;
            var item = (CSFileObj)ListApp.SelectedItem;
            App.AddEdit(item, CodeType.App);
        }
        private async void Delete_App_Click(object sender, RoutedEventArgs e)
        {
            if (ListApp.SelectedItem == null)
                return;
            var item = (CSFileObj)ListApp.SelectedItem;
            var res = new ChoseWindow("删除确认", "是否要删除").Set();
            if (res)
            {
                var data = await App.HttpUtils.Remove(CodeType.App, item);
                if (data == null)
                {
                    App.LogShow("删除", "服务器返回错误");
                    return;
                }
                App.LogShow("删除", data.Message);
                if (data.Build)
                {
                    ReApp();
                }
            }
        }
        private void Re_App_Click(object sender, RoutedEventArgs e)
        {
            ReApp();
        }
        private void Clear_App_Click(object sender, RoutedEventArgs e)
        {
            InputApp.Text = "";
        }
        private async void Key_App_Click(object sender, RoutedEventArgs e)
        {
            if (ListApp.SelectedItem == null)
                return;
            var item = ListApp.SelectedItem as AppTempFileObj;
            if (item == null)
                return;
            var res = new InputWindow("设置APP的Key", item.Key).Set();
            if (res == item.Key)
                return;
            if (string.IsNullOrWhiteSpace(res))
                return;
            var res1 = await App.HttpUtils.SetAppKey(item.UUID, res);
            App.LogShow("修改Key", res1.Message);
            if (res1.Build)
            {
                ReApp();
            }
        }
        private void Input_Dll_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputDll.Text))
            {
                ListDll.Items.Clear();
                foreach (var item in DllList)
                {
                    ListDll.Items.Add(item.Value);
                }
            }
            else
            {
                ListDll.Items.Clear();
                foreach (var item in DllList)
                {
                    if (item.Value.UUID.Contains(InputDll.Text))
                    {
                        ListDll.Items.Add(item.Value);
                    }
                }
            }
        }
        private void Input_Class_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputClass.Text))
            {
                ListClass.Items.Clear();
                foreach (var item in ClassList)
                {
                    ListClass.Items.Add(item.Value);
                }
            }
            else
            {
                ListClass.Items.Clear();
                foreach (var item in ClassList)
                {
                    if (item.Value.UUID.Contains(InputClass.Text))
                    {
                        ListClass.Items.Add(item.Value);
                    }
                }
            }
        }
        private void Input_IoT_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputIoT.Text))
            {
                ListIoT.Items.Clear();
                foreach (var item in IoTList)
                {
                    ListIoT.Items.Add(item.Value);
                }
            }
            else
            {
                ListIoT.Items.Clear();
                foreach (var item in IoTList)
                {
                    if (item.Value.UUID.Contains(InputIoT.Text))
                    {
                        ListIoT.Items.Add(item.Value);
                    }
                }
            }
        }
        private void Input_Robot_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputRobot.Text))
            {
                ListRobot.Items.Clear();
                foreach (var item in RobotList)
                {
                    ListRobot.Items.Add(item.Value);
                }
            }
            else
            {
                ListRobot.Items.Clear();
                foreach (var item in RobotList)
                {
                    if (item.Value.UUID.Contains(InputRobot.Text))
                    {
                        ListRobot.Items.Add(item.Value);
                    }
                }
            }
        }
        private void Input_WebSocket_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputWebSocket.Text))
            {
                ListWebSocket.Items.Clear();
                foreach (var item in WebSocketList)
                {
                    ListWebSocket.Items.Add(item.Value);
                }
            }
            else
            {
                ListWebSocket.Items.Clear();
                foreach (var item in WebSocketList)
                {
                    if (item.Value.UUID.Contains(InputWebSocket.Text))
                    {
                        ListWebSocket.Items.Add(item.Value);
                    }
                }
            }
        }
        private void Input_App_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputApp.Text))
            {
                ListApp.Items.Clear();
                foreach (var item in AppList)
                {
                    ListApp.Items.Add(item.Value);
                }
            }
            else
            {
                ListApp.Items.Clear();
                foreach (var item in AppList)
                {
                    if (item.Value.UUID.Contains(InputApp.Text))
                    {
                        ListApp.Items.Add(item.Value);
                    }
                }
            }
        }

        private void Window_Closed(object sender, CancelEventArgs e)
        {
            if (new ChoseWindow("关闭", "你确定要关闭编辑器吗").Set())
            {
                Application.Current.Shutdown();
            }
            else
            {
                e.Cancel = true;
            }
        }
    }
}
