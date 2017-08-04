using System;
using System.Collections.Generic;
using System.Text;
using Doms.HttpService.HttpProtocol;

namespace Doms.HttpService.HttpHandler
{
    /// <summary>
    /// Http handler context
    /// </summary>
    public class HandlerContext
    {
        private HttpRequestHeader _requestHeader;
        private HttpResponseHeader _responseHeader;
        private System.Net.IPEndPoint _bindEndPoint;
        private string _bindEndPointName;
        private System.Net.IPAddress _clientIPAddress;
        private string _connectionToken;

        public HandlerContext(
            HttpRequestHeader requestHeader,
            HttpResponseHeader responseHeader,
            System.Net.IPEndPoint bindEndPoint,
            string bindEndPointName,
            System.Net.IPAddress clientIPAddress,
            string connectionToken)
        {
            _requestHeader = requestHeader;
            _responseHeader = responseHeader;
            _bindEndPoint = bindEndPoint;
            _bindEndPointName = bindEndPointName;
            _clientIPAddress = clientIPAddress;
            _connectionToken = connectionToken;
        }

        public HttpRequestHeader RequestHeader
        {
            get { return _requestHeader; }
        }

        public HttpResponseHeader ResponseHeader
        {
            get { return _responseHeader; }
        }

        public System.Net.IPEndPoint BindEndPoint
        {
            get { return _bindEndPoint; }
        }

        public string BindEndPointName
        {
            get { return _bindEndPointName; }
        }

        public System.Net.IPAddress ClientIPAddress
        {
            get { return _clientIPAddress; }
        }

        public string ConnectionToken
        {
            get { return _connectionToken; }
        }
    }
}
