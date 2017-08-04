using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace Doms.HttpService.HttpProtocol
{
    /// <summary>
    /// Http style request or response header
    /// </summary>
    public interface IProtocolHeader
    {
        /// <summary>
        /// Return all headers
        /// </summary>
        NameValueList Headers { get; }

        /// <summary>
        /// Append a header
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        void AppendHeader(string name, string val);

        /// <summary>
        /// Set the header raw data
        /// </summary>
        void SetHeaderRawData(byte[] data, int offset, int count);

        /// <summary>
        /// Get the header raw data
        /// </summary>
        /// <returns></returns>
        byte[] GetHeaderRawData();

    }
}
