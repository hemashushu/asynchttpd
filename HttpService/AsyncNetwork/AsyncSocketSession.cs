using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Doms.HttpService.HttpProtocol;

namespace Doms.HttpService.AsyncNetwork
{
    /// <summary>
    /// Asynchronous socket session.
    /// provide base HTTP style data sending and receiving functions.
    /// </summary>
    public class AsyncSocketSession
    {
        #region private variables
        //the working socket
        private Socket _workingSocket;

        //the last send and recive header
        private IProtocolHeader _lastSendHeader;
        private IProtocolHeader _lastReceiveHeader;

        //the temporary send and receive state object
        private SocketSendState _toBeSendData;
        private SocketReceiveState _toBeReceiveData;

        //the data package size in each receiving or sending time
        private const int TRANSFER_BLOCK_LENGTH = 16 * 1024;

        //the header buffer length
        private const int HEADER_BUFFER_LENGTH = 4 * 1024;

        //progress of sending and receiving body 
        private BodyTransferProgress _sendingProgress;
        private BodyTransferProgress _receivingProgress;

        //the socket working status
        private bool _isSendingHeader;
        private bool _isReceivingHeader;
        private bool _isClosing;

        //buffer for receive header
        private byte[] _headerBuffer;
        #endregion

        #region events
        /// <summary>
        /// Socket connection has been close
        /// </summary>
        public event EventHandler<ConnectionCloseEventArgs> ConnectionClose;

        /// <summary>
        /// Send header or/and body complete
        /// </summary>
        public event EventHandler<SendCompleteEventArgs> SendComplete;

        /// <summary>
        /// Receive header complete
        /// </summary>
        public event EventHandler<ReceiveHeaderCompleteEventArgs> ReceiveHeaderComplete;

        /// <summary>
        /// Receive body complete
        /// </summary>
        public event EventHandler<ReceiveBodyCompleteEventArgs> ReceiveBodyComplete;
        #endregion

        #region init and properties
        /// <summary>
        /// Initialize varables
        /// </summary>
        public AsyncSocketSession(Socket workSocket)
        {
            _workingSocket = workSocket;

            //mark the socket status as receiving header in default
            _isReceivingHeader = true;

            //the buffer for receive header
            _headerBuffer = new byte[HEADER_BUFFER_LENGTH];

            //progress
            _sendingProgress = new BodyTransferProgress();
            _receivingProgress = new BodyTransferProgress();
        }

        /// <summary>
        /// working socket
        /// </summary>
        public Socket WorkingSocket
        {
            get { return _workingSocket; }
        }
        #endregion

        #region send data
        /// <summary>
        /// Send header only (without body)
        /// </summary>
        /// <param name="header"></param>
        public void Send(IProtocolHeader header)
        {
            if (_isClosing)
                throw new AsyncSocketException("Connection has closed");

            _lastSendHeader = header;
            _isSendingHeader = true;
            _toBeSendData = null;
            _sendingProgress.Reset();

            byte[] headerData = header.GetHeaderRawData();
            SocketSendState sendState = new SocketSendState(headerData, 0, headerData.Length);
            beginSendData(sendState);
        }

        /// <summary>
        /// Send header and body at one time
        /// </summary>
        /// <param name="header"></param>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public void Send(IProtocolHeader header, byte[] data, int offset, int length)
        {
            //NOTE::
            //when this operation complete, it raise SendComplete event only once when
            //the header and body all send complete.

            if (_isClosing)
                throw new AsyncSocketException("Connection has closed");

            _lastSendHeader = header;
            _isSendingHeader = true;
            _toBeSendData = new SocketSendState(data, offset, length);
            _sendingProgress.Reset();
            _sendingProgress.Total = length;

            byte[] headerData = header.GetHeaderRawData();
            SocketSendState sendState = new SocketSendState(headerData, 0, headerData.Length);
            beginSendData(sendState);
        }

        /// <summary>
        /// Combine with 'ContinueSendingBody' method to send big data
        /// </summary>
        /// <param name="header"></param>
        /// <param name="totalPlanSendingLength"></param>
        public void Send(IProtocolHeader header, long totalPlanSendingLength)
        {
            //NOTE::
            //when need sending big data, call this method first,
            //and when this header send complete, call "ContinueSendingBody" many times 
            //to send the body until all has been send.

            if (_isClosing)
                throw new AsyncSocketException("Connection has closed");

            _lastSendHeader = header;
            _isSendingHeader = true;
            _toBeSendData = null;
            _sendingProgress.Reset();
            _sendingProgress.Total = totalPlanSendingLength;

            byte[] headerData = header.GetHeaderRawData();
            SocketSendState sendState = new SocketSendState(headerData, 0, headerData.Length);
            beginSendData(sendState);
        }

        /// <summary>
        /// Combine with 'Send(IProtocolHeader, long)' method to send big data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public void ContinueSendingBody(byte[] data, int offset, int length)
        {
            //NOTE::
            //the 'SendComplete' event will raise when the body has been send each time.

            if (_isClosing)
                throw new AsyncSocketException("Connection has closed");

            _isSendingHeader = false;
            _toBeSendData = null;
            SocketSendState sendState = new SocketSendState(data, offset, length);

            beginSendData(sendState);
        }

