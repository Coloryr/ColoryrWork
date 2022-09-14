using ColoryrServer.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColoryrServer.Core.DllManager.Service;

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
