using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lotus.OssCore.Domain
{
    /// <summary>
    /// 表示图片信息
    /// </summary>
    [Serializable]
    public class XImageInfo
    {
        public XImageInfo(Int32 width, Int32 height, String format, Int32 fileSize)
        {
            this.Width = width;
            this.Height = height;
            this.Format = format;
            this.FileSize = fileSize;
        }

        /// <summary>
        /// 获取图片大小
        /// </summary>
        public Int32 FileSize { get; private set; }
        /// <summary>
        /// 获取图片的格式
        /// </summary>
        public String Format { get; private set; }
        /// <summary>
        /// 获取图片的宽度
        /// </summary>
        public Int32 Width { get; private set; }
        /// <summary>
        /// 获取图片的高度
        /// </summary>
        public Int32 Height { get; private set; }
    }
}
