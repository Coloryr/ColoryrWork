using ColoryrServer.Pipe;
using ColoryrServer.SDK;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace ColoryrServer.Http
{
    class HttpWorker
    {
        public static void Worker()
        {
            while (!HttpServer.IsActive)
            {
                Thread.Sleep(200);
            }
            while (HttpServer.IsActive)
            {
                if (HttpServer.Queue.TryTake(out HttpListenerContext Context))
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
                                if (Context.Request.ContentType.StartsWith(ServerContentType.POSTXFORM))
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
        public static void PipeWorker()
        {
            while (!HttpServer.IsActive)
            {
                Thread.Sleep(200);
            }
            while (HttpServer.IsActive)
            {
                if (HttpServer.Queue.TryTake(out HttpListenerContext Context))
                {
                    try
                    {
                        HttpListenerRequest Request = Context.Request;
                        HttpListenerResponse Response = Context.Response;
                        switch (Request.HttpMethod)
                        {
                            case "POST":
                                StreamReader Reader = new(Context.Request.InputStream, Encoding.UTF8);
                                MyContentType type;
                                if (Context.Request.ContentType is ServerContentType.POSTXFORM)
                                {
                                    type = MyContentType.XFormData;
                                }
                                else if (Context.Request.ContentType is ServerContentType.POSTFORMDATA)
                                {
                                    type = MyContentType.MFormData;
                                }
                                else if (Context.Request.ContentType is ServerContentType.JSON)
                                {
                                    type = MyContentType.Json;
                                }
                                else
                                {
                                    Response.OutputStream.Write(Encoding.UTF8.GetBytes($"不支持{Context.Request.ContentType}"));
                                    Response.OutputStream.Flush();
                                    Response.StatusCode = 400;
                                    Response.Close();
                                    break;
                                }
                                dynamic httpReturn = HttpPost.PipeHttpPOST(Reader, Request.RawUrl, Request.Headers, type);
                                if (httpReturn is HttpReturn)
                                {
                                    Response.ContentType = httpReturn.ContentType;
                                    Response.ContentEncoding = httpReturn.Encoding;
                                    Response.OutputStream.Write(httpReturn.Data);
                                    Response.OutputStream.Flush();
                                    Response.StatusCode = httpReturn.ReCode;
                                    Response.Close();
                                    break;
                                }
                                else
                                {
                                    PipeClient.Http(httpReturn, Response);
                                }
                                break;
                            case "GET":
                                httpReturn = HttpGet.PipeHttpGET(Request.RawUrl, Request.Headers, Request.QueryString);
                                PipeClient.Http(httpReturn, Response);
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        ServerMain.LogError(e);
                    }
                }
                Thread.Sleep(TimeSpan.FromTicks(100));
            }
        }
    }
}
