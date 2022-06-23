using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.Unicode;

namespace ColoryrWork.Lib.Build;

public static class JsonUtils
{
    private readonly static JsonSerializerOptions Options1 = new()
    {
        IncludeFields = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };

    private readonly static JsonSerializerOptions Options2 = new()
    {
        WriteIndented = true,
        IncludeFields = true,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };

    /// <summary>
    /// 反序列化，将json字符串转换成json类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result"></param>
    /// <returns></returns>
    public static T ToObj<T>(string result)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(result, Options1);
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
    public static string ToString(object jsonClass)
    {
        return JsonSerializer.Serialize(jsonClass, Options2);
    }
}
