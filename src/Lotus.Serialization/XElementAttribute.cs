using System;
using System.Collections.Generic;
using System.Reflection;
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
                this.Namespace = namespaceName;
            }
        }

        public String ElementName { get; }
        public String Namespace { get; }
    }

    public static class XElementAttributeExtensions
    {
        public static Boolean IsXElement(this PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                return false;
            }

            return propertyInfo.GetCustomAttribute<XElementAttribute>() != null;
        }

        public static Boolean IsXCollection(this PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                return false;
            }

            return propertyInfo.GetCustomAttribute<XCollectionAttribute>() != null;
        }
    }
}
