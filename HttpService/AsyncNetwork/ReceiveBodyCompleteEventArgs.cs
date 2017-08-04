using System;
using System.Collections.Generic;
using System.Text;
using Doms.HttpService.HttpProtocol;

namespace Doms.HttpService.AsyncNetwork
{
    /// <summary>
    /// Body received event
    /// </summary>
    public class ReceiveBodyCompleteEventArgs : EventArgs
    {
        private IProtocolHeader _lastReceiveHeader; //last received header
        private byte[] _data; //the received data
        private int _dataLength; //the data length that has received in this time
        private int _dataOffset; //the data body offset

        //When receiving a big data, application may receive many small data in many times,
        //_totalHasReceivedLength and _totalPlanReceivingLength can record the progress
        private long _totalHasReceivedLength;
        private long _totalPlanReceivingLength;

        public ReceiveBodyCompleteEventArgs(
            IProtocolHeader lastReceiveHeader,
            byte[] data,
            int dataLength,
            int dataOffset,
            long totalHasReceivedLength,
            long totalPlanReceivingLength)
        {
            _lastReceiveHeader = lastReceiveHeader;
            _data = data;
            _dataLength = dataLength;
            _dataOffset = dataOffset;
            _totalHasReceivedLength = totalHasReceivedLength;
            _totalPlanReceivingLength = totalPlanReceivingLength;
        }

        public IProtocolHeader LastReceiveHeader
        {
            get { return _lastReceiveHeader; }
        }

        public byte[] Data
        {
            get { return _data; }
        }

        public int DataLength
        {
            get { return _dataLength; }
        }

        public int DataOffset
        {
            get { return _dataOffset; }
        }

        public long TotalHasReceivedLength
        {
            get { return _totalHasReceivedLength; }
        }

        public long TotalPlanReceivingLength
        {
            get { return _totalPlanReceivingLength; }
        }
    }
}
