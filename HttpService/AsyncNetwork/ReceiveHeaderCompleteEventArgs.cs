using System;
using System.Collections.Generic;
using System.Text;
using Doms.HttpService.HttpProtocol;

namespace Doms.HttpService.AsyncNetwork
{
    /// <summary>
    /// Header received event
    /// </summary>
    public class ReceiveHeaderCompleteEventArgs:EventArgs
    {
        private IProtocolHeader _lastReceiveHeader; //last received header

        //When the OnReceiveHeaderComplete event raise, caller must specify the 
        //_totalPlanReceivingLength value. If set 0 to this variable, it will not recive body  in the following time,
        //otherwise, it will do receiving body until the _totalHasReceivedLength reach the _totalPlanReceivingLength.
        private long _totalPlanReceivingLength; 

        public ReceiveHeaderCompleteEventArgs(
            IProtocolHeader lastReceiveHeader)
        {
            _lastReceiveHeader = lastReceiveHeader;
            _totalPlanReceivingLength = 0;
        }

        public IProtocolHeader LastReceiveHeader
        {
            get { return _lastReceiveHeader; }
        }

        public long TotalPlanReceivingLength
        {
            //NOTE:: 
            //If set 0 to this property, it will not recive body in the following time,
            //otherwise, it will do receiving body until the _totalHasReceivedLength
            //reach this property's value
            internal get { return _totalPlanReceivingLength; }
            set { _totalPlanReceivingLength = value; }
        }


    }
}
