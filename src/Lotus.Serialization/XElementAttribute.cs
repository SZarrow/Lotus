using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Lotus.Serialization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Enum)]
    public class XElementAttribute : Attribute
    {
        public XElementAttribute(String elementName, String namespaceName = null)
        {
            if (String.IsNullOrWhiteSpace(elementName))
            {
                throw new ArgumentNullException(nameof(elementName));
            }

            this.ElementName = elementName;

            if (!String.IsNullOrWhiteSpace(namespaceName))
            {
                this.Namespace = XNamespace.Get(namespaceName);
            }
        }

        public String ElementName { get; }
        public XNamespace Namespace { get; }
    }
}
