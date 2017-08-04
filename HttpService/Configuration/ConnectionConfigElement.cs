using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Doms.HttpService.Configuration
{
    public class ConnectionConfigElement:ConfigurationElement
    {
        [ConfigurationProperty("keepAlive",IsRequired=true, DefaultValue=true)]
        public bool KeepAlive
        {
            get { return (bool)this["keepAlive"]; }
            set { this["keepAlive"] = value; }
        }

        [ConfigurationProperty("timeout",IsRequired=true,DefaultValue=180)]
        [IntegerValidator(MinValue=30,MaxValue=3600)]
        public int Timeout
        {
            get { return (int)this["timeout"]; }
            set { this["timeout"] = value; }
        }

        [ConfigurationProperty("connectionsLimit", IsRequired = true, DefaultValue = 5000)]
        [IntegerValidator(MinValue=1,MaxValue=60000)]
        public int ConnectionsLimit
        {
            get { return (int)this["connectionsLimit"]; }
            set { this["connectionsLimit"] = value; }
        }

        
    }
}
