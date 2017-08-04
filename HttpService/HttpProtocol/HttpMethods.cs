using System;
using System.Collections.Generic;
using System.Text;

namespace Doms.HttpService.HttpProtocol
{
    /// <summary>
    /// Http request methods
    /// </summary>
    public enum HttpMethods
    {
        OPTIONS,
        TRACE,
        GET,
        HEAD,
        DELETE,
        PUT,
        POST,
        COPY,
        MOVE,
        MKCOL,
        PROPFIND,
        PROPPATCH,
        LOCK,
        UNLOCK,
        SEARCH,
        CONNECT
    }//end enum

    class HttpMethodsParser
    {
        public static HttpMethods Parse(string method)
        {
            HttpMethods val = HttpMethods.GET;

            switch (method.ToUpper())
            {
                #region http methods
                case "OPTIONS":
                    val = HttpMethods.OPTIONS;
                    break;
                case "TRACE":
                    val = HttpMethods.TRACE;
                    break;
                case "GET":
                    val = HttpMethods.GET;
                    break;
                case "HEAD":
                    val = HttpMethods.HEAD;
                    break;
                case "DELETE":
                    val = HttpMethods.DELETE;
                    break;
                case "PUT":
                    val = HttpMethods.PUT;
                    break;
                case "POST":
                    val = HttpMethods.POST;
                    break;
                case "COPY":
                    val = HttpMethods.COPY;
                    break;
                case "MOVE":
                    val = HttpMethods.MOVE;
                    break;
                case "MKCOL":
                    val = HttpMethods.MKCOL;
                    break;
                case "PROPFIND":
                    val = HttpMethods.PROPFIND;
                    break;
                case "PROPPATCH":
                    val = HttpMethods.PROPPATCH;
                    break;
                case "LOCK":
                    val = HttpMethods.LOCK;
                    break;
                case "UNLOCK":
                    val = HttpMethods.UNLOCK;
                    break;
                case "SEARCH":
                    val = HttpMethods.SEARCH;
                    break;
                case "CONNECT":
                    val = HttpMethods.CONNECT;
                    break;
                #endregion

                default:
                    throw new InvalidOperationException("No this http method.");
            }

            return val;
        }
    }//end class

}
