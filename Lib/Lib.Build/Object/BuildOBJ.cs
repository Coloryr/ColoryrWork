﻿namespace Lib.Build.Object
{
    public enum ReType
    {
        AddDll, AddClass, AddIoT, AddWebSocket, AddRobot,
        Login, GetApi,
        GetDll, GetClass, GetIoT, GetWebSocket, GetRobot, GetApp,
        CodeDll, CodeClass, CodeIoT, CodeWebSocket, CodeRobot, CodeApp,
        RemoveDll, RemoveClass, RemoveIoT, RemoveWebSocket, RemoveRobot,
        UpdataDll, UpdataClass, UpdataIoT, UpdataWebSocket, UpdataRobot,
        AppRemoveFile,
        AddApp, RemoveApp, AppAddCS, AppAddXaml, RemoveAppCS, RemoveAppXaml,
        AppCsUpdata, AppXamlUpdata, AppUpdata,
        Check
    }
    class BuildOBJ
    {
        public string Token { get; set; }
        public string User { get; set; }
        public ReType Mode { get; set; }
        public string UUID { get; set; }
        public string Code { get; set; }
        public string Text { get; set; }
        public string Temp { get; set; }
        public int Version { get; set; }
    }
}
