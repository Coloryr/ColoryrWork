﻿using ColoryrBuild.Views;
using ColoryrBuild.Windows;
using ColoryrWork.Lib.Build.Object;
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
        public delegate void Refresh();
        public static event Refresh CallRefresh;
        
        public Dictionary<string, CSFileObj> SocketList;
        public Dictionary<string, CSFileObj> RobotList;
        public Dictionary<string, CSFileObj> WebSocketList;
        public Dictionary<string, CSFileObj> MqttList;
        public Dictionary<string, CSFileObj> TaskList;
        public Dictionary<string, AppTempFileObj> AppList;
        public Dictionary<string, CSFileObj> WebList;
        public MainWindow()
        {
            InitializeComponent();
            App.MainWindow_ = this;
            GetApi();
            ReList();
        }

        private async void GetApi()
        {
            string dir = App.RunLocal + "CodeTEMP\\SDK\\";
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
            CallRefresh.Invoke();
            ReSocket();
            ReRobot();
            ReWebSocket();
            ReApp();
            ReMqtt();
            ReTask();
            ReWeb();
        }

        private async void ReSocket()
        {
            var list = await App.HttpUtils.GetList(CodeType.Socket);
            if (list == null)
            {
                App.LogShow("刷新", "Socket刷新失败");
                return;
            }
            SocketList = list.List;
            ListSocket.Items.Clear();
            foreach (var item in SocketList)
            {
                ListSocket.Items.Add(item.Value);
            }
            App.LogShow("刷新", "Socket刷新成功");
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
        private async void ReWeb()
        {
            var list = await App.HttpUtils.GetWebList();
            if (list == null)
            {
                App.LogShow("刷新", "Web刷新失败");
                return;
            }
            WebList = list.List;
            ListWeb.Items.Clear();
            foreach (var item in WebList)
            {
                ListWeb.Items.Add(item.Value);
            }
            App.LogShow("刷新", "Web刷新成功");
        }
        public void Re(CodeType type)
        {
            switch (type)
            {
                case CodeType.Dll:
                    TableView1.Action();
                    break;
                case CodeType.Class:
                    TableView2.Action();
                    break;
                case CodeType.Socket:
                    ReSocket();
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
                case CodeType.Web:
                    ReWeb();
                    break;
            }
        }
        private async void Add_Socket_Click(object sender, RoutedEventArgs e)
        {
            var data = new InputWindow("UUID设置").Set();
            if (string.IsNullOrWhiteSpace(data))
                return;
            var list = await App.HttpUtils.Add(CodeType.Socket, data);
            if (list == null)
            {
                App.LogShow("添加", "服务器返回错误");
                return;
            }
            App.LogShow("创建", list.Message);
            if (list.Build)
            {
                ReSocket();
            }
        }
        private void Change_Socket_Click(object sender, RoutedEventArgs e)
        {
            if (ListSocket.SelectedItem == null)
                return;
            var item = ListSocket.SelectedItem as CSFileObj;
            App.AddEdit(item, CodeType.Socket);
        }
        private async void Delete_Socket_Click(object sender, RoutedEventArgs e)
        {
            if (ListSocket.SelectedItem == null)
                return;
            var item = ListSocket.SelectedItem as CSFileObj;
            var res = new ChoseWindow("删除确认", "是否要删除").Set();
            if (res)
            {
                var data = await App.HttpUtils.Remove(CodeType.Socket, item);
                if (data == null)
                {
                    App.LogShow("删除", "服务器返回错误");
                    return;
                }
                App.LogShow("删除", data.Message);
                if (data.Build)
                {
                    ReSocket();
                }
            }
        }
        private void Re_Socket_Click(object sender, RoutedEventArgs e)
        {
            ReSocket();
        }
        private void Clear_Socket_Click(object sender, RoutedEventArgs e)
        {
            InputSocket.Text = "";
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
            var item = ListRobot.SelectedItem as CSFileObj;
            App.AddEdit(item, CodeType.Robot);
        }
        private async void Delete_Robot_Click(object sender, RoutedEventArgs e)
        {
            if (ListRobot.SelectedItem == null)
                return;
            var item = ListRobot.SelectedItem as CSFileObj;
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
            var item = ListWebSocket.SelectedItem as CSFileObj;
            App.AddEdit(item, CodeType.WebSocket);
        }
        private async void Delete_WebSocket_Click(object sender, RoutedEventArgs e)
        {
            if (ListWebSocket.SelectedItem == null)
                return;
            var item = ListWebSocket.SelectedItem as CSFileObj;
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
            var item = ListMqtt.SelectedItem as CSFileObj;
            App.AddEdit(item, CodeType.Mqtt);
        }
        private async void Delete_Mqtt_Click(object sender, RoutedEventArgs e)
        {
            if (ListMqtt.SelectedItem == null)
                return;
            var item = ListMqtt.SelectedItem as CSFileObj;
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

        private async void Add_Web_Click(object sender, RoutedEventArgs e)
        {
            var data = new InputWindow("UUID设置").Set();
            if (string.IsNullOrWhiteSpace(data))
                return;
            var list = await App.HttpUtils.Add(CodeType.Web, data);
            if (list == null)
            {
                App.LogShow("添加", "服务器返回错误");
                return;
            }
            App.LogShow("创建", list.Message);
            if (list.Build)
            {
                ReWeb();
            }
        }
        private void Change_Web_Click(object sender, RoutedEventArgs e)
        {
            if (ListWeb.SelectedItem == null)
                return;
            var item = ListWeb.SelectedItem as CSFileObj;
            App.AddEdit(item, CodeType.Web);
        }
        private async void Delete_Web_Click(object sender, RoutedEventArgs e)
        {
            if (ListWeb.SelectedItem == null)
                return;
            var item = ListWeb.SelectedItem as CSFileObj;
            var res = new ChoseWindow("删除确认", "是否要删除").Set();
            if (res)
            {
                var data = await App.HttpUtils.Remove(CodeType.Web, item);
                if (data == null)
                {
                    App.LogShow("删除", "服务器返回错误");
                    return;
                }
                App.LogShow("删除", data.Message);
                if (data.Build)
                {
                    ReWeb();
                }
            }
        }
        private void Re_Web_Click(object sender, RoutedEventArgs e)
        {
            ReWeb();
        }
        private void Clear_Web_Click(object sender, RoutedEventArgs e)
        {
            InputWeb.Text = "";
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
            var item = ListTask.SelectedItem as CSFileObj;
            App.AddEdit(item, CodeType.Task);
        }
        private async void Delete_Task_Click(object sender, RoutedEventArgs e)
        {
            if (ListTask.SelectedItem == null)
                return;
            var item = ListTask.SelectedItem as CSFileObj;
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
            var item = ListApp.SelectedItem as CSFileObj;
            App.AddEdit(item, CodeType.App);
        }
        private async void Delete_App_Click(object sender, RoutedEventArgs e)
        {
            if (ListApp.SelectedItem == null)
                return;
            var item = ListApp.SelectedItem as CSFileObj;
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
        
        
        private void Input_Socket_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputSocket.Text))
            {
                ListSocket.Items.Clear();
                foreach (var item in SocketList)
                {
                    ListSocket.Items.Add(item.Value);
                }
            }
            else
            {
                ListSocket.Items.Clear();
                foreach (var item in SocketList)
                {
                    if (item.Value.UUID.Contains(InputSocket.Text))
                    {
                        ListSocket.Items.Add(item.Value);
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

        private void Input_Web_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputWeb.Text))
            {
                ListWeb.Items.Clear();
                foreach (var item in WebList)
                {
                    ListWeb.Items.Add(item.Value);
                }
            }
            else
            {
                ListWeb.Items.Clear();
                foreach (var item in WebList)
                {
                    if (item.Value.UUID.Contains(InputWeb.Text))
                    {
                        ListWeb.Items.Add(item.Value);
                    }
                }
            }
        }

        private void Input_Task_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputTask.Text))
            {
                ListTask.Items.Clear();
                foreach (var item in TaskList)
                {
                    ListTask.Items.Add(item.Value);
                }
            }
            else
            {
                ListTask.Items.Clear();
                foreach (var item in TaskList)
                {
                    if (item.Value.UUID.Contains(InputTask.Text))
                    {
                        ListTask.Items.Add(item.Value);
                    }
                }
            }
        }

        private void Input_Mqtt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputMqtt.Text))
            {
                ListMqtt.Items.Clear();
                foreach (var item in MqttList)
                {
                    ListWeb.Items.Add(item.Value);
                }
            }
            else
            {
                ListMqtt.Items.Clear();
                foreach (var item in MqttList)
                {
                    if (item.Value.UUID.Contains(InputMqtt.Text))
                    {
                        ListMqtt.Items.Add(item.Value);
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
