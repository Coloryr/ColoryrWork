namespace ColoryrWork.Lib.Build;

public static class ConfigSave
{
    private static readonly object s_locker = new();
    public static T Config<T>(T obj1, string FilePath) where T : new()
    {
        FileInfo file = new(FilePath);
        T? obj;
        if (!file.Exists)
        {
            if (obj1 == null)
                obj = new T();
            else
                obj = obj1;
            Save(obj, FilePath);
        }
        else
        {
            lock (s_locker)
            {
                obj = JsonUtils.ToObj<T>(File.ReadAllText(FilePath));
            }
            obj ??= new();
        }
        return obj;
    }
    /// <summary>
    /// 保存配置文件
    /// </summary>
    public static void Save(object obj, string FilePath)
    {
        lock (s_locker)
        {
            File.WriteAllText(FilePath, JsonUtils.ToString(obj));
        }
    }
}
