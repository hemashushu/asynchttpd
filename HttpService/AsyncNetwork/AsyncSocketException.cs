using System;
using System.Collections.Generic;
using System.Text;

namespace Doms.HttpService.AsyncNetwork
{
    /// <summary>
    /// Asynchronous socket (network layer) exception
    /// </summary>
    public class AsyncSocketException:Exception
    {
        public AsyncSocketException() : base() { }
        public AsyncSocketException(string message) : base(message) { }
    }
}
