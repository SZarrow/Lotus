using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using LC = Lotus.Core;
using DotNetWheels.Core;

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
                BuildDocTree(value, eleStack, type);
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
                String result = Encoding.UTF8.GetString(ms.ToArray());
                return RemoveUTF8MarkChar(result);
            }
        }

        private void BuildDocTree(Object value, Stack<XElement> stack, Type instanceType)
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

                        Object propertyValue = insProp.XGetValue(value);
                        if (propertyValue != null)
                        {
                            insPropEl.Value = propertyValue.ToString();
                        }

                        parentEl.Add(insPropEl);
                    }
                }
            }
        }

        public LC.XResult<T> Deserialize<T>(String xml)
        {
            if (String.IsNullOrWhiteSpace(xml))
            {
                return new LC.XResult<T>(default(T), new ArgumentNullException(nameof(xml)));
            }

            xml = RemoveUTF8MarkChar(xml);

            XDocument doc = null;
            try
            {
                doc = XDocument.Parse(xml, LoadOptions.None);
            }
            catch (Exception ex)
            {
                return new LC.XResult<T>(default(T), ex);
            }

            if (doc == null)
            {
                return new LC.XResult<T>(default(T), new NullReferenceException(nameof(doc)));
            }

            var root = doc.Root;
            var targetType = typeof(T);

            if (String.Compare(root.Name.LocalName, targetType.Name, true) != 0)
            {
                return new LC.XResult<T>(default(T), new InvalidOperationException("the name of xml root isn't match name of target type"));
            }

            T instance = default(T);
            try
            {
                instance = Activator.CreateInstance<T>();
            }
            catch (Exception ex)
            {
                return new LC.XResult<T>(default(T), ex);
            }

            var eles = root.Elements();
            foreach (var el in eles)
            {
                if (el.HasElements)
                {
                    foreach (var elx in el.Elements())
                    {

                    }
                }
                else
                {
                    var propertyInfo = targetType.GetProperty(el.Name.LocalName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
                    if (propertyInfo != null)
                    {
                        var propertyValue = XConvert.TryParse(targetType, el.Value, null);
                        if (propertyValue.Success)
                        {
                            propertyInfo.XSetValue(instance, propertyValue.Value);
                        }
                    }
                }
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// 移除UTF8编码的标记字符
        /// </summary>
        /// <param name="xml">要移除UTF8编码标记字符的Xml字符串</param>
        public static String RemoveUTF8MarkChar(String xml)
        {
            String byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            if (xml.StartsWith(byteOrderMarkUtf8))
            {
                xml = xml.Remove(0, byteOrderMarkUtf8.Length);
            }

            return xml;
        }
    }
}
