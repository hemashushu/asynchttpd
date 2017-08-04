using System;
using System.Collections.Generic;
using System.Text;
using Doms.HttpService.HttpProtocol;

namespace Doms.HttpService.HttpHandler
{
    /// <summary>
    /// Http handler interface, http handler use to create request processor
    /// </summary>
    public interface IHttpHandler
    {
        /// <summary>
        /// Create new http request processor
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        IHttpRequestProcessor CreatProcessor(HandlerContext context);

        /// <summary>
        /// Close this handler
        /// </summary>
        void Close();
    }
}
