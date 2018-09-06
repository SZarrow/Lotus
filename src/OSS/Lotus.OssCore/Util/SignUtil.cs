using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Lotus.Core;

namespace Lotus.OssCore.Util
{
    /// <summary>
    /// 
    /// </summary>
    public static class SignUtil
    {
        /// <summary>
        /// 计算指定输入字符串的MD5值。
        /// </summary>
        /// <param name="input">输入的字符串</param>
        public static String GetMD5(String input)
        {
            if (String.IsNullOrEmpty(input))
            {
                return null;
            }

            Byte[] data = null;
            Byte[] result = null;
            MD5 md5Hasher = null;

            try
            {
                data = Encoding.UTF8.GetBytes(input);
                md5Hasher = MD5.Create();
                result = md5Hasher.ComputeHash(data);
            }
            catch
            {
                return null;
            }
            finally
            {
                if (md5Hasher != null)
                {
                    md5Hasher.Dispose();
                }
            }

            if (result == null || result.Length == 0)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();
            foreach (var b in result)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 计算指定输入字符串的SHA1值。
        /// </summary>
        /// <param name="input">输入的字符串</param>
        public static String GetSHA1(String input)
        {
            if (String.IsNullOrEmpty(input))
            {
                return null;
            }

            Byte[] data = null;
            Byte[] result = null;
            HashAlgorithm sh1csp = null;

            try
            {
                data = Encoding.UTF8.GetBytes(input);
                sh1csp = new SHA1Managed();
                result = sh1csp.ComputeHash(data);
            }
            catch
            {
                return null;
            }
            finally
            {
                if (sh1csp != null)
                {
                    sh1csp.Dispose();
                }
            }

            if (result == null || result.Length == 0)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();
            foreach (var b in result)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Computes the HMAC_SHA1 value of input with key and return base64 string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="key">The key that HMAC_SHA1 Algorithm used.</param>
        /// <returns>Returns base64 string.</returns>
        public static XResult<String> HMACSHA1Base64String(String input, String key)
        {
            if (String.IsNullOrWhiteSpace(input))
            {
                return new XResult<String>(null, new ArgumentNullException("input"));
            }

            if (String.IsNullOrWhiteSpace(key))
            {
                return new XResult<String>(null, new ArgumentNullException("key"));
            }

            Byte[] keyData = null;
            try
            {
                keyData = Encoding.UTF8.GetBytes(key);
            }
            catch (Exception ex)
            {
                return new XResult<String>(null, ex);
            }

            Byte[] inputData = null;
            try
            {
                inputData = Encoding.UTF8.GetBytes(input);
            }
            catch (Exception ex)
            {
                return new XResult<String>(null, ex);
            }

            HMACSHA1 hmac = new HMACSHA1(keyData);
            Byte[] result = null;

            try
            {
                result = hmac.ComputeHash(inputData);
            }
            catch (Exception ex)
            {
                return new XResult<String>(null, ex);
            }

            if (result == null || result.Length == 0)
            {
                return new XResult<String>(null, new ArgumentNullException("result = hmac.ComputeHash(inputData);"));
            }

            return new XResult<String>(Convert.ToBase64String(result));
        }
    }
}
