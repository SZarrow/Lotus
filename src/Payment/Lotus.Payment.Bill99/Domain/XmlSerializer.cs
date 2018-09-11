using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace Lotus.Payment.Bill99.Domain
{
    public abstract class XmlSerializer<T>
    {
        private readonly XDocument _doc;

        protected XmlSerializer()
        {
            _doc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"));
        }

        public String ToXml()
        {
            BuildDocument(_doc);

            using (var ms = new MemoryStream())
            {
                _doc.Save(ms, SaveOptions.DisableFormatting);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        public T FromXml(String xml)
        {
            throw new NotImplementedException();
        }

        public abstract void BuildDocument(XDocument doc);

        protected String GetCamelName(String name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                return String.Empty;
            }

            String firstChar = ((Char)(name[0] < 97 ? (name[0] + 32) : name[0])).ToString();
            return name.Length > 1 ? firstChar + name.Substring(1) : firstChar;
        }
    }
}
