using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColoryrServer.Core.DllManager;

public static class DllUseSave
{
    public static Dictionary<DllBuildSave, List<DllBuildSave>> UseLoad = new();

    public static void AddSave(DllBuildSave key, DllBuildSave item)
    {
        List<DllBuildSave> list;
        if (UseLoad.ContainsKey(key))
        {
            list = UseLoad[key];
            var item1 = list.Find(a => a.Name == item.Name);
            if (item1 != null)
            {
                list.Remove(item1);
            }

        }
        else
        {
            list = new();
            UseLoad.Add(key, list);
        }
        list.Add(item);
    }

    public static void Update(DllBuildSave name)
    {
        var list = UseLoad.Keys.Where(a => a.Name == name.Name);
        if (list.Any())
        {
            var list1 = UseLoad[list.First()];
            foreach (var item in list1)
            {
                Reload(item);
            }
        }
    }

    public static void Reload(DllBuildSave name)
    {
        switch (name.SelfType)
        {
            case DllType.Dll:

                break;
        }
    }
}
