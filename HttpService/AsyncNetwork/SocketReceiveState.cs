using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace Doms.HttpService.AsyncNetwork
{
    /// <summary>
    /// The state object for asynchronous socket
    /// </summary>
    class SocketReceiveState
    {
        private byte[] _data; //the buffer of receiving data
        private int _length; //the length of the buffer need to be store
        private int _offset; //The data offset in the next receiving
        private int _startIndex; //the data offset in the start

        public SocketReceiveState(byte[] data, int startIndex, int length)
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