        /// <summary>
        /// begin send data
        /// </summary>
        /// <param name="sendState"></param>
        private void beginSendData(SocketSendState sendState)
        {
            try
            {
                //calculate the length need to be send at this time
                int sendByte = sendState.Length - (sendState.Offset - sendState.StartIndex);
                if (sendByte > TRANSFER_BLOCK_LENGTH)
                {
                    sendByte = TRANSFER_BLOCK_LENGTH;
                }

                _workingSocket.BeginSend(
                    sendState.Data,
                    sendState.Offset,
                    sendByte,
                    SocketFlags.None,
                    new AsyncCallback(sendDataCallback),
                    sendState);
            }
            catch (Exception ex)
            {
                onClose(ex);
            }
        }

        /// <summary>
        /// send data callback
        /// </summary>
        /// <param name="result"></param>
        private void sendDataCallback(IAsyncResult result)
        {
            SocketSendState sendState = (SocketSendState)result.AsyncState;

            try
            {
                int sendByte = _workingSocket.EndSend(result);
                sendState.Offset += sendByte;

                if (!_isSendingHeader)
                {
                    _sendingProgress.Completed += sendByte;
                }
            }
            catch (Exception ex)
            {
                onClose(ex);
                return;
            }

            if (sendState.Offset - sendState.StartIndex < sendState.Length)
            {
                //send the remain data
                beginSendData(sendState);
            }
            else
            {
                //when send header and conent at one time, this object will not equal NULL
                if (_toBeSendData != null)
                {
                    //here means the header has been send complete, 
                    //now should prepare to send body
                    _isSendingHeader = false;
                    sendState = _toBeSendData;
                    _toBeSendData = null;
                    beginSendData(sendState);
                }
                else
                {
                    //raise the SendComplete event
                    SendCompleteEventArgs arg = new SendCompleteEventArgs(
                        _lastSendHeader,
                        sendState.Length,
                        _sendingProgress.Completed,
                        _sendingProgress.Total);

                    try
                    {
                        SendComplete(this, arg);
                    }
                    catch (Exception ex)
                    {
                        onClose(ex);
                        return;
                    }
                }
            }//end if

        }
        #endregion

        #region receive data

        /// <summary>
        /// Begin receive data
        /// </summary>
        /// <param name="header"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public void Receive(IProtocolHeader header, byte[] buffer, int offset, int length)
        {
            //the buffer length must large than TRANSFER_BLOCK_LENGTH
            if (buffer.Length - offset < TRANSFER_BLOCK_LENGTH ||
                length < TRANSFER_BLOCK_LENGTH)
            {
                throw new ArgumentException("Buffer length must large than TRANSFER_BLOCK_LENGTH");
            }

            if (_isClosing)
                throw new AsyncSocketException("Connection has closed");

            _lastReceiveHeader = header;
            _toBeReceiveData = new SocketReceiveState(buffer, offset, length);
            _receivingProgress.Reset();

            SocketReceiveState receiveState = new SocketReceiveState(
                _headerBuffer, 0, _headerBuffer.Length);

            beginReceiveData(receiveState);
        }

        /// <summary>
        /// Continue receiving body
        /// </summary>
        public void ContinueReceivingBody(byte[] buffer, int offset, int length)
        {
            //NOTE::
            //need to call this method many times until all body receive complete

            _toBeReceiveData = new SocketReceiveState(buffer, offset, length);
            beginReceiveData(_toBeReceiveData);
        }

        /// <summary>
        /// begin receive data
        /// </summary>
        private void beginReceiveData(SocketReceiveState receiveState)
        {
            try
            {
                //calculate the length need to be receive at this time
                int receiveByte = receiveState.Length - (receiveState.Offset - receiveState.StartIndex);
                if (receiveByte > TRANSFER_BLOCK_LENGTH)
                {
                    receiveByte = TRANSFER_BLOCK_LENGTH;
                }

                _workingSocket.BeginReceive(
                    receiveState.Data,
                    receiveState.Offset,
                    receiveByte,
                    SocketFlags.None,
                    new AsyncCallback(receiveDataCallback),
                    receiveState);
            }
            catch (Exception ex)
            {
                onClose(ex);
            }
        }

        /// <summary>
        /// receive data callback
        /// </summary>
        /// <param name="result"></param>
        private void receiveDataCallback(IAsyncResult result)
        {
            SocketReceiveState receiveState = (SocketReceiveState)result.AsyncState;
            int recByte = 0;

            try
            {
                recByte = _workingSocket.EndReceive(result);
                receiveState.Offset += recByte;

                if (!_isReceivingHeader)
                {
                    _receivingProgress.Completed += recByte;
                }

            }
            catch (Exception ex)
            {
                onClose(ex);
                return;
            }

            if (recByte > 0)
            {
                try
                {
                    analyzeData(receiveState);
                }
                catch (Exception ex)
                {
                    onClose(ex);
                    return;
                }
            }
            else
            {
                onClose(new AsyncSocketException("Connection has been closed"));
            }

        }
        #endregion

