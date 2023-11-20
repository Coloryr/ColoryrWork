using ColoryrServer.Core.Dll;
using ColoryrServer.Core.Http;
using ColoryrServer.SDK;
using ColoryrWork.Lib.Build.Object;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json.Nodes;

namespace ColoryrServer.ASP.Test;

[DllIN(true)]//true则报错输出至网页
public class App_DLL
{
    [NotesSDK("一个静态异步接口", ["输入"], ["输出"])]
    public static async Task<object> Main(HttpDllRequest http)
    {
        var data = await http.GetBody(JsonType.SystemJson);
        return "true";
    }

    [NotesSDK("一个静态接口", ["输入"], ["输出"])]
    public static object Test(HttpDllRequest http)
    {
        var data = http.GetBody(JsonType.SystemJson).Result;
        return "true";
    }

    [NotesSDK("一个异步接口", ["输入"], ["输出"])]
    public async Task<object> Test1(HttpDllRequest http)
    {
        var data = await http.GetBody(JsonType.SystemJson);
        return "true";
    }

    [NotesSDK("一个接口", ["输入"], ["输出"])]
    public object Test2(HttpDllRequest http)
    {
        var data = http.GetBody(JsonType.SystemJson).Result;
        return "true";
    }
}

[DllIN(true)]//true则报错输出至网页
public class App_DLL_DATA
{
    [NotesSDK("一个静态异步接口", ["输入"], ["输出"])]
    public static async Task<object> Main(HttpDllRequest http)
    {
        var data = await http.GetBody(JsonType.SystemJson);
        if (data.Item2.TryGetValue("key", out var data1))
        {
            return data1.ToString();
        }
        return "true";
    }

    [NotesSDK("一个静态接口", ["输入"], ["输出"])]
    public static object Test(HttpDllRequest http)
    {
        var data = http.GetBody(JsonType.SystemJson).Result;
        if (data.Item2.TryGetValue("key", out var data1))
        {
            return data1.ToString();
        }
        return "true";
    }

    [NotesSDK("一个异步接口", ["输入"], ["输出"])]
    public async Task<object> Test1(HttpDllRequest http)
    {
        var data = await http.GetBody(JsonType.SystemJson);
        if (data.Item2.TryGetValue("key", out var data1))
        {
            return data1.ToString();
        }
        return "true";
    }

    [NotesSDK("一个接口", ["输入"], ["输出"])]
    public object Test2(HttpDllRequest http)
    {
        var data = http.GetBody(JsonType.SystemJson).Result;
        if (data.Item2.TryGetValue("key", out var data1))
        {
            return data1.ToString();
        }
        return "true";
    }
}

[DllIN(true)]//true则报错输出至网页
public class App_DLL_DATA_ARRAY
{
    private static int Add(JsonArray array)
    {
        int count = 0;
        foreach (var item in array)
        {
            count += (int)item!;
        }

        return count;
    }
    [NotesSDK("一个静态异步接口", ["输入"], ["输出"])]
    public static async Task<object> Main(HttpDllRequest http)
    {
        var data = await http.GetBody(JsonType.SystemJson);
        if (data.Item2.TryGetValue("", out var data1) && data1 is JsonArray array)
        {
            return Add(array).ToString();
        }
        return "true";
    }

    [NotesSDK("一个静态接口", ["输入"], ["输出"])]
    public static object Test(HttpDllRequest http)
    {
        var data = http.GetBody(JsonType.SystemJson).Result;
        if (data.Item2.TryGetValue("", out var data1) && data1 is JsonArray array)
        {
            return Add(array).ToString();
        }
        return "true";
    }

    [NotesSDK("一个异步接口", ["输入"], ["输出"])]
    public async Task<object> Test1(HttpDllRequest http)
    {
        var data = await http.GetBody(JsonType.SystemJson);
        if (data.Item2.TryGetValue("", out var data1) && data1 is JsonArray array)
        {
            return Add(array).ToString();
        }
        return "true";
    }

