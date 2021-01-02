namespace ColoryrBuild
{
    public record ConfigObj
    {
        public string Name { get; set; }
        public string Token { get; set; }
        public string Http { get; set; }
        public bool SaveToken { get; set; }
    }
}
