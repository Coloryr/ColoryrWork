using Newtonsoft.Json;
using System.Text;

namespace ColoryrWork.Lib.Build;

public static class JsonUtils
{
    /// <summary>
    /// 反序列化，将json字符串转换成json类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result"></param>
    /// <returns></returns>
    public static T? ToObj<T>(string result)
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(result);
        }
        catch
        {
            return default;
        }
    }

    public static T? ToObj<T>(byte[] result)
    {
        try
        {
            var str = Encoding.UTF8.GetString(result);
            return JsonConvert.DeserializeObject<T>(str);
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// 序列化，将json类转换成json字符串
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="jsonClass"></param>
    /// <returns></returns>
    public static string ToString<T>(T jsonClass)
    {
        return JsonConvert.SerializeObject(jsonClass);
    }
}
