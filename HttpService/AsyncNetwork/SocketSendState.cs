using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace Doms.HttpService.AsyncNetwork
{
    /// <summary>
    /// The state object for asynchronous socket
    /// </summary>
    public class SocketSendState
    {
        private byte[] _data; //the data to be send
        private int _length; //the length of the data need to be send, not the whole data length
        private int _offset; //The data offset in the next sending
        private int _startIndex; //the data offset in the start

        public SocketSendState(byte[] data, int startIndex, int length)
        {
            _data = data;
            _offset = startIndex;
            _startIndex = startIndex;
            _length = length;
        }

        public byte[] Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public int StartIndex
        {
            get { return _startIndex; }
            set { _startIndex = value; }
        }

         public int Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }
    }

}
