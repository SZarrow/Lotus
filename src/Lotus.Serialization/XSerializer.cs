using System;
using System.Collections;
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
                BuildDocTree(type, value, eleStack);
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
                else
                {
                    eleStack.Peek().Add(parentEl);
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

        private void BuildDocTree(Type nodeType, Object nodeValue, Stack<XElement> stack)
        {
            var insCusAttr = nodeType.GetCustomAttribute<XElementAttribute>(false);
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

            var insProperties = nodeType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.DeclaredOnly);
            if (insProperties != null && insProperties.Length > 0)
            {
                var parentEl = stack.Count > 0 ? stack.Peek() : null;
                foreach (var insProp in insProperties)
                {
                    Object propertyValue = insProp.XGetValue(nodeValue);

                    var insPropXElAttr = insProp.GetCustomAttribute<XElementAttribute>();
                    if (insPropXElAttr != null)
                    {
                        var insPropEl = new XElement(insPropXElAttr.ElementName);
                        if (!String.IsNullOrWhiteSpace(insPropXElAttr.Namespace))
                        {
                            insPropEl.Name = XName.Get(insPropXElAttr.ElementName, insPropXElAttr.Namespace);
                        }
                        else
                        {
                            String parentNamespace = stack.Count > 0 ? stack.Peek().Name.NamespaceName : null;
                            if (!String.IsNullOrWhiteSpace(parentNamespace))
                            {
                                insPropEl.Name = XName.Get(insPropXElAttr.ElementName, parentNamespace);
                            }
                        }

                        Boolean hasChildrenXElements = (from t0 in insProp.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                        where t0.GetCustomAttribute<XElementAttribute>() != null
                                                        || t0.GetCustomAttribute<XCollectionAttribute>() != null
                                                        select t0).Count() > 0;

                        if (propertyValue != null)
                        {
                            if (hasChildrenXElements)
                            {
                                BuildDocTree(insProp.PropertyType, propertyValue, stack);
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

                    //序列化集合
                    var insPropXCoAttr = insProp.GetCustomAttribute<XCollectionAttribute>();
                    if (insPropXCoAttr != null)
                    {
                        var collection = propertyValue as IEnumerable;
                        var newStack = new Stack<XElement>();

                        if (parentEl != null)
                        {
                            newStack.Push(parentEl);
                        }

                        foreach (var item in collection)
                        {
                            BuildDocTree(item.GetType(), item, newStack);
                        }

                        if (newStack.Count > 0 && parentEl != null)
                        {
                            while (newStack.Count > 0)
                            {
                                parentEl.Add(newStack.Pop());
                            }
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
                                let xcoAttr = p.GetCustomAttribute<XCollectionAttribute>()
                                where xelAttr != null || xcoAttr != null
                                select new Tuple<XElementAttribute, XCollectionAttribute, PropertyInfo>(xelAttr, xcoAttr, p);

            var xels = root.Elements();
            foreach (var xel in xels)
            {
                if (xel.HasElements)
                {

                }
                else
                {
                    var xelProp = xelProperties.FirstOrDefault(x => x.Item1 != null && x.Item1.ElementName == xel.Name.LocalName);
                    if (xelProp != null)
                    {
                        SetProperty(value, xelProp.Item3, xel);
                    }
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
