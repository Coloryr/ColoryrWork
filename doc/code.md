# ColoryrServer

业务代码编写

[返回](../README.md)

**默认的账户密码均为`Admin`**

[接口代码编写](./code/http.md)

[类代码编写](./code/class.md)

[Socket代码编写](./code/socket.md)

[机器人代码编写](./code/robot.md)

[WebSocket代码编写](./code/websocket.md)

[Mqtt代码编写](./code/mqtt.md)

[Service代码编写](./code/service.md)

[网页代码编写](./code/web.md)

[势力代码编写](./code/demo.md)

## 软路由

`接口`和`网页`类型需要注意软路由路径  
例如你的UUID设置为`WebApi/test`，则访问地址为
```
{服务器基地址}/WebApi/test
```

## 服务器SDK

服务器提供了一些重复使用的方法[ColoryrSDK](../src/ColoryrServer/Core/SDK/ColoryrSDK.cs)

数据库包装方法[DatabaseSDK](../src/ColoryrServer/Core/SDK/DatabaseSDK.cs)

HttpClient包装方法[HtmlSDK](../src/ColoryrServer/Core/SDK/HtmlSDK.cs)

Mqtt包装方法[MqttSDK](../src/ColoryrServer/Core/SDK/MqttSDK.cs)

机器人数据包[RobotSDK](../src/ColoryrServer/Core/SDK/RobotSDK.cs)

任务包装方法[TaskSDK](../src/ColoryrServer/Core/SDK/TaskSDK.cs)

Web资源包装方法[WebHtmlSDK](../src/ColoryrServer/Core/SDK/WebHtmlSDK.cs)

## Vue项目构建
Vue项目构建会执行指令
```
$ npm run build
```
因此需要nodejs运行环境，并且配置好环境变量才能正常编译

`node_modules`需要手动准备  
在文件夹`ColoryrServer/Codes/Static`下有一个`node_modules`文件夹，里面装的是Vue编译需要的库

如果你只使用框架自带的Vue项目
按照下面步骤做即可
1. 安装nodejs配置环境变量
2. 新建一个Vue项目
3. 在Vue项目文件夹下打开命令行输入
```
$ npm i
```
4. 结束后剪切`node_modules`文件夹到`ColoryrServer/Codes/Static`中去

注意：如果你使用自己的Vue项目要配置好路径
例如你的`网页项目`的UUID为`Web/test`，则需要在`vite.config.ts`或其他配置文件设置输出路径为`./Web/test/`
如果不设置会无法正确找到文件
