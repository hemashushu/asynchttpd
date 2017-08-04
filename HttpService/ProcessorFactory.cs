using System;
using System.Collections.Generic;
using System.Text;
using Doms.HttpService.HttpProtocol;
using Doms.HttpService.HttpHandler;

namespace Doms.HttpService
{
    /// <summary>
    /// A factory for creating the request processor
    /// </summary>
    class ProcessorFactory
    {
        private List<IHttpHandler> _handlers;
        private DefaultProcessor _defaultProcessor;

        public ProcessorFactory()
        {
            _handlers = new List<IHttpHandler>();
            _defaultProcessor = new DefaultProcessor();
        }

        /// <summary>
        /// Add new handler to this factory
        /// </summary>
        /// <param name="handler"></param>
        public void AddHandler(IHttpHandler handler)
        {
            _handlers.Add(handler);
        }

        /// <summary>
        /// Create a new request processor
        /// </summary>
        /// <param name="requestHeader"></param>
        /// <returns></returns>
        public IHttpRequestProcessor CreateProcessor(HandlerContext context)
        {
            IHttpRequestProcessor processor = null;

            for (int idx = 0; idx < _handlers.Count; idx++)
            {
                processor = _handlers[idx].CreatProcessor(context);
                if (processor != null) break;
            }

            if (processor == null)
            {
                //return default processor if no handler can create processor
                return _defaultProcessor;
            }
            else
            {
                return processor;
            }
        }
    }
}
