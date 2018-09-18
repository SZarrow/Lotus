using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using DotNetWheels.Core;

namespace Lotus.Serialization
{
    public class XSerializer
    {
        public String Serialize(Object value, Action<XDocument> configDoc = null)
        {
            var doc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"));
            var eleStack = new Stack<XElement>();
            var typeStack = new Stack<Type>();

            var instanceType = value.GetType();
            typeStack.Push(instanceType);

            while (instanceType.BaseType != null
                && instanceType.BaseType != typeof(Object))
            {
                instanceType = instanceType.BaseType;
                typeStack.Push(instanceType);
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

            if (eleStack.Count == 1)
            {
                rootEl = eleStack.Pop();
            }

            doc.Add(rootEl);

            if (configDoc != null)
            {
                configDoc(doc);
            }

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
                var parentEl = stack.Count > 0 ? stack.Peek() : null;
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
                        Boolean hasChildrenXElements = (from t0 in insProp.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                        where t0.GetCustomAttribute<XElementAttribute>() != null
                                                        select t0).Count() > 0;

                        if (propertyValue != null)
                        {
                            if (hasChildrenXElements)
                            {
                                BuildDocTree(propertyValue, stack, insProp.PropertyType);
                            }
                            else
                            {
                                insPropEl.Value = propertyValue.ToString();
                            }
                        }

                        if (parentEl != null && !hasChildrenXElements)
                        {
                            parentEl.Add(insPropEl);
                        }
                    }
                }
            }
        }

        public T Deserialize<T>(String xml)
        {
            if (String.IsNullOrWhiteSpace(xml))
            {
                return default(T);
            }

            xml = RemoveUTF8MarkChar(xml);

            XDocument doc = null;
            try
            {
                doc = XDocument.Parse(xml, LoadOptions.None);
            }
            catch (Exception)
            {
                return default(T);
            }

            if (doc == null)
            {
                return default(T);
            }

            var targetType = typeof(T);
            T instance = default(T);
            try
            {
                instance = Activator.CreateInstance<T>();
            }
            catch (Exception)
            {
                return default(T);
            }

            Fill(instance, doc.Root);

            return instance;
        }

        private void Fill<T>(T value, XElement root)
        {
            var valueType = typeof(T);
            var xelProperties = from p in valueType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty)
                                let xelAttr = p.GetCustomAttribute<XElementAttribute>()
                                where xelAttr != null
                                select new { XName = xelAttr.ElementName, Property = p };

            var xels = root.Elements();
            foreach (var xel in xels)
            {
                var xelProp = xelProperties.FirstOrDefault(x => x.XName == xel.Name.LocalName);
                if (xelProp != null)
                {
                    SetProperty(value, xelProp.Property, xel);
                }
            }

        }

        private void SetProperty(Object instance, PropertyInfo property, XElement xel)
        {
            if (xel.HasElements)
            {
                var newIns = Activator.CreateInstance(property.PropertyType);
                var properties = from p in property.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty)
                                 let xelAttr = p.GetCustomAttribute<XElementAttribute>()
                                 where xelAttr != null
                                 select new { XName = xelAttr.ElementName, Property = p };

                foreach (var xelChild in xel.Elements())
                {
                    var insProp = properties.FirstOrDefault(x => x.XName == xelChild.Name.LocalName);
                    if (insProp != null)
                    {
                        SetProperty(newIns, insProp.Property, xelChild);
                    }
                }

                property.XSetValue(instance, newIns);
            }
            else
            {
                var propertyValueResult = XConvert.TryParse(property.PropertyType, xel.Value, null);
                if (propertyValueResult.Success)
                {
                    property.XSetValue(instance, propertyValueResult.Value);
                }
            }
        }

        /// <summary>
        /// 移除UTF8编码的标记字符
        /// </summary>
        /// <param name="xml">要移除UTF8编码标记字符的Xml字符串</param>
        public static String RemoveUTF8MarkChar(String xml)
        {
            String byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            if (xml.StartsWith(byteOrderMarkUtf8) && xml[0] != '<')
            {
                xml = xml.Remove(0, byteOrderMarkUtf8.Length);
            }

            return xml;
        }
    }
}
