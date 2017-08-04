using System;
using System.Collections.Generic;
using System.Text;
using Doms.HttpService.HttpProtocol;
using Doms.HttpService.HttpHandler;

using Doms.BaseWebServer.SimpleHttpHandler.Processers;

namespace Doms.BaseWebServer.SimpleHttpHandler
{
    public class SimpleHandler:IHttpHandler
    {
        #region IHttpHandler Members

        public IHttpRequestProcessor CreatProcessor(HandlerContext context)
        {
            switch (context.RequestHeader.Method)
            {
                case HttpMethods.GET:
                    return new HttpGet();
                case HttpMethods.POST:
                    return new HttpPost();
            }
            return null;
        }

        public void Close()
        {
            //do nothing
        }

        #endregion
    }
}
