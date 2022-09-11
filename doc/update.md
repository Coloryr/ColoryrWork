# ColoryrServer

更新日志

## 2.3.0
- 新增  
`MessageKey`，用于储存bot的消息ID  
`RobotMessage`新增`Permission`变量，用于判断`群成员权限`
`接口函数`可以静态实现  
debug编译和release编译选项，在代码中输入`//ColoryrServer_Release`就会进行release编译

- 调整  
调整反射性能  
更新ColorMiraiSDK  

- 不兼容变更  
`RobotSend`与`RobotMessage`中的`messageId`与旧版不兼容  
`RobotSend`与`RobotMessage`中的`MessageSourceKind`与旧版不兼容  
`RobotSend`与`RobotMessage`所有变量更名



