using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer.Core.FileSystem
{
    public class StaticDir
    {
        private ConcurrentDictionary<string, StaticDir> NextDir = new();
        private ConcurrentDictionary<string, HtmlFileObj> FileTemp = new();
        private FileSystemWatcher watch;
        private string dir;
        private bool IsRun = false;
        private Thread thread;
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
                NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size
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
        }

        public void OnError(object sender, ErrorEventArgs e)
        {
            ServerMain.LogError(e.GetException());
        }

        public void OnRnamed(object sender, RenamedEventArgs e)
        {
            ServerMain.LogOut(e.ToString());
        }

        public void OnDeleted(object sender, FileSystemEventArgs e)
        {
            ServerMain.LogOut(e.ToString());
        }

        public void OnCreated(object sender, FileSystemEventArgs e)
        {
            ServerMain.LogOut(e.ToString());
        }

        public void OnChanged(object sender, FileSystemEventArgs e)
        {
            ServerMain.LogOut(e.ToString());
        }

        public void Stop()
        {
            IsRun = false;
            watch.Dispose();
            FileTemp.Clear();
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
                    foreach (var item1 in FileTemp)
                    {
                        item1.Value.Tick();
                        if (item1.Value.Time <= 0)
                        {
                            FileTemp.TryRemove(item1.Key, out var item);
                        }
                    }
                    Thread.Sleep(1000);
                }
                catch (Exception e)
                {
                    ServerMain.LogError(e);
                }
            }
        }

        public byte[] GetFile(string[] rote, int index)
        {
            if (index == rote.Length)
            {
                string name = rote[^1];
                if (FileTemp.ContainsKey(name))
                {
                    var item = FileTemp[name];
                    item.Reset();
                    return item.Data;
                }
            }
            else if (NextDir.ContainsKey(rote[index]))
            {
                return NextDir[rote[index]].GetFile(rote, index + 1);
            }
            return null;
        }
    }
}