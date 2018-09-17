using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using DotNetWheels.Core;

namespace Lotus.Serialization
{
    public class XSerializer
    {
        public String Serialize(Object value)
        {
            var doc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"));
            var eleStack = new Stack<XElement>();
            var typeStack = new Stack<Type>();

            var instanceType = value.GetType();
            typeStack.Push(instanceType);
            //创建当前实例的属性节点

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

            SetValue(targetType, instance, doc.Root);
            return instance;
        }

        private void SetValue<T>(Type targetType, T instance, XElement el)
        {
            var ps = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
            var propertyInfo = (from p in targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty)
                                let cusAttr = p.GetCustomAttribute<XElementAttribute>()
                                where cusAttr != null && cusAttr.ElementName == el.Name.LocalName
                                select p).FirstOrDefault();

            if (propertyInfo != null)
            {
                var propertyValueResult = XConvert.TryParse(propertyInfo.PropertyType, el.Value, null);
                if (propertyValueResult.Success)
                {
                    propertyInfo.XSetValue(instance, propertyValueResult.Value);
                }
            }

            if (el.HasElements)
            {
                foreach (var childEl in el.Elements())
                {
                    SetValue(targetType, instance, childEl);
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
