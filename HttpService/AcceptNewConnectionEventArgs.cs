using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Doms.HttpService
{
    /// <summary>
    /// Event args for accepted a new connection
    /// </summary>
    class AcceptNewConnectionEventArgs : EventArgs
    {
        private Socket _workSocket;
        private IPEndPoint _bindEndPoint;
        private string _bindingEndPointName;

        public AcceptNewConnectionEventArgs(
            Socket workSocket, IPEndPoint bindEndPoint, string bindEndPointName)
        {
            _workSocket = workSocket;
            _bindEndPoint = bindEndPoint;
            _bindingEndPointName = bindEndPointName;
        }

        public Socket WorkSocket
        {
            get { return _workSocket; }
        }

        public IPEndPoint BindEndPoint
        {
            get { return _bindEndPoint; }
        }

        public string BindEndPointName
        {
            get { return _bindingEndPointName; }
        }

    }
}
