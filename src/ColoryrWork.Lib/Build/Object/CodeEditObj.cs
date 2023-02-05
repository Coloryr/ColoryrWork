namespace ColoryrWork.Lib.Build.Object;

public enum EditFun
{
    Add, Remove, Edit
}
public record CodeEditObj
{
    public EditFun Fun { get; set; }
    public int Line { get; set; }
    public string Code { get; set; }
}
