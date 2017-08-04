using System;
using System.Collections.Generic;
using System.Text;
using Doms.HttpService.AsyncNetwork;
using Doms.HttpService.HttpProtocol;
using Doms.HttpService.HttpHandler;

namespace Doms.HttpService
{
    /// <summary>
    /// Server session for process client request and send response to client.
    /// </summary>
    class ServerSession
    {
        #region private variables
        //asynchronous socket network session
        private AsyncSocketSession _session;

        //for remember what is session doing now
        private SessionWorkStatus _workStatus;

        //the processor factory
        private ProcessorFactory _processorFactory;

        //the last processor
        private IHttpRequestProcessor _lastProcessor;
        private HttpResponseHeader _lastResponseHeader;

        //buffer for receiving request body and process result body data
        private byte[] _bodyBuffer;
        private const int BODY_BUFFER_LENGTH = 128 * 1024;

        //the client IP address and local bind EndPoint of network conntion
        private System.Net.IPAddress _clientIp;
        private System.Net.IPEndPoint _bindEndPoint;
        private string _bindEndPointName;

        //a token string, to identify each network connection
        private string _connectionToken;

        //a flag to indicate whether close network connection when one conversation is end
        private bool _clientKeepAlive;
        private bool _serverKeepAlive;

        //a flag to indicate session is closing
        private bool _isClosing;

        //the session last active time, for calculate idle time
        private DateTime _lastActiveTime;
        private const int _maxKeepAliveTime = 180;

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        #endregion

        #region events
        //when session has close, raise this event
        public event EventHandler SessionClose;

        #endregion

        #region properties
        /// <summary>
        /// Connection token for identify each network connection
        /// </summary>
        public string ConnectionToken
        {
            get { return _connectionToken; }
        }

        public System.Net.IPAddress ClientIpAddress
        {
            get { return _clientIp; }
        }

        public string BindEndPointName
        {
            get { return _bindEndPointName; }
        }
        #endregion

