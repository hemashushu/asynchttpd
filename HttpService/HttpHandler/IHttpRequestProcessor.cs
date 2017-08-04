using System;
using System.Collections.Generic;
using System.Text;
using Doms.HttpService.HttpProtocol;

namespace Doms.HttpService.HttpHandler
{
    /// <summary>
    /// Request processor interface
    /// </summary>
    public interface IHttpRequestProcessor
    {
        //NOTE::
        //the method functions:
        //1. When a new request come, call 'ProcessRequest' first, and append request body 
        // to handler by 'RequestBodyArrival' method.
        // when all request body has been append, call 'FinishAppend'.
        //
        //2. Not all handler can handle request body, so need check 'ProcessContext.RequestBodyAcceptable'
        // property before put body to handler.
        //
        //3. When the request has been handled, check the 'ResponseBodyLength' property,
        // if it equals '0', indicate that there is no response body, when it large than '0', it means
        // there are response body, and manager should call 'SubmitResponseBody' to
        // get all response body until the method return '0' and send the data to client.
        // 
        //4. Specially, 'ResponseBodyLength' property may equals '-1', that value means
        // the response body length is unknown currently, and use "chunked" transfer encoding mode,
        // call 'GetNextResponseData' until it return '0'.

        /// <summary>
        /// Handle request
        /// </summary>
        /// <param name="context"></param>
        void ProcessRequest(HandlerContext context);

        /// <summary>
        /// Indicate whether the current processor can accept request body
        /// </summary>
        bool RequestBodyAcceptable { get; }

        /// <summary>
        /// Append request body to handler
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        void RequestBodyArrival(byte[] buffer, int length);

        /// <summary>
        /// Finish append request body
        /// </summary>
        void AllRequestBodyReceived();

        /// <summary>
        /// Get the response body length
        /// </summary>
        long ResponseBodyLength { get; }

        /// <summary>
        /// Get the response body
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        int SubmitResponseBody(out byte[] data, out int offset);

        /// <summary>
        /// Close this processor
        /// </summary>
        void Close();
    }
}
