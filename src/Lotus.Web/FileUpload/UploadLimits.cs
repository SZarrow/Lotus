using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lotus.Web.FileUpload
{
    /// <summary>
    /// 上传限制条件
    /// </summary>
    [Serializable]
    public class UploadLimits
    {
        private Int32 _maxFileCount;

        /// <summary>
        /// The limits of upload files.
        /// </summary>
        public UploadLimits()
        {
            _maxFileCount = 1;
            this.SingleFileMaxSize = 2097152;
        }

        /// <summary>
        /// 允许覆盖
        /// </summary>
        public Boolean AllowOverride { get; set; }

        /// <summary>
        /// 单个文件的最大上传大小，单位KB
        /// </summary>
        public Int64 SingleFileMaxSize { get; set; }

        /// <summary>
        /// 允许的文件的contentType类型，多个属性用"|"隔开。
        /// </summary>
        public String AllowedFileTypes { get; set; }

        /// <summary>
        /// 单次上传允许的最大文件个数
        /// </summary>
        public Int32 MaxFileCount
        {
            get { return _maxFileCount; }
            set
            {
                if (value != _maxFileCount && value > 0)
                {
                    _maxFileCount = value;
                }
            }
        }
    }
}