        #region check idle
        /// <summary>
        /// Check current connection whether hold too long time
        /// </summary>
        /// <returns></returns>
        public bool CheckIfIdle(int timeout)
        {
            TimeSpan span = DateTime.Now - _lastActiveTime;
            long elapsedSecond = (long)span.TotalSeconds;
            if (_workStatus == SessionWorkStatus.ReceivingRequestHeader &&
                elapsedSecond >= timeout)
            {
                return true;
            }
            else if (elapsedSecond >= _maxKeepAliveTime)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region start session

        public ServerSession(
            System.Net.Sockets.Socket workSocket,
            System.Net.IPEndPoint bindEndPoint,
            string bindEndPointName,
            bool serverKeepAlive,
            ProcessorFactory processorFactory)
        {
            workSocket.NoDelay = true; //donot use Nagle 

            _clientIp = ((System.Net.IPEndPoint)workSocket.RemoteEndPoint).Address;
            _bindEndPoint = bindEndPoint;
            _bindEndPointName = bindEndPointName;
            _serverKeepAlive = serverKeepAlive;
            _processorFactory = processorFactory;

            _session = new AsyncSocketSession(workSocket);
            _session.ConnectionClose += new EventHandler<ConnectionCloseEventArgs>(session_ConnectionClose);
            _session.SendComplete += new EventHandler<SendCompleteEventArgs>(session_SendComplete);
            _session.ReceiveHeaderComplete += new EventHandler<ReceiveHeaderCompleteEventArgs>(session_ReceiveHeaderComplete);
            _session.ReceiveBodyComplete += new EventHandler<ReceiveBodyCompleteEventArgs>(session_ReceiveBodyComplete);

            _connectionToken = System.Guid.NewGuid().ToString("N");
            _bodyBuffer = new byte[BODY_BUFFER_LENGTH];

            //set the active time
            _lastActiveTime = DateTime.Now;
        }

        /// <summary>
        /// Start session and enter receiving request status
        /// </summary>
        public void StartSession()
        {
            //start receive request
            _workStatus = SessionWorkStatus.ReceivingRequestHeader;
            HttpRequestHeader requestHeader = new HttpRequestHeader();
            _session.Receive(requestHeader, _bodyBuffer, 0, _bodyBuffer.Length);
        }
        #endregion

        #region close session
        /// <summary>
        /// Close session by force
        /// </summary>
        public void Close()
        {
            if (!_isClosing)
            {
                _isClosing = true;
                _session.Close();
            }
        }

        private void session_ConnectionClose(object sender, ConnectionCloseEventArgs e)
        {
            if (e.LastException != null)
            {
                logger.Debug("{0} session been close: {1}",
                    _connectionToken.Substring(0, 6),
                    e.LastException.Message);
            }

            //close the last processor
            if (_lastProcessor != null)
            {
                _lastProcessor.Close();
            }

            SessionClose(this, EventArgs.Empty);
        }
        #endregion

        #region receive request and process it
        private void session_ReceiveHeaderComplete(object sender, ReceiveHeaderCompleteEventArgs e)
        {
            logger.Debug("{0} receive header complete", _connectionToken.Substring(0, 6));

            _lastActiveTime = DateTime.Now;
            _workStatus = SessionWorkStatus.Busy;

            try
            {
                processRequest(e);
            }
            catch (Exception ex)
            {
                HttpRequestHeader request = (HttpRequestHeader)e.LastReceiveHeader; //for debug
                logger.Error("{0} unhandle exception occur while processing request: \r\n{1}:{2}\r\n{3}",
                    _connectionToken.Substring(0, 6),
                    request.Method,
                    request.Url,
                    ex.Message);
                Close();
            }
        }

        private void session_ReceiveBodyComplete(object sender, ReceiveBodyCompleteEventArgs e)
        {
            logger.Debug("{0} receive body complete", _connectionToken.Substring(0, 6));

            _lastActiveTime = DateTime.Now;
            _workStatus = SessionWorkStatus.Busy;

            try
            {
                _lastProcessor.RequestBodyArrival(e.Data, e.DataLength);

                if (e.TotalHasReceivedLength >= e.TotalPlanReceivingLength)
                {
                    //all body receive complete
                    _lastProcessor.AllRequestBodyReceived();
                }
            }
            catch (Exception ex)
            {
                //exception occur while append request body to handler, 
                //only PUT and POST will occur.
                logger.Warn("{0} append request to processor {1} body error: {2}",
                    _connectionToken.Substring(0, 6),
                    _lastProcessor.GetType().Name,
                    ex.Message);
                Close();
                return;
            }

            if (e.TotalHasReceivedLength >= e.TotalPlanReceivingLength)
            {
                //handle complete, send response
                sendResponse();
            }
            else
            {
                //continue to receive body
                _workStatus = SessionWorkStatus.ReceivingRequestBody;
                _session.ContinueReceivingBody(_bodyBuffer, 0, _bodyBuffer.Length);
            }
        }

        private void processRequest(ReceiveHeaderCompleteEventArgs e)
        {
            logger.Debug("{0} process request", _connectionToken.Substring(0, 6));

            if (_lastProcessor != null)
            {
                _lastProcessor.Close();
            }

            HttpRequestHeader requestHeader = (HttpRequestHeader)e.LastReceiveHeader;

            logger.Debug("{0} receive new request: {1}, url: {2}",
                _connectionToken.Substring(0, 6),
                requestHeader.Method,
                requestHeader.Url);

            //check if keep connection alive
            _clientKeepAlive = (requestHeader.Connection == null ||
                String.Compare(requestHeader.Connection, "keep-alive", true) == 0);

            _lastResponseHeader = new HttpResponseHeader();
            _lastResponseHeader.Date = DateTime.Now.ToUniversalTime();
            _lastResponseHeader.Server = HttpServiceControler.SERVER_NAME;

            HandlerContext context = new HandlerContext(
                requestHeader, _lastResponseHeader, _bindEndPoint, _bindEndPointName, _clientIp,_connectionToken);

            _lastProcessor = _processorFactory.CreateProcessor(context);
            _lastProcessor.ProcessRequest(context);

            //request contains body
            if (requestHeader.ContentLength > 0)
            {
                if (_lastProcessor.RequestBodyAcceptable)
                {
                    //continue to receive reqeust body
                    _workStatus = SessionWorkStatus.ReceivingRequestBody;
                    e.TotalPlanReceivingLength = requestHeader.ContentLength;
                }
                else
                {
                    //NOTE:: 
                    //can not accept request body, because
                    //some http method (GET,HEAD,DELETE) should not contain request body
                    //or POST,PUT access deny.
                    logger.Warn("{0} processor {1} can not accept request body",
                        _connectionToken.Substring(0, 6),
                        _lastProcessor.GetType().Name);
                    Close();
                }
            }
            else
            {
                sendResponse();
            }
        }

        #endregion

        #region send response
        private void sendResponse()
        {
            logger.Debug("{0} send response", _connectionToken.Substring(0, 6));

            //send response
            _workStatus = SessionWorkStatus.SendingResponseHeader;
            
            //append connection header if need
            if (_clientKeepAlive && !_serverKeepAlive)
            {
                _lastResponseHeader.Connection = "close";
            }

            if (_lastProcessor.ResponseBodyLength > 0)
            {
                //send response with body
                _session.Send(_lastResponseHeader, _lastProcessor.ResponseBodyLength);
            }
            else if (_lastProcessor.ResponseBodyLength == 0)
            {
                //send response header only
                _session.Send(_lastResponseHeader);
            }
            else
            {
                //NOTE::
                //send response in "chunked" transfer encoding mode(with unknow body length)
                _session.Send(_lastResponseHeader, long.MaxValue);
            }
        }

        private void session_SendComplete(object sender, SendCompleteEventArgs e)
        {
            logger.Debug("{0} response sent.", _connectionToken.Substring(0, 6));

            _lastActiveTime = DateTime.Now;

            if (_lastProcessor.ResponseBodyLength != 0)
            {
                int length = 0;
                byte[] buffer;
                int offset;

                try
                {
                    length = _lastProcessor.SubmitResponseBody(
                        out buffer, out offset);
                }
                catch (Exception ex)
                {
                    //exception occur while get response body from processer
                    logger.Warn("{0} exception occur while get response body from processer {1}, message: {2}",
                        _connectionToken.Substring(0, 6),
                        _lastProcessor.GetType().Name,
                        ex.Message);
                    Close();
                    return;
                }

                if (length > 0)
                {
                    _workStatus = SessionWorkStatus.SendingResponseBody;
                    _session.ContinueSendingBody(buffer, offset, length);
                    return;
                }

            }

            //close session if does not keep alive
            //NOTE::Let client to close the connection, otherwise, server will
            //generate lots of TIME_WAIT sockets

            ////if (!_serverKeepAlive)
            ////{
            ////    Close();
            ////}
            ////else
            ////{
                //continue receive next request
                _workStatus = SessionWorkStatus.ReceivingRequestHeader;
                HttpRequestHeader requestHeader = new HttpRequestHeader();
                _session.Receive(requestHeader, _bodyBuffer, 0, _bodyBuffer.Length);
            ////}
        }
        #endregion

    }//end class
}

//Copyright (c) 2007-2009, Kwanhong Young, All rights reserved.
//mapleaves@gmail.com
//http://www.domstorage.com