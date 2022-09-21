using ColoryrBuild.Windows;
using ColoryrWork.Lib.Build;
using ColoryrWork.Lib.Build.Object;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ColoryrBuild.PostBuild;

public abstract class HttpUtilsBase
{
    protected HttpClient httpClient;

    private byte[] KeyArray;
    private byte[] IvArray;
    public HttpUtilsBase()
    {
        var Handler = new HttpClientHandler();
        httpClient = new(Handler)
        {
            Timeout = TimeSpan.FromSeconds(10)
        };
        httpClient.DefaultRequestHeaders.Add(BuildKV.BuildK, BuildKV.BuildV);

        string key = App.Config.Key;
        string iv = App.Config.IV;
        if (key.Length != 32)
        {
            if (key.Length > 32)
            {
                key = key[..31];
            }
            else
            {
                key += new string(new char[32 - key.Length]);
            }
        }
        if (iv.Length != 16)
        {
            if (iv.Length > 16)
            {
                iv = iv[..15];
            }
            else
            {
                iv += new string(new char[16 - iv.Length]);
            }
        }
        KeyArray = Encoding.UTF8.GetBytes(key);
        IvArray = Encoding.UTF8.GetBytes(iv);
    }
    /// <summary>
    /// AES加密
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    protected byte[] AES(string data)
    {
        byte[] toEncryptArray = Encoding.UTF8.GetBytes(data);
        using var rDel = Aes.Create();
        rDel.BlockSize = 128;
        rDel.KeySize = 256;
        rDel.FeedbackSize = 128;
        rDel.Padding = PaddingMode.PKCS7;
        rDel.Mode = CipherMode.CBC;
        rDel.Key = KeyArray;
        rDel.IV = IvArray;

        using var cTransform = rDel.CreateEncryptor();
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

        return resultArray;
    }
    /// <summary>
    /// 发送POST请求
    /// </summary>
    /// <param name="pack">数据包</param>
    /// <returns>结果</returns>
    public async Task<string> DoPost(BuildOBJ pack)
    {
        try
        {
            HttpContent Content = new ByteArrayContent(AES(JsonConvert.SerializeObject(pack)));
            Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var temp = await httpClient.PostAsync(App.Config.Http, Content);
            return await temp.Content.ReadAsStringAsync();
        }
        catch (Exception e)
        {
            InfoWindow.Show("登录错误", "服务器无响应" + Environment.NewLine + e.ToString());
            return null;
        }
    }
}