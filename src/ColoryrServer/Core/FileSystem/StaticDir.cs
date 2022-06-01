using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace ColoryrServer.Core.FileSystem;

public class StaticDir
{
    private ConcurrentDictionary<string, StaticDir> NextDir = new();
    private ConcurrentDictionary<string, HtmlFileObj> FileTempMap = new();
    private FileSystemWatcher watch;
    private string dir;
    private bool IsRun = false;
    private Thread thread;

    public byte[] HtmlIcon { get; private set; }
    public byte[] Html404 { get; private set; }
    public byte[] HtmlIndex { get; private set; }

    public StaticDir(string dir)
    {
        this.dir = dir;
        IsRun = true;
        thread = new(TickTask)
        {
            Name = $"watch {dir}"
        };
        watch = new()
        {
            Path = dir,
            NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.Size
        };
        watch.BeginInit();
        watch.Changed += OnChanged;
        watch.Created += OnCreated;
        watch.Deleted += OnDeleted;
        watch.Renamed += OnRnamed;
        watch.Error += OnError;
        watch.EndInit();
        watch.EnableRaisingEvents = true;
        thread.Start();
        var info = new DirectoryInfo(dir);
        foreach (var item in info.GetDirectories())
        {
            NextDir.TryAdd(item.Name, new StaticDir(item.FullName + "/"));
        }
        var list = new DirectoryInfo(dir).GetFiles();
        foreach (var item in list)
        {
            if (item.Name.ToLower() is "404.html")
                Html404 = File.ReadAllBytes(item.FullName);
            else if (item.Name.ToLower() is "favicon.ico")
                HtmlIcon = File.ReadAllBytes(item.FullName);
            else if (item.Name.ToLower() is "index.html")
                HtmlIndex = File.ReadAllBytes(item.FullName);
        }
    }

    public void DirRename(string now)
    {
        watch.EnableRaisingEvents = false;
        watch.Path = now;
        watch.EnableRaisingEvents = true;
    }

    public void OnError(object sender, ErrorEventArgs e)
    {
        Console.WriteLine(e.GetException());
    }

    public void OnRnamed(object sender, RenamedEventArgs e)
    {
        if (e.Name.Contains('.'))
        {
            if (FileTempMap.TryRemove(e.OldName, out var v))
            {
                FileTempMap.TryAdd(e.Name, v);
                Console.WriteLine($"rename file:{e.FullPath}");
            }
        }
        else if (NextDir.ContainsKey(e.OldName))
        {
            if (NextDir.TryRemove(e.OldName, out var v1))
            {
                v1.DirRename(e.FullPath);
                NextDir.TryAdd(e.Name, v1);
                Console.WriteLine($"rename dir:{e.FullPath}");
            }
        }
    }

    public void OnDeleted(object sender, FileSystemEventArgs e)
    {
        if (e.Name.Contains('.'))
        {
            FileTempMap.TryRemove(e.Name, out var v);
        }
        else if (NextDir.ContainsKey(e.Name))
        {
            if (NextDir.TryRemove(e.Name, out var v1))
            {
                v1.Stop();
                Console.WriteLine($"delete dir:{e.FullPath}");
            }
        }
    }

    public void OnCreated(object sender, FileSystemEventArgs e)
    {
        if (Directory.Exists(e.FullPath))
        {
            var dir = new StaticDir(e.FullPath);
            NextDir.TryAdd(e.Name, dir);
            Console.WriteLine($"create dir:{e.FullPath}");
        }
    }

    public void OnChanged(object sender, FileSystemEventArgs e)
    {
        if (File.Exists(e.FullPath))
        {
            if (e.Name.ToLower() is "404.html")
                Html404 = FileTemp.LoadBytes(e.FullPath, false);
            else if (e.Name.ToLower() is "favicon.ico")
                HtmlIcon = FileTemp.LoadBytes(e.FullPath, false);
            else if (e.Name.ToLower() is "index.html")
                HtmlIndex = FileTemp.LoadBytes(e.FullPath, false);
            else if (FileTempMap.TryGetValue(e.Name, out var v))
            {
                v.Data = FileTemp.LoadBytes(e.FullPath, false);
                Console.WriteLine($"reload file:{e.FullPath}");
            }
        }
    }

    public void Stop()
    {
        IsRun = false;
        watch.Dispose();
        FileTempMap.Clear();
        foreach (var item in NextDir)
        {
            item.Value.Stop();
        }
        NextDir.Clear();
    }

    private void TickTask()
    {
        while (IsRun)
        {
            try
            {
                foreach (var item1 in FileTempMap)
                {
                    item1.Value.Tick();
                    if (item1.Value.Time <= 0)
                    {
                        FileTempMap.TryRemove(item1.Key, out var item);
                    }
                }
                Thread.Sleep(1000);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public byte[] GetFileIndex()
    {
        return HtmlIndex;
    }

    public byte[] GetFile(string[] rote, int index)
    {
        if (index == rote.Length - 1)
        {
            string name = rote[^1];
            if (!name.Contains('.'))
            {
                if (NextDir.ContainsKey(rote[index]))
                    return NextDir[rote[index]].GetFileIndex();
                else
                    return null;
            }
            if (name.ToLower() is "404.html")
                return Html404;
            else if (name.ToLower() is "favicon.ico")
                return HtmlIcon;
            else if (name.ToLower() is "index.html")
                return HtmlIndex;

            if (FileTempMap.ContainsKey(name))
            {
                var item = FileTempMap[name];
                item.Reset();
                return item.Data;
            }
            else if (File.Exists(dir + name))
            {
                var file = new HtmlFileObj()
                {
                    Data = FileTemp.LoadBytes(dir + name, false)
                };
                file.Reset();
                FileTempMap.TryAdd(name, file);
                return file.Data;
            }
        }
        else if (NextDir.ContainsKey(rote[index]))
        {
            return NextDir[rote[index]].GetFile(rote, index + 1);
        }
        return null;
    }
}
