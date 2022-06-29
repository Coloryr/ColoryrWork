# ColoryrServer

## Task代码编写
[返回](code.md)

默认Task代码  

```C#
using System;
using ColoryrServer.SDK;

[DLLIN]
public class test
{
    public TaskRes Run(object[] args)
    {
        return new() 
        {
            Res = true
        };
    }

    public void OnStart()
    {
        
    }

    public void OnStop()
    {
        
    }
    
    public bool OnError(Exception e)
    {
        return false;
    }
}
```

类**必须**带有[ColoryrServer.SDK.DLLIN](../../src/ColoryrServer/Core/SDK/NotesSDK.cs#L21)的属性 

- `Run`表示任务内容
- `OnStart`表示服务器启动时执行的部分
- `OnStop`表示服务器关闭时执行的部分
- `OnError`表示服务器运行产生错误时执行的部分

返回如果为true，则这个事件不会传到下个Dll中去
