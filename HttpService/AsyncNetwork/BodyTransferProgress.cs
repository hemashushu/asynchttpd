using System;
using System.Collections.Generic;
using System.Text;

namespace Doms.HttpService.AsyncNetwork
{
    /// <summary>
    /// Body transfer(sending/receiving) progress
    /// </summary>
    class BodyTransferProgress
    {
        private long _completed; //the data length of that has been send/received
        private long _total; //the total length need to be send/received

        public void Reset()
        {
            _completed = 0;
            _total = 0;
        }

        public long Completed
        {
            get { return _completed; }
            set { _completed = value; }
        }

        public long Total
        {
            get { return _total; }
            set { _total = value; }
        }

    }
}
