# ColoryrServer

代码实例

[返回](../code.md)

## 一个简单的接口
```C#
using ColoryrServer.SDK;

[DLLIN(true)]//true则报错输出至网页
public class app_FD578CDE687FD183ED8E321C7A1ACEC25BDAF9CF
{
    public record Obj1
    {
        public string data { get; set; }
        public string arg { get; set; }
    }
    [NotesSDK("一个接口", new string[1]{ "输入" }, new string[1]{ "输出" })]
    public dynamic Main(HttpDllRequest http)
    {
        var arg = http.Get("arg");
        var data = http.Get("data");
        return new Obj1
        {
            data = data,
            arg = arg
        };
    }
}
```

## 返回一张图片
```C#
using ColoryrServer.SDK;

[DLLIN(true)]//true则报错输出至网页
public class app_FD578CDE687FD183ED8E321C7A1ACEC25BDAF9CF
{
    [NotesSDK("一个接口", new string[1]{ "输入" }, new string[1]{ "输出" })]
    public dynamic Main(HttpDllRequest http)
    {
        return new HttpResponseBytes()
        {
            Data = FileLoad.LoadBytes("test.jpg"),
            ContentType = ServerContentType.JPG
        };
    }
}
```

## 播放一个流视频
```C#
using ColoryrServer.SDK;

[DLLIN(true)]//true则报错输出至网页
public class app_FD578CDE687FD183ED8E321C7A1ACEC25BDAF9CF
{
    [NotesSDK("一个接口", new string[1]{ "输入" }, new string[1]{ "输出" })]
    public dynamic Main(HttpDllRequest http)
    {
        return FileLoad.StartStream(http, "test.mp4", ServerContentType.MP4);
    }
}
```

## 操作数据库
```C#
using ColoryrServer.SDK;
using System.Linq;

[DLLIN(true)]//true则报错输出至网页
public class app_FD578CDE687FD183ED8E321C7A1ACEC25BDAF9CF
{
    [NotesSDK("一个接口", new string[1]{ "输入" }, new string[1]{ "输出" })]
    public dynamic Main(HttpDllRequest http)
    {
        var arg = http.Get("arg");
        var sql = new Mysql("test");
        var data = sql.Query("select data from table1 where arg = @arg", new { arg });
        return data.First();
    }
}
```

## 进行一次http请求
```C#
using ColoryrServer.SDK;

[DLLIN(true)]//true则报错输出至网页
public class app_FD578CDE687FD183ED8E321C7A1ACEC25BDAF9CF
{
    [NotesSDK("一个接口", new string[1]{ "输入" }, new string[1]{ "输出" })]
    public dynamic Main(HttpDllRequest http)
    {
        using var client = new DllHttpClient();
        var data = client.GetString("http://127.0.0.1");
        return data;
    }
}
```

## 调用一个类
类代码
```C#
using ColoryrServer.SDK;

[DLLIN]
public class test
{
    public string GetString(string data)
    {
        return data + "test";
    }
}
```
接口代码
```C#
using ColoryrServer.SDK;

[DLLIN(true)]//true则报错输出至网页
public class app_FD578CDE687FD183ED8E321C7A1ACEC25BDAF9CF
{
    [NotesSDK("一个接口", new string[1]{ "输入" }, new string[1]{ "输出" })]
    public dynamic Main(HttpDllRequest http)
    {
        var test = new test();
        var data = test.GetString("data1");
        return data;
    }
}
```