using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using Doms.HttpService.HttpProtocol;
using Doms.HttpService.HttpHandler;

namespace Doms.BaseWebServer.SimpleHttpHandler.Processers
{
    public class HttpGet:IHttpRequestProcessor
    {
        private MemoryStream _outputStream;

        #region IHttpRequestProcessor Members

        public void ProcessRequest(HandlerContext context)
        {
            _outputStream = new MemoryStream();
            TextWriter writer = new StreamWriter(_outputStream, Encoding.UTF8);
            writer.WriteLine("<html><head><meta http-equiv=\"content-type\" content=\"text/html; charset=UTF-8\"><title>DomsHttpd test</title></head>");
            writer.WriteLine("<body><h1>Hello world, DomsHttpd!</h1>");
            writer.WriteLine("<br/>request URL is: {0}", context.RequestHeader.Url);
            writer.WriteLine("<br/>user agent: {0}", context.RequestHeader.UserAgent);
            writer.WriteLine("<br/>try <a href=\"/index?id=" + this.GetHashCode().ToString() + "\">change page</a>");
            writer.WriteLine("<br/><form id=\"form1\" method=\"post\" action=\"activepage\">");
            writer.WriteLine("<br/>input some words: <input type=\"text\" name=\"field1\" />");
            writer.WriteLine("<br/><input type=\"submit\" value=\"提交\" />");
            writer.WriteLine("</form>");
            writer.WriteLine("</body></html>");
            writer.Flush();

            _outputStream.Seek(0, SeekOrigin.Begin);

            HttpResponseHeader response = context.ResponseHeader;
            response.Status = System.Net.HttpStatusCode.OK;
            response.ContentType = "text/html";
            response.ContentLength = _outputStream.Length;
        }

        public bool RequestBodyAcceptable
        {
            get { return false; }
        }

        public void RequestBodyArrival(byte[] buffer, int length)
        {
            throw new InvalidOperationException();
        }

        public void AllRequestBodyReceived()
        {
            throw new InvalidOperationException();
        }

        public long ResponseBodyLength
        {
            get { return _outputStream.Length; }
        }

        public int SubmitResponseBody(out byte[] data, out int offset)
        {
            byte[] buffer = new byte[16 * 1024];
            int readByte = _outputStream.Read(buffer, 0, buffer.Length);
            if (readByte == 0) _outputStream.Close();
            data = buffer;
            offset = 0;
            return readByte;
        }

        public void Close()
        {
            _outputStream.Close();
        }

        #endregion


    }
}
