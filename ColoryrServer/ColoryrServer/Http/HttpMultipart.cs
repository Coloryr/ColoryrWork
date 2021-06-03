using ColoryrServer.SDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ColoryrServer.Http
{
    internal class HttpMultipartRes 
    {
        /// <summary>
        /// 参数集合
        /// </summary>
        public Dictionary<string, string> Parameters = new();
        /// <summary>
        /// 上传的文件内容
        /// </summary>
        public Dictionary<string, HttpMultipartFile> FileContents = new();
    }

    internal class HttpMultipart
    {
        private static readonly Regex NameMatch = new(@"(?<=name\=\"")(.*?)(?=\"")");
        private static readonly Regex ContentTypeMatch = new(@"(?<=Content\-Type:)(.*?)(?=\r\n\r\n)");
        private static readonly Regex FilenameMatch = new(@"(?<=filename\=\"")(.*?)(?=\"")");
        public static HttpMultipartRes Parse(Stream stream, long Length)
        {
            HttpMultipartRes res = new();
            var data = ToByteArray(stream, Length);
            var content = Encoding.UTF8.GetString(data);
            var delimiterEndIndex = content.IndexOf("\r\n", StringComparison.Ordinal);
            if (delimiterEndIndex > -1)
            {
                var delimiter = content.Substring(0, content.IndexOf("\r\n", StringComparison.Ordinal)).Trim();
                var sections = content.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var s in sections)
                {
                    if (s.Contains("Content-Disposition"))
                    {
                        var nameMatch = NameMatch.Match(s);
                        var name = nameMatch.Value.Trim().ToLower();

                        if (string.IsNullOrWhiteSpace(name))
                        {
                            continue;
                        }

                        var contentTypeMatch = ContentTypeMatch.Match(s);
                        var filenameMatch = FilenameMatch.Match(s);

                        if (contentTypeMatch.Success && filenameMatch.Success)
                        {
                            var ContentType = contentTypeMatch.Value.Trim();
                            if (ContentType is ServerContentType.OSTREAM)
                            {
                                var Filename = filenameMatch.Value.Trim();

                                var startIndex = contentTypeMatch.Index + contentTypeMatch.Length + "\r\n\r\n".Length;
                                byte[] temp = Encoding.UTF8.GetBytes(s);
                                var contentLength = temp.Length - startIndex - 2;

                                var fileData = new byte[contentLength];

                                Buffer.BlockCopy(temp, startIndex, fileData, 0, contentLength);

                                res.FileContents.Add(name, new()
                                {
                                    Data = fileData,
                                    FileName = Filename
                                });
                            }
                        }
                        else
                        {
                            var startIndex = nameMatch.Index + nameMatch.Length + "\r\n\r\n".Length;
                            res.Parameters.Add(name, s.Substring(startIndex).TrimEnd('\r', '\n').Trim());
                        }
                    }
                }
            }
            return res;
        }

        public static byte[] ToByteArray(Stream stream, long Length)
        {
            var buffer = new byte[Length];
            using var ms = new MemoryStream();
            while (true)
            {
                var read = stream.Read(buffer, 0, buffer.Length);
                if (read <= 0)
                {
                    return ms.ToArray();
                }
                ms.Write(buffer, 0, read);
            }
        }
    }
}
