namespace ColoryrServer.Core.FileSystem;

internal class HtmlFileObj
{
    public byte[] Data { get; set; }
    public int Time { get; private set; }
    public void Reset()
    {
        Time = ServerMain.Config.Requset.TempTime;
    }
    public void Tick()
    {
        Time--;
    }
}
