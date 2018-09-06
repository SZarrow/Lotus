using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lotus.OssCore.Common
{
    /// <summary>
    /// 图片处理参数基类
    /// </summary>
    public abstract class ImageProcessParameter
    {
        private Dictionary<String, List<String>> _parameters;

        /// <summary>
        /// 
        /// </summary>
        protected ImageProcessParameter()
        {
            _parameters = new Dictionary<String, List<String>>(5);
        }

        /// <summary>
        /// 添加参数到指定的图片处理函数
        /// </summary>
        /// <param name="operateName">图片处理函数的名称</param>
        /// <param name="parameter">图片处理的参数</param>
        protected void AddParameter(String operateName, String parameter)
        {
            if (!String.IsNullOrWhiteSpace(operateName) &&
                !String.IsNullOrWhiteSpace(parameter))
            {
                if (!_parameters.ContainsKey(operateName))
                {
                    _parameters[operateName] = new List<String>(10);
                }

                List<String> args;
                if (_parameters.TryGetValue(operateName, out args))
                {
                    if (!args.Contains(parameter))
                    {
                        _parameters[operateName].Add(parameter);
                    }
                }
            }
        }

        /// <summary>
        /// 获取图片处理参数值
        /// </summary>
        public virtual String GetProcessArguments()
        {
            if (_parameters.Count > 0)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var p in _parameters)
                {
                    sb.Append("/")
                        .Append(p.Key + ",")
                        .Append(String.Join(",", p.Value));
                }

                return sb.ToString();
            }

            return String.Empty;
        }
    }
}
