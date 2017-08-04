using System;
using System.Collections.Generic;
using System.Text;

namespace Doms.HttpService
{
    /// <summary>
    /// WebServerException
    /// </summary>
    public class HttpServiceException:Exception
    {
        public HttpServiceException() : base() { }
        public HttpServiceException(string message) : base(message) { }
    }
}
