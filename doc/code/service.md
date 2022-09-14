# ColoryrServer

Service代码编写

[返回](code.md)

Service分为四种类型
- `Normal`默认服务
- `ErrorDump`异常回调服务
- `OnlyOpen`单启动结束服务
- `Builder`编译回调服务

## 默认服务  

```C#
using System;
using ColoryrServer.SDK;

//ColoryrServer_Debug

[ServiceIN(false, ServiceType.Normal)] //true表示跟随服务器启动
public class test
{
    public ServiceNextState Run(object[] args, CancellationToken token)
    {
        return ServiceNextState.Stop;
    }

    public void OnStart()
    {
        
    }

    public void OnStop()
    {
        
    }
}
```

类**必须**带有[ColoryrServer.SDK.DLLIN](../../src/ColoryrServer/Core/SDK/NotesSDK.cs#L69)的属性
且`type`=`ServiceType.Normal`

- `Run`表示任务内容回调
- `OnStart`表示Service启动的回调
- `OnStop`表示Service关闭的回调

**任务内容回调会有一个至少50ms的调用间隔**

服务启动时会调用`OnStart`，之后会循环调用`Run`，关闭服务时会调用`OnStop`

默认服务有Init, Start, Ready, Going, Pause, Stop, Error, WaitArg状态  
通过[ServiceSDK](../../src/ColoryrServer/Core/SDK/ServiceSDK.cs#6)来操作Service
- `Init`表示服务正在初始化，完成后会转为`Start`
- `Start`表示服务正在调用`OnStart`，完成后会转为`Ready`
- `Ready`表示服务已初始化完成，且已启动，等待执行
- `Going`表示服务正在执行
- `Pause`表示服务正在暂停执行
- `Stop`表示服务正在调用`OnStop`，完成后会转为`Init`需要重新启动
- `Error`表示服务运行出错
- `WaitArg`等待运行参数设置后继续运行

## 异常回调服务  

```C#
using System;
using ColoryrServer.SDK;

//ColoryrServer_Debug

[ServiceIN(type: ServiceType.ErrorDump)]
public class test
{
    public bool OnError(Exception e)
    {
        return false; //true表示处理结束
    }
}
```

异常回调服务没有状态

类**必须**带有[ColoryrServer.SDK.DLLIN](../../src/ColoryrServer/Core/SDK/NotesSDK.cs#L69)的属性
且`type`=`ServiceType.ErrorDump`

- `OnError`表示服务器异常回调

返回`true`表示处理结束，不会继续传播事件

## 单启动结束服务
```C#
using System;
using ColoryrServer.SDK;

//ColoryrServer_Debug

[ServiceIN(false, ServiceType.OnlyOpen)] //true表示跟随服务器启动
public class test
{
    public void OnStart()
    {
        
    }

    public void OnStop()
    {
        
    }
}
```

单启动结束服务只有启动和关闭两种操作

## 编译回调服务
```C#
using System;
using ColoryrServer.SDK;

//ColoryrServer_Debug

[ServiceIN(type: ServiceType.Builder)] //true表示跟随服务器启动
public class test
{
    public bool OnBuild(PerBuildArg arg)
    {
        return false; //true表示处理结束
    }

    public bool OnBuild(PostBuildArg arg)
    {
        return false;
    }
}
```

- `OnBuild`编译回调
  - `PerBuildArg`编译前回调
  - `PostBuildArg`编译后回调