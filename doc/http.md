# ColoryrServer

## 接口代码编写
[返回](code.md)

默认的接口代码  
该代码中包含一个[接口类](#接口类)和一个[接口函数](#接口函数)
```C#
using ColoryrServer.SDK;

public class app_test
{
    [NotesSDK("一个接口", new string[1]{ "输入" }, new string[1]{ "输出" })]
    public dynamic main(HttpRequest http)
    {  
        return "true";
    }
}
```

## 接口类
接口类命名规则为`app_`再加上接口的UUID  
例如接口的UUID为`test`则类名为`app_test`  

一个接口类**必须**包含名字为`main`的接口函数  
不是叫`main`的接口函数都为识别为[子接口](#子接口)  

接口类**必须**是`public`否则将无法调用  

接口类的请求地址为
```
http://{serverIP}:{serverPore}/{WebAPI}/{UUID}
``` 
例如你的接口UUID为`test`，默认配置文件的情况下，则默认的URL地址为  
```
http://127.0.0.0.1:25555/WebAPI/test
```

## 接口函数
一个标准的接口函数  
在接口类中，所有的`public`函数都为接口函数
```C#
[NotesSDK("一个接口", new string[1]{ "输入" }, new string[1]{ "输出" })]
public dynamic main(HttpRequest http)
{  
    return "true";
}
```
接口函数输入的参数必须是[ColoryrServer.SDK.HttpRequest](../ColoryrServer/Core/SDK/HttpSDK.cs#L7)  
并且需要带有[ColoryrServer.SDK.NotesSDK](../ColoryrServer/Core/SDK/NotesSDK.cs)的属性

### 接口请求参数
接口函数传入的对象为[ColoryrServer.SDK.HttpRequest](../ColoryrServer/Core/SDK/HttpSDK.cs#L7)
```C#
public class HttpRequest
{
    public Dictionary<string, dynamic> Parameter { get; init; }
    public NameValueCollection RowRequest { get; init; }//原始请求的字符串
    public string Cookie { get; init; }
    public MyContentType ContentType { get; init; }
    public Stream Stream { get; init; }

    /// 获取参数
    /// </summary>
    /// <param name="arg">参数名</param>
    /// <returns>数据</returns>
    public dynamic GetParameter(string arg)
        => Parameter.ContainsKey(arg) ? Parameter[arg] : null;
}
```
### 接口返回类型
接口可以返回的类型可以是
- string
- Dictionary<string, object>
- [ColoryrServer.SDK.HttpResponseString](../ColoryrServer/Core/SDK/HttpSDK.cs#L68)
- [ColoryrServer.SDK.HttpResponseDictionary](../ColoryrServer/Core/SDK/HttpSDK.cs#L90)
- [ColoryrServer.SDK.HttpResponseStream](../ColoryrServer/Core/SDK/HttpSDK.cs#L123)
- [ColoryrServer.SDK.HttpResponseBytes](../ColoryrServer/Core/SDK/HttpSDK.cs#L139)

其他返回类型均会报错

## 子接口
一个接口类里面可以有多个子接口  
子接口的请求地址为
```
http://{serverIP}:{serverPore}/{WebAPI}/{UUID}/{name}
```  
例如下面的代码
```C#
using ColoryrServer.SDK;

public class app_test
{
    [NotesSDK("主接口", new string[1]{ "输入" }, new string[1]{ "输出" })]
    public dynamic main(HttpRequest http)
    {  
        return "true";
    }
    [NotesSDK("子接口", new string[1]{ "输入" }, new string[1]{ "输出" })]
    public dynamic push(HttpRequest http)
    {  
        return "ok";
    }
}
```
请求
```
http://127.0.0.0.1:25555/WebAPI/test
```
后返回的数据为
```
test
```
请求
```
http://127.0.0.0.1:25555/WebAPI/test/push
```
后返回的数据为
```
ok
```
