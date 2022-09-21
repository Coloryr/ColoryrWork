using ColoryrServer.Core.Http;
using ColoryrServer.SDK;
using ColoryrWork.Lib.Debug.Object;
using ColoryrWork.Lib.ServerDebug;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System.Threading.Tasks;
using System;

namespace ColoryrServer.Core.ServerDebug;

internal static class PackRead
{
    public static void Pack(IChannelHandlerContext context, IByteBuffer buffer)
    {
        byte type = buffer.ReadByte();
        switch (type)
        {
            case 1:
                {
                    var obj1 = buffer.ReadRegisterPack();
                    var res = HttpInvokeRoute.CheckBase(obj1.url);
                    if (!res)
                    {
                        HttpInvokeRoute.AddDebug(obj1.url, context);
                    }
                    context.SendRegister(!res);
                }
                break;
            case 2:
                {
                    var obj = buffer.ReadHttpResPack();
                    DebugRoute.Res(obj.id, new()
                    {
                        ReCode = obj.resopneObj.ReCode,
                        Head = obj.resopneObj.Head,
                        ContentType = obj.resopneObj.ContentType,
                        Cookie = obj.resopneObj.Cookie,
                        Res = ResType.Byte,
                        Data = obj.resopneObj.Data
                    });
                }
                break;
            case 3:
                {
                    var obj = buffer.ReadDatabasePack();
                    IDatabase database;
                    switch (obj.type)
                    {
                        case 0:
                            database = new Mysql(obj.database, obj.id);
                            break;
                        case 1:
                            database = new MSsql(obj.database, obj.id);
                            break;
                        case 2:
                            database = new SqliteSql(obj.database, obj.id);
                            break;
                        case 3:
                            database = new OracleSql(obj.database, obj.id);
                            break;
                        default:
                            context.SendDatabaseRes(new()
                            {
                                ok = false,
                                qid = obj.qid,
                                message = "错误的类型"
                            });
                            return;
                    }
                    Task.Run(() =>
                    {
                        DatabaseResObj res;
                        try
                        {
                            switch (obj.read)
                            {
                                case 0:
                                    {
                                        var data = database.Query(obj.sql, obj.arg);
                                        res = new()
                                        {
                                            ok = true,
                                            res = data,
                                            qid = obj.qid
                                        };
                                    }
                                    break;
                                case 1:
                                    {
                                        var data = database.Execute(obj.sql, obj.arg);
                                        res = new()
                                        {
                                            ok = true,
                                            res1 = data,
                                            qid = obj.qid
                                        };
                                    }
                                    break;
                                default:
                                    res = new()
                                    {
                                        ok = false,
                                        qid = obj.qid,
                                        message = "错误的类型"
                                    };
                                    return;
                            }
                        }
                        catch (Exception e)
                        {
                            res = new()
                            {
                                ok = false,
                                qid = obj.qid,
                                message = e.ToString()
                            };
                        }
                        context.SendDatabaseRes(res);
                    });
                }
                break;
        }
    }
}
