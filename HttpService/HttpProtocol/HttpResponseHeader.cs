using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.Specialized;

namespace Doms.HttpService.HttpProtocol
{
    /// <summary>
    /// Http response header
    /// </summary>
    public class HttpResponseHeader : IProtocolHeader
    {
        private NameValueList _headers;
        private System.Net.HttpStatusCode _status;

        private readonly Encoding _defaultEncoding = Encoding.ASCII;

        public HttpResponseHeader()
        {
            _headers = new NameValueList();
            _status = System.Net.HttpStatusCode.OK;
        }

        #region IProtocolHeader Members

        /// <summary>
        /// Get all headers
        /// </summary>
        public NameValueList Headers
        {
            get { return _headers; }
        }

        /// <summary>
        /// Append header
        /// </summary>
        public void AppendHeader(string name, string val)
        {
            _headers.Add(name, val);
        }

        public void SetHeaderRawData(byte[] data, int offset, int count)
        {
            throw new InvalidOperationException("Cannot set HeaderRawData at server side.");
        }

        /// <summary>
        /// Combine all headers into a byte[]
        /// </summary>
        /// <returns></returns>
        public byte[] GetHeaderRawData()
        {
            StringBuilder lines = new StringBuilder(512);

            lines.Append("HTTP/1.1\x20" + (int)_status + "\x20" + HttpResponseStatusCode.GetStatusName(_status) + "\r\n");

            //combine each header
            for (int idx = 0; idx < _headers.Count; idx++)
            {
                lines.Append(_headers.Names[idx]);
                lines.Append(":\x20");
                lines.Append(_headers.Values[idx]);
                lines.Append("\r\n");
            }

            //the last line
            lines.Append("\r\n");
            return _defaultEncoding.GetBytes(lines.ToString());
        }
        #endregion

        #region response headers value
        public System.Net.HttpStatusCode Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public DateTime Date
        {
            get
            {

                //NOTE::this is GMT time
                string dateValue = _headers.Get("Date");
                DateTime val;
                DateTime.TryParse(dateValue, out val);
                return val;
            }
            set
            {
                //NOTE::
                //use GMT DataTime format,see RFC 822, updated by RFC 1123
                // you must call DateTime.ToUniveralTime() first before set this value
                //Example:
                // Sun, 06 Nov 1994 08:49:37 GMT
                _headers.Set("Date", value.ToString("r"));
            }
        }

        public string Server
        {
            get { return _headers.Get("Server"); }
            set { _headers.Set("Server", value); }
        }

        /// <summary>
        /// can specify transfer encoding to: chunked
        /// </summary>
        public string TransferEncoding
        {
            get { return _headers.Get("Transfer-Encoding"); }
            set { _headers.Set("Transfer-Encoding", value); }
        }

        public string Location
        {
            get { return _headers.Get("Location"); }
            set { _headers.Set("Location", value); }
        }

        public string ContentType
        {
            get { return _headers.Get("Content-Type"); }
            set { _headers.Set("Content-Type", value); }
        }

        public long ContentLength
        {
            get
            {
                string contentLengthValue = _headers.Get("Content-Length");
                long val;
                long.TryParse(contentLengthValue, out val);
                return val;
            }
            set
            {
                //NOTE::
                //when the HTTP handler does not know the response body length,
                //the response header should not contains "Content-Length" segment.
                //so, if server does not know the response body length, do not set this property.
                _headers.Set("Content-Length", value.ToString());
            }
        }

        public string ContentEncoding
        {
            get
            {
                return _headers.Get("Content-Encoding");
            }
            set
            {
                _headers.Set("Content-Encoding", value);
            }
        }

        public string ContentRange
        {
            //NOTE::
            //use for resume transfer in GET method
            //Example:
            // Content-Range: bytes 21010-47021/47022
            // Content-Range: bytes 0-499/*
            //the 3 digital means: start position, end position, total body length
            get { return _headers.Get("Content-Range"); }
            set { _headers.Set("Content-Range", value); }
        }

        public string AcceptRange
        {
            //Example:
            //Accept-Ranges: bytes
            get { return _headers.Get("Accept-Ranges"); }
            set { _headers.Set("Accept-Ranges", value); }
        }

        public string ETag
        {
            get { return _headers.Get("ETag"); }
            set { _headers.Set("ETag", value); }
        }

        public DateTime LastModified
        {
            get
            {
                //NOTE:: this is GMT time
                string lastModifiedValue = _headers.Get("Last-Modified");
                DateTime val;
                DateTime.TryParse(lastModifiedValue, out val);
                return val;
            }
            set
            {
                //NOTE::
                //use GMT DataTime format,see RFC 822, updated by RFC 1123
                //Example:
                //  Sun, 06 Nov 1994 08:49:37 GMT
                _headers.Set("Last-Modified", value.ToString("r"));
            }
        }

        public string Connection
        {
            get { return _headers.Get("Connection"); }
            set { _headers.Set("Connection", value); }
        }

        public DateTime Expires
        {
            get
            {
                //NOTE:: this is GMT time
                string expiresValue = _headers.Get("Expires");
                DateTime val;
                DateTime.TryParse(expiresValue, out val);
                return val;
            }
            set
            {
                //NOTE::
                //use GMT DataTime format,see RFC 822, updated by RFC 1123
                //Example:
                //  Sun, 06 Nov 1994 08:49:37 GMT
                _headers.Set("Expires", value.ToString("r"));
            }
        }
        #endregion

    }
}
