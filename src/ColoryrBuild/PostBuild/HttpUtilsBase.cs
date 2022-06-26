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

    private byte[] keyArray;
    private byte[] ivArray;
    public HttpUtilsBase()
    {
        var Handler = new HttpClientHandler();
        httpClient = new(Handler)
        {
            Timeout = TimeSpan.FromSeconds(10)
        };
        httpClient.DefaultRequestHeaders.Add(BuildKV.BuildK, BuildKV.BuildV);
        httpClient.DefaultRequestHeaders.Add(BuildKV.BuildK1, App.Config.AES.ToString());

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
        keyArray = Encoding.UTF8.GetBytes(key);
        ivArray = Encoding.UTF8.GetBytes(iv);
    }

    protected byte[] AES(string data)
    {
        if (App.Config.AES)
        {
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(data);
            using var rDel = Aes.Create();
            rDel.BlockSize = 128;
            rDel.KeySize = 256;
            rDel.FeedbackSize = 128;
            rDel.Padding = PaddingMode.PKCS7;
            rDel.Mode = CipherMode.CBC;
            rDel.Key = keyArray;
            rDel.IV = ivArray;

            using var cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return resultArray;
        }
        return Encoding.UTF8.GetBytes(data);
    }

    public async Task<string> DoPost(BuildOBJ pack)
    {
        try
        {
            HttpContent Content = new ByteArrayContent(AES(JsonConvert.SerializeObject(pack)));
            Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var temp = await httpClient.PostAsync(App.Config.Http, Content);
            return await temp.Content.ReadAsStringAsync();
        }
        catch
        {
            return null;
        }
    }
}