        #region analyze receive data
        /// <summary>
        /// analyze the received data
        /// </summary>
        /// <param name="receiveState"></param>
        private void analyzeData(SocketReceiveState receiveState)
        {
            //a flag to indicate whether the header/body has received complete
            bool receiveCompleted = true;

            if (_isReceivingHeader)
            {
                //seek the split-symbol between the header and body
                int position = seekSplitSymbolPosition(receiveState.Data, receiveState.Offset);

                if (position < 0)
                {
                    //continue receive header
                    receiveCompleted = false;
                    if (receiveState.Offset >= HEADER_BUFFER_LENGTH)
                    {
                        onClose(new AsyncSocketException("Header data overflow"));
                        return;
                    }
                }
                else
                {
                    //parse the header string
                    //byte[] headerData = new byte[position];
                    //Buffer.BlockCopy(receiveState.Data, 0, headerData, 0, headerData.Length);
                    //_lastReceiveHeader.HeaderRawData = headerData;

                    _lastReceiveHeader.SetHeaderRawData(receiveState.Data, 0, position);

                    //raise ReceiveHeaderComplete event
                    ReceiveHeaderCompleteEventArgs arg = new ReceiveHeaderCompleteEventArgs(_lastReceiveHeader);

                    try
                    {
                        ReceiveHeaderComplete(this, arg);
                        //NOTE::
                        //here, caller must specify the "arg.TotalPlanReceivingLength" value,
                        //to indicate whether continue receive body or not
                    }
                    catch (Exception ex)
                    {
                        onClose(ex);
                        return;
                    }

                    if (arg.TotalPlanReceivingLength > 0)
                    {
                        //need receive body
                        _isReceivingHeader = false;
                        _receivingProgress.Total = arg.TotalPlanReceivingLength;

                        //check this data whether it contains partial body
                        int tailLength = receiveState.Offset - position - 4;
                        if (tailLength > 0)
                        {
                            Buffer.BlockCopy(receiveState.Data, position + 4,
                                _toBeReceiveData.Data, _toBeReceiveData.Offset, tailLength);

                            _toBeReceiveData.Offset += tailLength;
                            _receivingProgress.Completed = tailLength;
                        }

                        //replace the receive state object
                        receiveState = _toBeReceiveData;
                    }
                }
            }

            //to judge if the body has receive complete
            if (!_isReceivingHeader)
            {
                if (receiveState.Offset - receiveState.StartIndex < receiveState.Length &&
                    _receivingProgress.Completed < _receivingProgress.Total)
                {
                    //not complete yet
                    receiveCompleted = false;
                }
                else
                {
                    //receive body complete

                    if (_receivingProgress.Completed >= _receivingProgress.Total)
                    {
                        //all body receive complete, reset this flag
                        _isReceivingHeader = true;
                    }

                    //raise the ReceiveBodyComplete event
                    ReceiveBodyCompleteEventArgs arg = new ReceiveBodyCompleteEventArgs(
                        _lastReceiveHeader,
                        receiveState.Data,
                        receiveState.Offset,
                        receiveState.StartIndex,
                        _receivingProgress.Completed,
                        _receivingProgress.Total);

                    try
                    {
                        ReceiveBodyComplete(this, arg);
                    }
                    catch (Exception ex)
                    {
                        onClose(ex);
                        return;
                    }
                }
            }

            //continue to receiving header or body data
            if (!receiveCompleted)
            {
                beginReceiveData(receiveState);
            }

        }
        #endregion

        #region private functions
        /// <summary>
        /// seek the split-symbol position
        /// </summary>
        /// <param name="source"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private int seekSplitSymbolPosition(byte[] source, int length)
        {
            //the split-symbol is double "\r\n"
            byte[] splitSymbol = new byte[] { 0xd, 0xa, 0xd, 0xa };

            for (int idx = 0; idx < length; idx++)
            {
                int offset = 0;
                for (; offset < 4 && idx + offset < length; offset++)
                {
                    if (source[idx + offset] != splitSymbol[offset]) break;
                }
                if (offset == 4)
                {
                    return idx;
                }
            }
            return -1;
        }
        #endregion

        #region Close
        /// <summary>
        /// Close session initiativitly
        /// </summary>
        public void Close()
        {
            onClose(null);
        }

        /// <summary>
        /// close socket and raise "ConnectionClose" event
        /// </summary>
        private void onClose(Exception ex)
        {
            if (_isClosing) return;
            _isClosing = true;

            try
            {
                _workingSocket.Shutdown(SocketShutdown.Both);
                _workingSocket.Close();
            }
            catch
            {
                //ignore socket close error
            }

            //raise event
            if (ConnectionClose != null)
            {
                ConnectionCloseEventArgs arg = new ConnectionCloseEventArgs(ex);
                ConnectionClose(this, arg);
            }
        }
        #endregion

    }
}

//Copyright (c) 2007-2009, Kwanhong Young, All rights reserved.
//mapleaves@gmail.com
//http://www.domstorage.com