    [NotesSDK("一个接口", ["输入"], ["输出"])]
    public object Test2(HttpDllRequest http)
    {
        var data = http.GetBody(JsonType.SystemJson).Result;
        if (data.Item2.TryGetValue("", out var data1) && data1 is JsonArray array)
        {
            return Add(array).ToString();
        }
        return "true";
    }
}

public class ServerTest
{
    private HttpClient client;

    private void LoadDll()
    {
        string name = "dll/test";
        var assembly = new DllAssembly(CodeType.Dll, name)
        {
            SelfType = typeof(App_DLL)
        };
        var attr = assembly.SelfType.GetCustomAttribute<DllIN>(true);
        assembly.Debug = attr.Debug;

        foreach (var item in assembly.SelfType.GetMethods())
        {
            if (item.Name is "GetType" or "ToString" or "Equals" or "GetHashCode"
                || !item.IsPublic)
                continue;
            assembly.MethodInfos.Add(item.Name, item);
        }

        HttpInvokeRoute.AddDll(name, assembly);
    }

    private void LoadDllData()
    {
        string name = "dll/data";
        var assembly = new DllAssembly(CodeType.Dll, name)
        {
            SelfType = typeof(App_DLL_DATA)
        };
        var attr = assembly.SelfType.GetCustomAttribute<DllIN>(true);
        assembly.Debug = attr.Debug;

        foreach (var item in assembly.SelfType.GetMethods())
        {
            if (item.Name is "GetType" or "ToString" or "Equals" or "GetHashCode"
                || !item.IsPublic)
                continue;
            assembly.MethodInfos.Add(item.Name, item);
        }

        HttpInvokeRoute.AddDll(name, assembly);
    }

    private void LoadDllDataArray()
    {
        string name = "dll/array";
        var assembly = new DllAssembly(CodeType.Dll, name)
        {
            SelfType = typeof(App_DLL_DATA_ARRAY)
        };
        var attr = assembly.SelfType.GetCustomAttribute<DllIN>(true);
        assembly.Debug = attr.Debug;

        foreach (var item in assembly.SelfType.GetMethods())
        {
            if (item.Name is "GetType" or "ToString" or "Equals" or "GetHashCode"
                || !item.IsPublic)
                continue;
            assembly.MethodInfos.Add(item.Name, item);
        }

        HttpInvokeRoute.AddDll(name, assembly);
    }

    private void LoadStatic()
    {
        string name = "web/test";
        var obj = new WebObj()
        {
            IsVue = false,
            Codes = new()
            {
                { "index.html", "web1" },
                { "data.html", "web2"}
            },
            Files = []
        };
        HttpInvokeRoute.AddWeb(name, obj);
    }

    [SetUp]
    public void Setup()
    {
        client = new();

        Task.Run(ASPServer.Main);

        while (ASPServer.IsStarting)
        {
            Thread.Sleep(500);
        }

        LoadDll();
        LoadDllData();
        LoadDllDataArray();
        LoadStatic();
    }

    [Test]
    public async Task ServerGet()
    {
        var str = await client.GetStringAsync("http://localhost:8080/dll/test");
        Assert.That(str, Is.EqualTo("true"));

        str = await client.GetStringAsync("http://localhost:8080/dll/test/Test");
        Assert.That(str, Is.EqualTo("true"));

        str = await client.GetStringAsync("http://localhost:8080/dll/test/Test1");
        Assert.That(str, Is.EqualTo("true"));

        str = await client.GetStringAsync("http://localhost:8080/dll/test/Test2");
        Assert.That(str, Is.EqualTo("true"));
    }

    [Test]
    public async Task ServerPost()
    {
        var content = new StringContent("");
        var res = await client.PostAsync("http://localhost:8080/dll/test", content);
        var str = await res.Content.ReadAsStringAsync();
        Assert.That(str, Is.EqualTo("true"));

        res = await client.PostAsync("http://localhost:8080/dll/test/Test", content);
        str = await res.Content.ReadAsStringAsync();
        Assert.That(str, Is.EqualTo("true"));

        res = await client.PostAsync("http://localhost:8080/dll/test/Test1", content);
        str = await res.Content.ReadAsStringAsync();
        Assert.That(str, Is.EqualTo("true"));

        res = await client.PostAsync("http://localhost:8080/dll/test/Test2", content);
        str = await res.Content.ReadAsStringAsync();
        Assert.That(str, Is.EqualTo("true"));
    }

