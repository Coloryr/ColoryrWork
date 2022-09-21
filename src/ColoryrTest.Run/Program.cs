using ColoryrServer.SDK;

namespace ColoryrTest.Run;

internal class Program
{
    private delegate dynamic DllIN(HttpDllRequest arg);
    //private static bool Check(Type type)
    //{
    //    bool find = false;
    //    do
    //    {
    //        if (type == typeof(INetty))
    //        {
    //            find = true;
    //            break;
    //        }
    //        type = type.BaseType;
    //    } while (type != null);
    //    return find;
    //}
    static void Main(string[] args)
    {
        //var bossGroup = new MultithreadEventLoopGroup();
        //var workerGroup = new MultithreadEventLoopGroup();
        //var p1 = new NettyDemoCS();
        //var p2 = new NettyDemoCS();
        //Stopwatch watch = new();
        //watch.Start();
        //Type type = p1.GetType();
        //bool res = Check(type);
        //watch.Stop();
        //Console.WriteLine(res);
        //Console.WriteLine(watch.Elapsed);
        //p1.Start(bossGroup, workerGroup, 12345);
        //p2.Start(bossGroup, workerGroup, 24567);
        //Console.Read();
        //p1.Stop();
        //p2.Stop();
        //Task.WaitAll(bossGroup.ShutdownGracefullyAsync(), workerGroup.ShutdownGracefullyAsync());

        //Stopwatch stopwatch = new();
        //stopwatch.Start();
        //var class1 = new DllDemo();
        //for (int a = 0; a < 100000000; a++)
        //{
        //    class1.Main(null);
        //}
        //stopwatch.Stop();
        //Console.WriteLine(stopwatch.ElapsedMilliseconds);

        //var type = typeof(DllDemo);

        //stopwatch = new();
        //stopwatch.Start();
        //dynamic obj1 = Activator.CreateInstance(type);
        //for (int a = 0; a < 100000000; a++)
        //{
        //    obj1.Main(null);
        //}
        //stopwatch.Stop();
        //Console.WriteLine(stopwatch.ElapsedMilliseconds);

        //var method = type.GetMethod("Main");

        //stopwatch = new();
        //stopwatch.Start();
        //obj1 = Activator.CreateInstance(type);
        //for (int a = 0; a < 100000000; a++)
        //{
        //    //method.Invoke(obj1, new object[] { null });
        //}
        //stopwatch.Stop();
        //Console.WriteLine(stopwatch.ElapsedMilliseconds);

        //stopwatch = new();
        //stopwatch.Start();
        //obj1 = Activator.CreateInstance(type);
        //DllIN getDelegate = Delegate.CreateDelegate(typeof(DllIN), obj1, method) as DllIN;
        //for (int a = 0; a < 100000000; a++)
        //{
        //    getDelegate(null);
        //}
        //stopwatch.Stop();
        //Console.WriteLine(stopwatch.ElapsedMilliseconds);
    }
}