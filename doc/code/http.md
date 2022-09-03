# ColoryrServer

接口代码编写

[返回](../code.md)

默认的接口代码  
```C#
using ColoryrServer.SDK;

[DLLIN(true)]//true则报错输出至网页
public class app_FD578CDE687FD183ED8E321C7A1ACEC25BDAF9CF
{
    [NotesSDK("一个接口", new string[1]{ "输入" }, new string[1]{ "输出" })]
    public dynamic Main(HttpDllRequest http)
    {  
        return "true";
    }
}
```

该代码中包含一个[接口类](#接口类)和一个[接口函数](#接口函数)

## 接口类
**必须**带有[ColoryrServer.SDK.DLLIN](../../src/ColoryrServer/Core/SDK/NotesSDK.cs#L21)的属性  
`DLLIN`中可以设置报错是否输出到网页中去

一个接口类**必须**包含名字为`Main`的[接口函数](#接口函数)  
不是叫`Main`的[接口函数](#接口函数)都为识别为[子接口](#子接口)  

接口类**必须**是`public`否则将无法调用  

接口类的请求地址为
```
http://{服务器基地址}/{UUID}
``` 
例如你的接口UUID为`WebApi/test`，默认配置文件的情况下，则默认的URL地址为  
```
http://127.0.0.1/WebApi/test
```

## 接口函数
一个标准的接口函数  
在[接口类](#接口类)中，所有的`public`函数都为接口函数
```C#
[NotesSDK("一个接口", new string[1]{ "输入" }, new string[1]{ "输出" })]
public dynamic Main(HttpDllRequest http)
{  
    return "true";
}
```
接口函数输入的参数**必须**是[ColoryrServer.SDK.HttpDllRequest](../../src/ColoryrServer/Core/SDK/WebApiSDK.cs#L8)  
并且**必须**带有[ColoryrServer.SDK.NotesSDK](../../src/ColoryrServer/Core/SDK/NotesSDK.cs#L6)的属性

## 子接口
一个接口类里面可以有多个子接口  
子接口的请求地址为
```
http://{服务器基地址}/{UUID}/{name}
```  
例如下面的代码
```C#
using ColoryrServer.SDK;

[DLLIN(true)]
public class app_FD578CDE687FD183ED8E321C7A1ACEC25BDAF9CF
{
    [NotesSDK("主接口", new string[1]{ "输入" }, new string[1]{ "输出" })]
    public dynamic Main(HttpDllRequest http)
    {  
        return "true";
    }
    [NotesSDK("子接口", new string[1]{ "输入" }, new string[1]{ "输出" })]
    public dynamic push(HttpDllRequest http)
    {  
        return "ok";
    }
}
```
请求
```
http://127.0.0.1:25555/WebApi/test
```
后返回的数据为
```
test
```
请求
```
http://127.0.0.1:25555/WebApi/test/push
```
后返回的数据为
```
ok
```

### 接口请求参数
接口函数传入的对象为[ColoryrServer.SDK.HttpDllRequest](../../src/ColoryrServer/Core/SDK/WebApiSDK.cs#L8)  

### 接口返回类型
接口可以返回的类型可以是
- string
- [ColoryrServer.SDK.HttpResponseString](../../src/ColoryrServer/Core/SDK/WebApiSDK.cs#L98)
- [ColoryrServer.SDK.HttpResponseDictionary](../../src/ColoryrServer/Core/SDK/WebApiSDK.cs#L111)
- [ColoryrServer.SDK.HttpResponseStream](../../src/ColoryrServer/Core/SDK/WebApiSDK.cs#L144)
- [ColoryrServer.SDK.HttpResponseBytes](../../src/ColoryrServer/Core/SDK/WebApiSDK.cs#L160)

返回其他类型会尝试进行JSON序列化

