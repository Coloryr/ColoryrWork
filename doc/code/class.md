# ColoryrServer

## 类代码编写
[返回](code.md)

默认的类代码  

```C#
using ColoryrServer.SDK;

[DLLIN]
public class test
{
    public test()
    {
             
    }

    public string getString(string data)
    {
        return data + "test";
    }
}
```

每个类代码**至少有一个类必须**带有[ColoryrServer.SDK.DLLIN](../../src/ColoryrServer/Core/SDK/NotesSDK.cs#L21)的属性 

类代码没有太多限制，也支持多文件  
只要注意类名不要冲突即可
