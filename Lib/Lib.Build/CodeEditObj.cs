namespace Lib.Build
{
    public enum EditFun
    {
        Add, Remove, Edit
    }
    record CodeEditObj
    {
        public EditFun Fun { get; set; }
        public int Line { get; set; }
        public string Code { get; set; }
    }
}
