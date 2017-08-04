using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.Specialized;

namespace Doms.HttpService.HttpProtocol
{
    /// <summary>
    /// Http request header
    /// </summary>
    public class HttpRequestHeader : IProtocolHeader
    {
        private NameValueList _headers;
        private HttpMethods _method;
        private string _url;
        //private byte[] _urlRawData;

        private readonly Encoding _defaultEncoding = Encoding.ASCII;

        public HttpRequestHeader()
        {
            _headers = new NameValueList();
            _method = HttpMethods.GET;
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
            if (data.Length == 0 || offset + count > data.Length || count == 0)
            {
                throw new ArgumentException("Invalid header data");
            }

            //parse the first line
            int firstLinePos = Array.IndexOf<byte>(data, (byte)'\n', offset, count);
            if (firstLinePos > 0)
            {
                int pos1 = Array.IndexOf<byte>(data, (byte)'\x20', offset, firstLinePos - offset); //find first space
                int pos2 = Array.LastIndexOf<byte>(data, (byte)'\x20', firstLinePos - 1, firstLinePos - pos1 - 1); //find the last space

                if (pos2 > pos1 + 1 && pos1 > 0)
                {
                    _method = HttpMethodsParser.Parse(_defaultEncoding.GetString(data, offset, pos1 - offset));

                    //Need ??
                    //_urlRawData = new byte[pos2 - pos1 - 1];
                    //Buffer.BlockCopy(data, pos1 + 1, _urlRawData, 0, _urlRawData.Length);
                    //_url = _defaultEncoding.GetString(_urlRawData);

                    _url = _defaultEncoding.GetString(data, pos1 + 1, pos2 - pos1 - 1);
                }
                else
                {
                    throw new ArgumentException("Invalid request header");
                }
            }
            else
            {
                throw new ArgumentException("Invalid request header");
            }

            //parse other lines
            if (firstLinePos < offset + count)
            {
                //split into lines
                string[] lines = _defaultEncoding.GetString(data, firstLinePos + 1, count - firstLinePos).Split('\n');
                foreach (string line in lines)
                {
                    int pos1 = line.IndexOf(':');
                    if (pos1 > 0)
                    {
                        string headerName = line.Substring(0, pos1);
                        string headerValue = line.Substring(pos1 + 1, line.Length - pos1 - 2);
                        _headers.Add(headerName, headerValue);
                    }
                }
            }
        }

        public byte[] GetHeaderRawData()
        {
            throw new InvalidOperationException("Cannot get HeaderRawData at server side.");
        }
        #endregion

        #region request headers value
        public HttpMethods Method
        {
            get { return _method; }
        }

        public string Url
        {
            get { return _url; }
        }

        //public byte[] UrlRawData
        //{
        //    get { return _urlRawData; }
        //}

        public string Host
        {
            get { return _headers.Get("Host"); }
        }

        public string Referer
        {
            get { return _headers.Get("Referer"); }
        }

        public string UserAgent
        {
            get { return _headers.Get("User-Agent"); }
        }

        public string Connection
        {
            get { return _headers.Get("Connection"); }
        }

        public string ContentType
        {
            get { return _headers.Get("Content-Type"); }
        }

        public long ContentLength
        {
            get
            {
                string contentLengthValue = _headers.Get("Content-Length");
                long val = 0;
                long.TryParse(contentLengthValue, out val);
                return val;
            }
        }


        public string ContentRange
        {
            //NOTE::
            //use for resume transfer in PUT method
            //Example:
            // Content-Range: bytes 21010-47021/47022
            // Content-Range: bytes 0-499/*
            //the 3 digital means: start position, end position, total body length
            get { return _headers.Get("Content-Range"); }
        }

        public string AcceptRanges
        {
            //Example:
            // Accept-Ranges: bytes
            get { return _headers.Get("Accept-Ranges"); }
        }

        public string Range
        {
            //NOTE::
            //use for resume transfer in GET method
            //Example:
            // Range: bytes=123-456
            // Range: bytes=123-
            // Range: bytes=-456
            get { return _headers.Get("Range"); }
        }

        public DateTime IfModifiedSince
        {
            get
            {
                //NOTE::this is GMT time
                string ifModifiedSinceValue = _headers.Get("If-Modified-Since");
                DateTime val;
                DateTime.TryParse(ifModifiedSinceValue, out val);
                return val;
            }
            //set
            //{
            //NOTE::
            //use GMT DataTime format,see RFC 822, updated by RFC 1123
            //Example:
            // Sun, 06 Nov 1994 08:49:37 GMT
            //_headers.Set("If-Modified-Since", value.ToString("r"));
            //}
        }

        #endregion

    }
}
