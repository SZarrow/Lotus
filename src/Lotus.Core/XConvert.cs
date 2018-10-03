using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Lotus.Core
{
    /// <summary>
    /// 超级转换类。
    /// Code copied from DotNetWheels.Core.
    /// </summary>
    public static class XConvert
    {
        private readonly static Dictionary<String, MethodInfo> _tryParseMethodCache = new Dictionary<String, MethodInfo>(15);

        public static ObjectResult TryParse(Type targetType, Object value, Object defaultValue)
        {
            return TryParseCore(targetType, value, defaultValue);
        }

        public static T TryParse<T>(Object value, T defaultValue = default(T))
        {
            var result = TryParseCore(typeof(T), value, defaultValue);
            return result.Exceptions.Count > 0 ? defaultValue : ((result.Value is T) ? (T)result.Value : defaultValue);
        }

        private static ObjectResult TryParseCore(Type targetType, Object value, Object defaultValue = null)
        {
            Type returnType = targetType;

            if (targetType.Name == "Nullable`1")
            {
                targetType = targetType.GenericTypeArguments[0];
            }

            if (targetType.IsEnum && value != null)
            {
                String strValue = value.ToString();
                if (Regex.IsMatch(strValue, @"^\d+$", RegexOptions.IgnoreCase))
                {
                    Int32 intValue;
                    if (Int32.TryParse(strValue, out intValue))
                    {
                        try
                        {
                            var obj = Enum.ToObject(targetType, intValue);
                            if (obj.GetType() == targetType)
                            {
                                return new ObjectResult(obj);
                            }
                        }
                        catch (Exception) { }
                    }
                }
                else
                {
                    var enumValues = Enum.GetValues(targetType);
                    for (var i = 0; i < enumValues.Length; i++)
                    {
                        var enumValue = enumValues.GetValue(i);
                        if (String.Compare(enumValue.ToString(), strValue, true) == 0)
                        {
                            return new ObjectResult(enumValue);
                        }
                    }
                    return new ObjectResult(null, new InvalidCastException($"无法将{strValue}转换成{targetType.ToString()}"));
                }
            }

            if (!_tryParseMethodCache.ContainsKey(targetType.FullName))
            {
                var tryParseMethodInfo = targetType.GetMethod("TryParse", new Type[] { typeof(String), targetType.MakeByRefType() });

                if (tryParseMethodInfo != null)
                {
                    _tryParseMethodCache[targetType.FullName] = tryParseMethodInfo;
                }
            }

            if (_tryParseMethodCache.ContainsKey(targetType.FullName))
            {
                var methodInfo = _tryParseMethodCache[targetType.FullName];
                if (methodInfo != null)
                {
                    Object[] args = new Object[] { value != null ? value.ToString() : null, null };

                    Tuple<Object, Object> tuple;
                    try
                    {
                        tuple = methodInfo.XInvoke(null, args) as Tuple<Object, Object>;
                        if ((Boolean)tuple.Item1)
                        {
                            return new ObjectResult(tuple.Item2);
                        }
                    }
                    catch (Exception ex)
                    {
                        return new ObjectResult(null, ex);
                    }

                    Object dv = null;
                    if (targetType.IsValueType && targetType.Name != "Nullable`1")
                    {
                        if (targetType == typeof(Boolean))
                        {
                            dv = false;
                        }
                        else
                        {
                            if (defaultValue != null)
                            {
                                dv = defaultValue;
                            }
                            else
                            {
                                if (returnType.Name == "Nullable`1")
                                {
                                    dv = null;
                                }
                                else
                                {
                                    dv = 0;
                                }
                            }
                        }
                    }

                    return new ObjectResult(dv);
                }
            }

            if (targetType == typeof(String))
            {
                return new ObjectResult(value == null ? defaultValue : value.ToString());
            }

            return new ObjectResult(value == null ? defaultValue : value);
        }
    }
}
