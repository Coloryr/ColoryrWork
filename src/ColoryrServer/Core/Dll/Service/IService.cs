using ColoryrServer.SDK;
using System;

namespace ColoryrServer.Core.Dll.Service;

internal interface IService
{
    ServiceState State { get; }
    Exception LastError { get; }
    void OnStart();
    void SetArg(object[] arg);
    void OnStop();
    void Close();
    void Pause();
}
