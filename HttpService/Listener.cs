using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Doms.HttpService
{
    /// <summary>
    /// TCP listener
    /// </summary>
    class Listener
    {
        private IPEndPoint _bindEndPoint;
        private string _bindEndPointName;
        private Socket _listenSocket;
        private bool _isClosing;

        public event EventHandler<AcceptNewConnectionEventArgs> NewConnectionAccepted;

        public Listener(IPEndPoint bindEndPoint, string bindEndPointName)
        {
            _bindEndPoint = bindEndPoint;
            _bindEndPointName = bindEndPointName;
        }

        public IPEndPoint BindEndPoint
        {
            get { return _bindEndPoint; }
        }

        public string BindingName
        {
            get { return _bindEndPointName; }
        }

        public void StartListen()
        {
            //create listen socket
            _listenSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

            //bind endpoint, this may occur exception
            _listenSocket.Bind(_bindEndPoint);
            _listenSocket.Listen(50);

            //start accept
            _listenSocket.BeginAccept(new AsyncCallback(acceptCallback), null);
        }

        public void StopListen()
        {
            _isClosing = true;
            _listenSocket.Close();
        }

        private void acceptCallback(IAsyncResult result)
        {
            Socket handler = null;

            try
            {
                handler = _listenSocket.EndAccept(result);
            }
            catch
            {
                //ignore this exception, it usually cause by remote closing
            }

            if (_isClosing) return;

            //continue listening
            _listenSocket.BeginAccept(new AsyncCallback(acceptCallback), null);

            //raise event
            if (handler != null)
            {
                AcceptNewConnectionEventArgs arg =  new AcceptNewConnectionEventArgs(
                    handler, _bindEndPoint, _bindEndPointName);

                NewConnectionAccepted(this, arg);
            }
        }

    }//end class
}
