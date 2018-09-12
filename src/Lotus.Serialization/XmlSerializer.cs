using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using Lotus.Core;

namespace Lotus.Serialization
{
    public class XmlSerializer
    {
        public String Serialize(XDocument doc)
        {
            if (doc == null)
            {
                return String.Empty;
            }

            using (var ms = new MemoryStream())
            {
                doc.Save(ms, SaveOptions.DisableFormatting);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        public XResult<T> Deserialize<T>(String xml)
        {
            if (String.IsNullOrWhiteSpace(xml))
            {
                return new XResult<T>(default(T));
            }

            XDocument doc = null;
            try
            {
                doc = XDocument.Parse(xml, LoadOptions.None);
            }
            catch (Exception ex)
            {
                return new XResult<T>(default(T), ex);
            }

            if (doc == null)
            {
                return new XResult<T>(default(T), new NullReferenceException(nameof(doc)));
            }

            

            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取指定名称的驼峰命名名称
        /// </summary>
        /// <param name="name"></param>
        public String GetCamelName(String name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                return String.Empty;
            }

            String firstChar = ((Char)(name[0] < 97 ? (name[0] + 32) : name[0])).ToString();
            return name.Length > 1 ? firstChar + name.Substring(1) : firstChar;
        }

        /// <summary>
        /// 获取指定名的Pascal命名名称。
        /// </summary>
        /// <param name="name"></param>
        public String GetPascalName(String name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                return String.Empty;
            }

            String firstChar = ((Char)(name[0] >= 97 ? (name[0] - 32) : name[0])).ToString();
            return name.Length > 1 ? firstChar + name.Substring(1) : firstChar;
        }
    }
}