    [Test]
    public async Task ServerPut()
    {
        var content = new StringContent("");
        var res = await client.PutAsync("http://localhost:8080/dll/test", content);
        var str = await res.Content.ReadAsStringAsync();
        Assert.That(str, Is.EqualTo("true"));

        res = await client.PutAsync("http://localhost:8080/dll/test/Test", content);
        str = await res.Content.ReadAsStringAsync();
        Assert.That(str, Is.EqualTo("true"));

        res = await client.PutAsync("http://localhost:8080/dll/test/Test1", content);
        str = await res.Content.ReadAsStringAsync();
        Assert.That(str, Is.EqualTo("true"));

        res = await client.PutAsync("http://localhost:8080/dll/test/Test2", content);
        str = await res.Content.ReadAsStringAsync();
        Assert.That(str, Is.EqualTo("true"));
    }

    [Test]
    public async Task ServerDelete()
    {
        var res = await client.DeleteAsync("http://localhost:8080/dll/test");
        var str = await res.Content.ReadAsStringAsync();
        Assert.That(str, Is.EqualTo("true"));

        res = await client.DeleteAsync("http://localhost:8080/dll/test/Test");
        str = await res.Content.ReadAsStringAsync();
        Assert.That(str, Is.EqualTo("true"));

        res = await client.DeleteAsync("http://localhost:8080/dll/test/Test1");
        str = await res.Content.ReadAsStringAsync();
        Assert.That(str, Is.EqualTo("true"));

        res = await client.DeleteAsync("http://localhost:8080/dll/test/Test2");
        str = await res.Content.ReadAsStringAsync();
        Assert.That(str, Is.EqualTo("true"));
    }

    [Test]
    public async Task ServerGetData()
    {
        var str = await client.GetStringAsync("http://localhost:8080/dll/data?key=data1");
        Assert.That(str, Is.EqualTo("data1"));

        str = await client.GetStringAsync("http://localhost:8080/dll/data/Test?key=data1");
        Assert.That(str, Is.EqualTo("data1"));

        str = await client.GetStringAsync("http://localhost:8080/dll/data/Test1?key=data1");
        Assert.That(str, Is.EqualTo("data1"));

        str = await client.GetStringAsync("http://localhost:8080/dll/data/Test2?key=data1");
        Assert.That(str, Is.EqualTo("data1"));
    }

    [Test]
    public async Task ServerPostDataJson()
    {
        var type = MediaTypeHeaderValue.Parse("application/json");
        var content = new StringContent("{\"key\":\"data1\"}", type);
        var res = await client.PutAsync("http://localhost:8080/dll/data", content);
        var str = await res.Content.ReadAsStringAsync();
        Assert.That(str, Is.EqualTo("data1"));

        res = await client.PutAsync("http://localhost:8080/dll/data/Test", content);
        str = await res.Content.ReadAsStringAsync();
        Assert.That(str, Is.EqualTo("data1"));

        res = await client.PutAsync("http://localhost:8080/dll/data/Test1", content);
        str = await res.Content.ReadAsStringAsync();
        Assert.That(str, Is.EqualTo("data1"));

        res = await client.PutAsync("http://localhost:8080/dll/data/Test2", content);
        str = await res.Content.ReadAsStringAsync();
        Assert.That(str, Is.EqualTo("data1"));
    }

    [Test]
    public async Task ServerPostDataFrom()
    {
        var data = new Dictionary<string, string>() { { "key", "data1" } };
        var content = new FormUrlEncodedContent(data);
        var res = await client.PutAsync("http://localhost:8080/dll/data", content);
        var str = await res.Content.ReadAsStringAsync();
        Assert.That(str, Is.EqualTo("data1"));

        res = await client.PutAsync("http://localhost:8080/dll/data/Test", content);
        str = await res.Content.ReadAsStringAsync();
        Assert.That(str, Is.EqualTo("data1"));

        res = await client.PutAsync("http://localhost:8080/dll/data/Test1", content);
        str = await res.Content.ReadAsStringAsync();
        Assert.That(str, Is.EqualTo("data1"));

        res = await client.PutAsync("http://localhost:8080/dll/data/Test2", content);
        str = await res.Content.ReadAsStringAsync();
        Assert.That(str, Is.EqualTo("data1"));
    }

