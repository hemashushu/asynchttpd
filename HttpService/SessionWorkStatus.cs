using System;
using System.Collections.Generic;
using System.Text;

namespace Doms.HttpService
{
    /// <summary>
    /// Session work status, use to remember what is the session doing now
    /// </summary>
    enum SessionWorkStatus
    {
        Idle, //session has nothing to do
        Busy, //processing the request

        SendingRequestHeader,
        SendingRequestBody,
        SendingResponseHeader,
        SendingResponseBody,

        ReceivingRequestHeader,
        ReceivingRequestBody,
        ReceivingResponseHeader,
        ReceivingResponseBody,
    }
}
