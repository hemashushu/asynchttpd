using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Doms.HttpService.Configuration
{
    public class BindConfigElementCollection:ConfigurationElementCollection
    {

        protected override ConfigurationElement CreateNewElement()
        {
            return new BindConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((BindConfigElement)element).ToEndPoint().ToString();
        }

        public void Add(BindConfigElement element)
        {
            base.BaseAdd(element);
        }

    }
}
