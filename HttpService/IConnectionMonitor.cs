using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Doms.HttpService
{
    public interface IConnectionMonitor
    {
        void ConnectionStart(string connectionToken, IPAddress clientIp, string bindEndPointName, ref bool cancel);
        void ConnectionEnd(string connectionToken, IPAddress clientIp, string bindEndPointName);
    }
}
