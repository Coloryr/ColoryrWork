using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

void test() { }

Dictionary<string, string> cookies = new();

CookieContainer cookie1 = new();

var handler = new HttpClientHandler() { UseCookies = false };
var Client = new HttpClient(handler)
{
    Timeout = TimeSpan.FromSeconds(5)
};
Client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36 Edg/81.0.416.77");

var uri = new Uri("https://www.baidu.com/");

HttpRequestMessage requestMessage = new(HttpMethod.Get, uri);
HttpResponseMessage result = Client.SendAsync(requestMessage).Result;
var cookie = result.Headers.GetValues("Set-Cookie");
foreach (var item in cookie)
{
    cookie1.SetCookies(uri, item);
}

foreach (Cookie item in cookie1.GetCookies(uri))
{
    cookies.Add(item.Name, item.Value);
}

Console.WriteLine(JsonConvert.SerializeObject(cookies));