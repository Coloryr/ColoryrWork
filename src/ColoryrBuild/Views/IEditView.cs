using ColoryrWork.Lib.Build.Object;

namespace ColoryrBuild.Views;

internal interface IEditView
{
    public CSFileObj obj { get; }
    public CodeType type { get; }
    public void Close();
    public void GetCode();
}
