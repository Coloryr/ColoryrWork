namespace Lib.Build
{
    class BuildKV
    {
        public const string BuildK = "Build";
        public const string BuildV = "2.2.0";
    }
    class UploadKV
    {
        public const string UploadK = "Upload";
        public const string UploadV = "2.2.0";
    }
    record UploadObj
    {
        public string UUID { get; set; }
        public string FileName { get; set; }
    }
}
