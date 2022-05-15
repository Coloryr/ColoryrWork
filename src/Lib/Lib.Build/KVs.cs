namespace ColoryrWork.Lib.Build
{
    public class BuildKV
    {
        public const string BuildK = "Build";
        public const string BuildV = "3.0.0";
        public const string BuildK1 = "AES";
    }
    public record UploadObj
    {
        public string UUID { get; set; }
        public string FileName { get; set; }
    }
}
