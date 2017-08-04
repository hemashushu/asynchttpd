using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Doms.HttpService.Configuration
{
    public class HttpServiceConfigSection:ConfigurationSection
    {
        #region instance
        private static HttpServiceConfigSection _instance =
            (HttpServiceConfigSection)ConfigurationManager.GetSection("domsHttpd");

        public static HttpServiceConfigSection Instance
        {
            get { return _instance; }
        }
        #endregion

        [ConfigurationProperty("binds", IsRequired = true)]
        [ConfigurationCollection(typeof(BindConfigElementCollection),
            AddItemName = "endpoint")]
        public BindConfigElementCollection Binds
        {
            get { return (BindConfigElementCollection)this["binds"]; }
            set { this["binds"] = value; }
        }

        [ConfigurationProperty("connection", IsRequired = true)]
        public ConnectionConfigElement Connection
        {
            get { return (ConnectionConfigElement)this["connection"]; }
            set { this["connection"] = value; }
        }
    }
}
