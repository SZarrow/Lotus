using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lotus.OssCore.Common
{
    /// <summary>
    /// 表示Url安全的Base64编码字符串
    /// </summary>
    [Serializable]
    public class UrlSafeBase64String
    {
        private String _base64String;

        /// <summary>
        /// 使用一个字符串实例化UrlSafeBase64String类。
        /// </summary>
        /// <param name="value"></param>
        public UrlSafeBase64String(String value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException("base64String");
            }

            try
            {
                _base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
            }
            catch { }
        }

        /// <summary>
        /// 获取Url安全的Base64编码字符串。
        /// </summary>
        public override String ToString()
        {
            if (String.IsNullOrWhiteSpace(_base64String))
            {
                return null;
            }
            else
            {
                return _base64String.Replace('+', '-').Replace('/', '_');
            }
        }
    }
}
