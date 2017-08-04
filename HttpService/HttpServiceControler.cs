using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Doms.HttpService.HttpHandler;
using Doms.HttpService.Configuration;

namespace Doms.HttpService
{
    /// <summary>
    /// Http server controler
    /// </summary>
    public class HttpServiceControler
    {
        #region private variables
        //local network endpoint listener
        private List<Listener> _listeners;

        //processor factory
        private ProcessorFactory _processorFactory;

        //connection monitor
        private IConnectionMonitor _connMon;

        //network connection sessions collection
        private Dictionary<string, ServerSession> _sessions;

        //config value, include max connections, keep alive, timeout ...
        private readonly int _maxConnections;
        private readonly bool _serverKeepAlive;
        private readonly int _keepAliveTimeout;

        //timer for scan idle connections
        private System.Timers.Timer _scanTimer;

        //scan interval, default is 5 seconds
        private const int SCAN_TIME_INTERVAL = 5 * 1000;

        //synchronous object
        private object _syncObject;

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        #endregion

        public const string SERVER_NAME = "DomsHttpd/2.0";

        #region init
        public HttpServiceControler()
        {
            _syncObject = new object();

            _listeners = new List<Listener>();
            _processorFactory = new ProcessorFactory();
            _sessions = new Dictionary<string, ServerSession>();

            //read config
            HttpServiceConfigSection config = HttpServiceConfigSection.Instance;
            _maxConnections = config.Connection.ConnectionsLimit;
            _serverKeepAlive = config.Connection.KeepAlive;
            _keepAliveTimeout = config.Connection.Timeout;

            //add binds
            foreach (BindConfigElement bind in config.Binds)
            {
                Listener listener = new Listener(bind.ToEndPoint(), bind.Name);
                listener.NewConnectionAccepted +=
                    new EventHandler<AcceptNewConnectionEventArgs>(listener_NewConnectionAccepted);
                _listeners.Add(listener);
            }

            //timer for scan idle connections
            if (_serverKeepAlive)
            {
                _scanTimer = new System.Timers.Timer(SCAN_TIME_INTERVAL);
                _scanTimer.AutoReset = true;
                _scanTimer.Elapsed += new System.Timers.ElapsedEventHandler(removeIdle);
                _scanTimer.Start();
            }
        }
        #endregion

        #region service control
        /// <summary>
        /// Start web service
        /// </summary>
        public void Start()
        {
            foreach (Listener listener in _listeners)
            {
                logger.Info("Listening network endpoint: {0}", listener.BindEndPoint);
                try
                {
                    listener.StartListen();
                }
                catch (Exception ex)
                {
                    logger.Error("Listening the network end-point {0} error: {1}", listener.BindEndPoint, ex.Message);
                    throw new ApplicationException("Service start fail");
                }
            }
        }

        /// <summary>
        /// Stop web service
        /// </summary>
        public void Stop()
        {
            logger.Info("Stopping http service");

            foreach (Listener listener in _listeners)
            {
                listener.StopListen();
            }
        }
        #endregion

        #region remove idle session
        /// <summary>
        /// Scan and find idle sessions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeIdle(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                //find idle sessions
                List<ServerSession> idles = new List<ServerSession>();
                lock (_syncObject)
                {
                    foreach (ServerSession session in _sessions.Values)
                    {
                        if (session.CheckIfIdle(_keepAliveTimeout)) idles.Add(session);
                    }
                }

                //close all idle sessions
                if (idles.Count > 0)
                {
                    logger.Debug("Found {0} idle network connections", idles.Count);

                    foreach (ServerSession session in idles)
                    {
                        session.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occur while removing idle connections, {0}", ex.Message);
            }
        }
        #endregion

        #region session connect and close
        /// <summary>
        /// New connection accepted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listener_NewConnectionAccepted(object sender, AcceptNewConnectionEventArgs e)
        {
            //the acive connections has exceed limit
            if (_sessions.Count >= _maxConnections)
            {
                try
                {
                    e.WorkSocket.Shutdown(System.Net.Sockets.SocketShutdown.Both);
                    e.WorkSocket.Close();
                }
                catch (Exception ex)
                {
                    logger.Info("server connections exceed the limited, close the connection: {0}", ex.Message);
                }
            }
            else
            {
                try
                {
                    //create new server session
                    ServerSession session = new ServerSession(
                        e.WorkSocket, e.BindEndPoint, e.BindEndPointName, _serverKeepAlive, _processorFactory);

                    logger.Debug("{0} new connection created on: {1} (binding name = {2})",
                         session.ConnectionToken.Substring(0, 6),
                         e.BindEndPoint, e.BindEndPointName);

                    //notify connection monitor
                    if (_connMon != null)
                    {
                        bool cancel = false;
                        _connMon.ConnectionStart(session.ConnectionToken,
                            session.ClientIpAddress, session.BindEndPointName, ref cancel);
                        if (cancel == true)
                        {
                            e.WorkSocket.Shutdown(System.Net.Sockets.SocketShutdown.Both);
                            e.WorkSocket.Close();
                            return;
                        }
                    }

                    //add session to collection
                    lock (_syncObject)
                    {
                        _sessions.Add(session.ConnectionToken, session);
                    }

                    session.SessionClose += new EventHandler(session_SessionClose);
                    session.StartSession();
                }
                catch (Exception ex)
                {
                    logger.Error("create new connection session fail: {0}", ex.Message);
                }
            }
        }

        /// <summary>
        /// Connection closed, remove the session from collection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void session_SessionClose(object sender, EventArgs e)
        {
            ServerSession session = (ServerSession)sender;
            logger.Debug("{0} network connection closed", session.ConnectionToken.Substring(0, 6));

            if (_connMon != null)
            {
                try
                {
                    _connMon.ConnectionEnd(session.ConnectionToken, 
                        session.ClientIpAddress, session.BindEndPointName);
                }
                catch (Exception ex)
                {
                    logger.Warn("notify disconnection signal to connection monitor fail: {0}", ex.Message);
                }
            }

            lock (_syncObject)
            {
                _sessions.Remove(session.ConnectionToken);
            }
        }
        #endregion

        #region properties
        /// <summary>
        /// Get the active network connections amount
        /// </summary>
        public int Connections
        {
            get { return _sessions.Count; }
        }
        #endregion

        #region http handler
        /// <summary>
        /// Add HTTP handler to current web server
        /// </summary>
        /// <param name="handler"></param>
        public void AddHandler(IHttpHandler handler)
        {
            logger.Debug("Add http handler: {0}", handler.GetType().Name);
            _processorFactory.AddHandler(handler);
        }

        public void SetConnectionMonitor(IConnectionMonitor monitor)
        {
            logger.Debug("Add connection monitor: {0}", monitor.GetType().Name);
            _connMon = monitor;
        }
        #endregion

    }//end class
}
