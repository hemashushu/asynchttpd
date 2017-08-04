using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using Doms.HttpService.HttpProtocol;
using Doms.HttpService.HttpHandler;

namespace Doms.BaseWebServer.SimpleHttpHandler.Processers
{
    public class HttpPost:IHttpRequestProcessor
    {
        private HandlerContext _context;
        private MemoryStream _inputStream;
        private MemoryStream _outputStream;

        #region IHttpRequestProcessor Members

        public void ProcessRequest(HandlerContext context)
        {
            _context = context;
            _inputStream = new MemoryStream();
        }

        public bool RequestBodyAcceptable
        {
            get { return true; }
        }

        public void RequestBodyArrival(byte[] buffer, int length)
        {
            _inputStream.Write(buffer, 0, length);
        }

        public void AllRequestBodyReceived()
        {
            _inputStream.Seek(0, SeekOrigin.Begin);
            TextReader reader = new StreamReader(_inputStream, Encoding.UTF8);
            string content = System.Web.HttpUtility.UrlDecode(reader.ReadToEnd());
            reader.Close();

            _outputStream = new MemoryStream();
            TextWriter writer = new StreamWriter(_outputStream, Encoding.UTF8);
            writer.WriteLine("<html><head><meta http-equiv=\"content-type\" content=\"text/html; charset=UTF-8\"><title>DomsHttpd test</title></head>");
            writer.WriteLine("<body><h1>DomsHttpd POST</h1>");
            writer.WriteLine("<br/>referer URL is: {0}", _context.RequestHeader.Referer);
            writer.WriteLine("<br/>the form content length: {0} bytes", _context.RequestHeader.ContentLength);
            writer.WriteLine("<br/>the form content is: <h3>{0}</h3>", System.Web.HttpUtility.HtmlEncode(content));
            writer.WriteLine("</body></html>");
            writer.Flush();

            _outputStream.Seek(0, SeekOrigin.Begin);

            HttpResponseHeader response = _context.ResponseHeader;
            response.Status = System.Net.HttpStatusCode.OK;
            response.ContentType = "text/html";
            response.ContentLength = _outputStream.Length;
        }

        public long ResponseBodyLength
        {
            get
            {
                if (_outputStream == null)
                    return 0;
                else
                    return _outputStream.Length;
            }
        }

        public int SubmitResponseBody(out byte[] data, out int offset)
        {
            if (_outputStream == null)
            {
                data = null;
                offset = 0;
                return 0;
            }
            else
            {
                byte[] buffer = new byte[16 * 1024];
                int readByte = _outputStream.Read(buffer, 0, buffer.Length);
                if (readByte == 0) _outputStream.Close();
                data = buffer;
                offset = 0;
                return readByte;
            }
        }

        public void Close()
        {
            _outputStream.Close();
        }

        #endregion

    }
}
