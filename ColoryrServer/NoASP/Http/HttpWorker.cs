using ColoryrServer.Http;
using ColoryrServer.SDK;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace ColoryrServer.NoASP
{
    class HttpWorker
    {
        private Thread thread;
        private ConcurrentStack<HttpListenerContext> list = new();
        public HttpWorker(string name)
        {
            thread = new Thread(Worker)
            {
                Name = name
            };
        }
        public void Start()
        {
            thread.Start();
        }
        public void Add(HttpListenerContext context)
        {
            list.Push(context);
        }
        private void Worker()
        {
            while (!HttpServer.IsActive)
            {
                Thread.Sleep(200);
            }
            while (HttpServer.IsActive)
            {
                if (list.TryPop(out HttpListenerContext? Context))
                {
                    bool isstream = false;
                    try
                    {
                        HttpListenerRequest Request = Context.Request;
                        HttpListenerResponse Response = Context.Response;
                        HttpReturn httpReturn;
                        switch (Request.HttpMethod)
                        {
                            case "POST":
                                Stream stream = Context.Request.InputStream;
                                MyContentType type;
                                if (Context.Request.ContentType == null)
                                {
                                    type = MyContentType.Other;
                                }
                                else if (Context.Request.ContentType.StartsWith(ServerContentType.POSTXFORM))
                                {
                                    type = MyContentType.XFormData;
                                }
                                else if (Context.Request.ContentType.StartsWith(ServerContentType.POSTFORMDATA))
                                {
                                    type = MyContentType.MFormData;
                                }
                                else if (Context.Request.ContentType.StartsWith(ServerContentType.JSON))
                                {
                                    type = MyContentType.Json;
                                }
                                else
                                {
                                    type = MyContentType.Other;
                                }
                                httpReturn = HttpPost.HttpPOST(stream, Request.ContentLength64, Request.RawUrl, Request.Headers, type);
                                if (!Response.OutputStream.CanWrite)
                                {
                                    Response.Abort();
                                    break;
                                }
                                Response.ContentType = httpReturn.ContentType;
                                Response.ContentEncoding = httpReturn.Encoding;
                                Response.StatusCode = httpReturn.ReCode;
                                if (httpReturn.Head != null)
                                    foreach (var Item in httpReturn.Head)
                                    {
                                        Response.AddHeader(Item.Key, Item.Value);
                                    }
                                if (httpReturn.Cookie != null)
                                    Response.AppendCookie(new Cookie("cs", httpReturn.Cookie));
                                if (httpReturn.Data1 == null)
                                    Response.OutputStream.Write(httpReturn.Data);
                                else
                                {
                                    Response.ContentLength64 = httpReturn.Data1.Length;
                                    httpReturn.Data1.Seek(httpReturn.Pos, SeekOrigin.Begin);
                                    Response.StatusCode = 206;
                                    isstream = true;
                                    httpReturn.Data1.CopyTo(Response.OutputStream);
                                }
                                Response.OutputStream.Flush();
                                Response.Close();
                                break;
                            case "GET":
                                httpReturn = HttpGet.HttpGET(Request.RawUrl, Request.Headers, Request.QueryString);
                                if (!Response.OutputStream.CanWrite)
                                {
                                    Response.Abort();
                                    break;
                                }
                                Response.ContentType = httpReturn.ContentType;
                                Response.ContentEncoding = httpReturn.Encoding;
                                Response.StatusCode = httpReturn.ReCode;
                                if (httpReturn.Head != null)
                                    foreach (var Item in httpReturn.Head)
                                    {
                                        Response.AddHeader(Item.Key, Item.Value);
                                    }
                                if (httpReturn.Cookie != null)
                                    Response.AppendCookie(new Cookie("cs", httpReturn.Cookie));
                                if (httpReturn.Data1 == null)
                                    Response.OutputStream.Write(httpReturn.Data);
                                else
                                {
                                    Response.ContentLength64 = httpReturn.Data1.Length;
                                    httpReturn.Data1.Seek(httpReturn.Pos, SeekOrigin.Begin);
                                    Response.StatusCode = 206;
                                    isstream = true;
                                    httpReturn.Data1.CopyTo(Response.OutputStream);
                                }
                                Response.OutputStream.Flush();
                                Response.Close();
                                break;
                        }
                    }
                    catch (HttpListenerException e)
                    {
                        if (e.ErrorCode == 64)
                            continue;
                    }
                    catch (Exception e)
                    {
                        if (isstream)
                            continue;
                        ServerMain.LogError(e);
                    }
                }
                Thread.Sleep(1);
            }
        }
    }
}
