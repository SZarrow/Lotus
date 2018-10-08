using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Lotus.Core;

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
            //序列化当前对象
            var insCusAttr = nodeType.GetCustomAttribute<XElementAttribute>(false);
            if (insCusAttr != null)
            {
                var xel = CreateXElement(insCusAttr, stack.Count > 0 ? stack.Peek() : null);
                stack.Push(xel);
            }

            //获取当前对象的属性列表
            var insProperties = nodeType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.DeclaredOnly);
            if (insProperties == null || insProperties.Length == 0)
            {
                return;
            }

            //取元素栈中的最近一个元素作为当前对象的父元素
            var parentEl = stack.Count > 0 ? stack.Peek() : null;

            foreach (var insProp in insProperties)
            {
                if (nodeValue == null)
                {
                    continue;
                }

                Object propertyValue = insProp.XGetValue(nodeValue);

                var insPropXElAttr = insProp.GetCustomAttribute<XElementAttribute>();
                if (insPropXElAttr != null)
                {
                    var hasChildrenProperties = (from p in insProp.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                 where p.GetCustomAttribute<XElementAttribute>() != null
                                                 || p.GetCustomAttribute<XCollectionAttribute>() != null
                                                 select p).Count() > 0;

                    //如果当前属性没有子属性，则直接将属性值赋值给当前属性
                    //然后将当前属性对应的元素添加到父元素
                    if (!hasChildrenProperties)
                    {
                        var insPropEl = CreateXElement(insPropXElAttr, parentEl);
                        insPropEl.Value = (propertyValue ?? String.Empty).ToString();

                        if (parentEl != null)
                        {
                            parentEl.Add(insPropEl);
                        }
                    }
                    else
                    {
                        //如果当前属性有子属性，则递归构建子属性的结构
                        BuildDocTree(insProp.PropertyType, propertyValue, stack);
                    }

                    //如果当前属性不是集合，则短路下面的操作
                    continue;
                }

                //如果父元素是集合
                var insPropXCoAttr = insProp.GetCustomAttribute<XCollectionAttribute>();
                if (insPropXCoAttr != null)
                {
                    var collection = propertyValue as IEnumerable;
                    if (collection != null)
                    {
                        var itemStack = new Stack<XElement>();

                        //这个地方添加一个parentEl是为了在遍历子元素时
                        //让子元素获取到父元素的命名空间
                        itemStack.Push(parentEl);

                        foreach (var item in collection)
                        {
                            BuildDocTree(item.GetType(), item, itemStack);
                        }

                        //这个地方要排除上面添加的parentEL
                        //只添加集合中的子元素
                        while (itemStack.Count > 1)
                        {
                            parentEl.Add(itemStack.Pop());
                        }

                        itemStack.Clear();
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

        private void Fill(Object value, XElement root)
        {
            var valueType = value.GetType();
            var xelProperties = from p in valueType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty)
                                let xelAttr = p.GetCustomAttribute<XElementAttribute>()
                                let xcoAttr = p.GetCustomAttribute<XCollectionAttribute>()
                                where xelAttr != null || xcoAttr != null
                                select new
                                {
                                    XElementAttribute = xelAttr,
                                    XCollectionAttribute = xcoAttr,
                                    PropertyInfo = p
                                };

            foreach (var xelProp in xelProperties)
            {
                if (xelProp.XElementAttribute != null)
                {
                    var xel = root.Elements().FirstOrDefault(x => x.Name.LocalName == xelProp.XElementAttribute.ElementName);
                    if (xel != null)
                    {
                        SetProperty(value, xelProp.PropertyInfo, xel);
                    }
                }
                else if (xelProp.XCollectionAttribute != null)
                {

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
                                 let xcoAttr = p.GetCustomAttribute<XCollectionAttribute>()
                                 where xelAttr != null || xcoAttr != null
                                 select new { ElementName = xelAttr != null ? xelAttr.ElementName : String.Empty, IsCollection = (xcoAttr != null), Property = p };

                foreach (var childProp in properties)
                {
                    if (childProp.IsCollection)
                    {
                        var genericTypes = childProp.Property.PropertyType.GenericTypeArguments;
                        var listType = typeof(List<>).MakeGenericType(genericTypes);
                        var list = listType.GetConstructor(Type.EmptyTypes).XConstruct(null) as IList;

                        foreach (var el in xel.Elements())
                        {
                            var listItem = Activator.CreateInstance(genericTypes[0]);
                            Fill(listItem, el);
                            list.Add(listItem);
                        }

                        childProp.Property.XSetValue(newIns, list);
                    }
                    else
                    {
                        var xelChild = xel.Elements().FirstOrDefault(x => x.Name.LocalName == childProp.ElementName);
                        if (xelChild != null)
                        {
                            SetProperty(newIns, childProp.Property, xelChild);
                        }
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

        private XElement CreateXElement(XElementAttribute elementAttribute, XElement parent)
        {
            var element = new XElement(elementAttribute.ElementName);

            //如果当前节点自己有命名空间，则添加自己的命名空间
            if (!String.IsNullOrWhiteSpace(elementAttribute.Namespace))
            {
                element.Name = XName.Get(element.Name.LocalName, elementAttribute.Namespace);
            }
            else
            {
                //如果当前节点自己没有命名空间，但是父节点有命名空间，
                //则将使用父节点的命名作为自己的命名空间
                if (parent != null)
                {
                    String parentNamespace = parent.Name.NamespaceName;
                    if (!String.IsNullOrWhiteSpace(parentNamespace))
                    {
                        element.Name = XName.Get(element.Name.LocalName, parentNamespace);
                    }
                }
            }

            return element;
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
