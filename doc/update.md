# ColoryrServer

更新日志

## 2.5.0计划
1. 服务器调试模式，可以本地IDE链接服务器进行调试  

## 2.4.0 同编辑器2.2.0
- 新增  
1. 服务器维护模式，开启后中止所有服务，仅允许编写代码  
服务器维护模式可以修改返回的html文件
2. 服务器代码重新编译，重新编译所有服务器代码
3. 编辑器每隔一段时间获取服务器日志

- 调整
1. `Task`修改为`Service`  
新的`Service`继承原有`Task`的部分功能  
去除运行次数限制，并且可以随服务器运行而启动  
同时编辑器可以查看状态、手动开启关闭`Service`  
2. 检查编辑器版本，旧版将提示升级
3. `TcpSocketRequest`更名为`SocketTcpRequest`  
`UdpSocketRequest`更名为`SocketUdpRequest`  

- 不兼容变更  
1. 所有`Task`代码失效
2. 数据库表`Code.task`,`CodeLog.task`失效
3. `FileLoad`中的`LoadString`与`LoadBytes`不兼容

## 2.3.0
- 新增  
1. RobotSDK调整
`MessageKey`，用于储存bot的消息ID  
`RobotMessage`新增`Permission`变量，用于判断`群成员权限`
`接口函数`可以静态实现  
2. debug编译和release编译选项
在代码中输入`//ColoryrServer_Release`就会进行release编译

- 调整  
1. 调整反射性能  
2. 更新ColorMiraiSDK  

- 不兼容变更  
1. RobotSDK调整  
`RobotSend`与`RobotMessage`中的`messageId`与旧版不兼容  
`RobotSend`与`RobotMessage`中的`MessageSourceKind`与旧版不兼容  
`RobotSend`与`RobotMessage`所有变量更名



