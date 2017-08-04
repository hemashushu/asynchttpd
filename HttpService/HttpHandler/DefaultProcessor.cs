using System;
using System.Collections.Generic;
using System.Text;
using Doms.HttpService.HttpProtocol;
using Doms.HttpService.HttpHandler;

namespace Doms.HttpService.HttpHandler
{
    /// <summary>
    /// Default processor
    /// </summary>
    class DefaultProcessor:IHttpRequestProcessor
    {
        //NOTE::
        //when all handlers cannot create a processor, webserver will use this processor to 
        //handle the request.

        private HttpResponseHeader _responseHeader;

        #region IHttpRequestProcessor Members

        public void ProcessRequest(HandlerContext context)
        {
            //return 'NotImplemented' http status, without response body
            _responseHeader = context.ResponseHeader;
            _responseHeader.Status = System.Net.HttpStatusCode.NotImplemented;
            _responseHeader.ContentLength = 0;
        }

        public bool RequestBodyAcceptable
        {
            get { return false; }
        }

        public void RequestBodyArrival(byte[] buffer, int length)
        {
            throw new InvalidOperationException("Can not accpet request body");
        }

        public void AllRequestBodyReceived()
        {
            throw new InvalidOperationException("Can not accpet request body");
        }

        public long ResponseBodyLength
        {
            get { return 0; }
        }

        public int SubmitResponseBody(out byte[] data, out int offset)
        {
            throw new InvalidOperationException("No response body");
        }

        public void Close()
        {
            //do nothing
        }

        #endregion

    }
}
