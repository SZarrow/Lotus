using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Lotus.Core;

namespace Lotus.Serialization
{
    public class XmlSerializer
    {
        public String Serialize(Object value)
        {
            var doc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"));
            var eleStack = new Stack<XElement>();
            var typeStack = new Stack<Type>();

            var instanceType = value.GetType();
            typeStack.Push(instanceType);

            while (instanceType.BaseType != null
                && instanceType.BaseType != typeof(Object))
            {
                typeStack.Push(instanceType.BaseType);
                instanceType = instanceType.BaseType;
            }

            while (typeStack.Count > 0)
            {
                var type = typeStack.Pop();
                BuildDocTree(eleStack, type);
            }

            XElement rootEl = null;
            while (eleStack.Count > 1)
            {
                var childEl = eleStack.Pop();
                var parentEl = eleStack.Pop();

                parentEl.Add(childEl);

                if (eleStack.Count == 0)
                {
                    rootEl = parentEl;
                    break;
                }
            }

            doc.Add(rootEl);

            using (var ms = new MemoryStream())
            {
                doc.Save(ms, SaveOptions.DisableFormatting | SaveOptions.OmitDuplicateNamespaces);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        private void BuildDocTree(Stack<XElement> stack, Type instanceType)
        {
            var insCusAttr = instanceType.GetCustomAttribute<XElementAttribute>(false);
            if (insCusAttr != null)
            {
                var xel = new XElement(insCusAttr.ElementName);

                //如果当前节点自己有命名空间，则添加自己的命名空间
                if (!String.IsNullOrWhiteSpace(insCusAttr.Namespace))
                {
                    xel.Name = XName.Get(insCusAttr.ElementName, insCusAttr.Namespace);
                }
                else
                {
                    //如果当前节点自己没有命名空间，但是父节点有命名空间，
                    //则将使用父节点的命名作为自己的命名空间
                    String parentNamespace = stack.Count > 0 ? stack.Peek().Name.NamespaceName : null;
                    if (!String.IsNullOrWhiteSpace(parentNamespace))
                    {
                        xel.Name = XName.Get(insCusAttr.ElementName, parentNamespace);
                    }
                }

                stack.Push(xel);
            }

            var insProperties = instanceType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.DeclaredOnly);
            if (insProperties != null && insProperties.Length > 0)
            {
                var parentEl = stack.Peek();
                foreach (var insProp in insProperties)
                {
                    var insPropAttr = insProp.GetCustomAttribute<XElementAttribute>();
                    if (insPropAttr != null)
                    {
                        var insPropEl = new XElement(insPropAttr.ElementName);

                        if (!String.IsNullOrWhiteSpace(insPropAttr.Namespace))
                        {
                            insPropEl.Name = XName.Get(insPropAttr.ElementName, insPropAttr.Namespace);
                        }
                        else
                        {
                            String parentNamespace = stack.Count > 0 ? stack.Peek().Name.NamespaceName : null;
                            if (!String.IsNullOrWhiteSpace(parentNamespace))
                            {
                                insPropEl.Name = XName.Get(insPropAttr.ElementName, parentNamespace);
                            }
                        }

                        parentEl.Add(insPropEl);
                    }
                }
            }
        }

        public XResult<T> Deserialize<T>(String xml)
        {
            if (String.IsNullOrWhiteSpace(xml))
            {
                return new XResult<T>(default(T), new ArgumentNullException(nameof(xml)));
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
