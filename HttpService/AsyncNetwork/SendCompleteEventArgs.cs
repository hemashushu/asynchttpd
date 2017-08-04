using System;
using System.Collections.Generic;
using System.Text;
using Doms.HttpService.HttpProtocol;

namespace Doms.HttpService.AsyncNetwork
{
    /// <summary>
    /// Header or body sent event
    /// </summary>
    public class SendCompleteEventArgs : EventArgs
    {
        private IProtocolHeader _lastSendHeader; //last sended header
        private int _sendedLength; //the actually send length this time

        //When sending a big data, application should split the big data into serveral
        //small blocks, and send these blocks in serveral times,
        //_totalSendedLength and _totalPlanSendingLength records the progress
        private long _totalSendedLength;
        private long _totalPlanSendingLength;

        public SendCompleteEventArgs(
            IProtocolHeader lastSendHeader,
            int sendedLength,
            long totalSendedLength,
            long totalPlanSendingLength)
        {
            _lastSendHeader = lastSendHeader;
            _sendedLength = sendedLength;
            _totalSendedLength = totalSendedLength;
            _totalPlanSendingLength = totalPlanSendingLength;
        }

        public IProtocolHeader LastSendHeader
        {
            get { return _lastSendHeader; }
        }

        public int SendedLength
        {
            get { return _sendedLength; }
        }

        public long TotalSendedLength
        {
            get { return _totalSendedLength; }
        }

        public long TotalPlanSendingLength
        {
            get { return _totalPlanSendingLength; }
        }
    }

}
