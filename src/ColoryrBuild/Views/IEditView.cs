using ColoryrWork.Lib.Build.Object;

namespace ColoryrBuild.Views;

internal interface IEditView
{
    public CSFileObj Obj { get; }
    public CodeType Type { get; }
    public void Close();
    public void GetCode();
}
