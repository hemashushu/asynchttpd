using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Net;

namespace Doms.HttpService.Configuration
{
    public class BindConfigElement:ConfigurationElement
    {

        [ConfigurationProperty("address",IsRequired=true,DefaultValue="*")]
        public string Address
        {
            get { return (string)this["address"]; }
            set { this["address"] = value; }
        }

        [ConfigurationProperty("port", IsRequired = true, DefaultValue = 80)]
        [IntegerValidator(MinValue=1,MaxValue=65535)]
        public int Port
        {
            get { return (int)this["port"]; }
            set { this["port"] = value; }
        }

        [ConfigurationProperty("name", IsRequired = true, DefaultValue = "httpd001")]
        [RegexStringValidator("^[0-9a-zA-Z_-]+$")]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        public IPEndPoint ToEndPoint()
        {
            IPAddress address = IPAddress.Any;
            if (Address != "*")
            {
                address =IPAddress.Parse(Address);
            }
            IPEndPoint ep = new IPEndPoint(address, Port);
            return ep;
        }

    }
}
