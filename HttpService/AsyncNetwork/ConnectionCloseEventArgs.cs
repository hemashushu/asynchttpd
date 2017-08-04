using System;
using System.Collections.Generic;
using System.Text;

namespace Doms.HttpService.AsyncNetwork
{
    /// <summary>
    /// Socket connection close event
    /// </summary>
    public class ConnectionCloseEventArgs:EventArgs
    {
        private Exception _lastException;

        public ConnectionCloseEventArgs(Exception ex)
        {
            _lastException = ex;
        }

        /// <summary>
        /// The last exception before connection close
        /// </summary>
        public Exception LastException
        {
            get { return _lastException; }
        }
    }
}
