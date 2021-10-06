namespace Lib.Build
{
    public class BuildKV
    {
        public const string BuildK = "Build";
        public const string BuildV = "2.2.1";
        public const string BuildK1 = "AES";
    }
    public class UploadKV
    {
        public const string UploadK = "Upload";
        public const string UploadV = "2.2.1";
    }
    public record UploadObj
    {
        public string UUID { get; set; }
        public string FileName { get; set; }
    }
}