    [Test]
    public async Task ServerPostDataMultipartFrom()
    {
        var content = new MultipartFormDataContent
        {
            { new StringContent("data1"), "key" }
        };
        var res = await client.PutAsync("http://localhost:8080/dll/data", content);
        var str = await res.Content.ReadAsStringAsync();
        Assert.That(str, Is.EqualTo("data1"));

        res = await client.PutAsync("http://localhost:8080/dll/data/Test", content);
        str = await res.Content.ReadAsStringAsync();
        Assert.That(str, Is.EqualTo("data1"));

        res = await client.PutAsync("http://localhost:8080/dll/data/Test1", content);
        str = await res.Content.ReadAsStringAsync();
        Assert.That(str, Is.EqualTo("data1"));

        res = await client.PutAsync("http://localhost:8080/dll/data/Test2", content);
        str = await res.Content.ReadAsStringAsync();
        Assert.That(str, Is.EqualTo("data1"));
    }

    [Test]
    public async Task ServerPostDataJsonArray()
    {
        var type = MediaTypeHeaderValue.Parse("application/json");
        var content = new StringContent("[123,456,789]", type);
        var res = await client.PutAsync("http://localhost:8080/dll/array", content);
        var str = await res.Content.ReadAsStringAsync();
        Assert.That(str, Is.EqualTo("1368"));

        res = await client.PutAsync("http://localhost:8080/dll/array/Test", content);
        str = await res.Content.ReadAsStringAsync();
        Assert.That(str, Is.EqualTo("1368"));

        res = await client.PutAsync("http://localhost:8080/dll/array/Test1", content);
        str = await res.Content.ReadAsStringAsync();
        Assert.That(str, Is.EqualTo("1368"));

        res = await client.PutAsync("http://localhost:8080/dll/array/Test2", content);
        str = await res.Content.ReadAsStringAsync();
        Assert.That(str, Is.EqualTo("1368"));
    }

    [Test]
    public async Task ServerGetDataJson()
    {
        var str = await client.GetStringAsync("http://localhost:8080/dll/data?{\"key\":\"data1\"}");
        Assert.That(str, Is.EqualTo("data1"));

        str = await client.GetStringAsync("http://localhost:8080/dll/data/Test?{\"key\":\"data1\"}");
        Assert.That(str, Is.EqualTo("data1"));

        str = await client.GetStringAsync("http://localhost:8080/dll/data/Test1?{\"key\":\"data1\"}");
        Assert.That(str, Is.EqualTo("data1"));

        str = await client.GetStringAsync("http://localhost:8080/dll/data/Test2?{\"key\":\"data1\"}");
        Assert.That(str, Is.EqualTo("data1"));
    }

    [Test]
    public async Task ServerGetDataJsonArray()
    {
        var str = await client.GetStringAsync("http://localhost:8080/dll/array?[123,456,789]");
        Assert.That(str, Is.EqualTo("1368"));

        str = await client.GetStringAsync("http://localhost:8080/dll/array/Test?[123,456,789]");
        Assert.That(str, Is.EqualTo("1368"));

        str = await client.GetStringAsync("http://localhost:8080/dll/array/Test1?[123,456,789]");
        Assert.That(str, Is.EqualTo("1368"));

        str = await client.GetStringAsync("http://localhost:8080/dll/array/Test2?[123,456,789]");
        Assert.That(str, Is.EqualTo("1368"));
    }

    [Test]
    public async Task ServerGetWeb()
    {
        var str = await client.GetStringAsync("http://localhost:8080/web/test");
        Assert.That(str, Is.EqualTo("web1"));

        str = await client.GetStringAsync("http://localhost:8080/web/test/index.html");
        Assert.That(str, Is.EqualTo("web1"));

        str = await client.GetStringAsync("http://localhost:8080/web/test/data.html");
        Assert.That(str, Is.EqualTo("web2"));
    }
}