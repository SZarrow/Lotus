using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lotus.OssCore.Common.ImageProcessParameters
{
    /// <summary>
    /// 缩放图片参数类，将图片按照要求生成缩略图，或者进行特定的缩放。
    /// </summary>
    public class ResizeImageProcessParameter : ImageProcessParameter
    {
        private const String OP_RESIZE = "resize";
        private const Int32 LengthLimited = 4096;

        /// <summary>
        /// 指定缩放后的宽度，取值范围[1-4096]。示例：/resize,w_100
        /// </summary>
        /// <param name="w"></param>
        public ResizeImageProcessParameter SetWidth(Int32 w)
        {
            Contract.Requires(w >= 1 && w <= LengthLimited);

            AddParameter(OP_RESIZE, "w_" + w.ToString());
            return this;
        }

        /// <summary>
        /// 指定缩放后的高度，取值范围[1-4096]。示例：/resize,h_100
        /// </summary>
        /// <param name="h"></param>
        public ResizeImageProcessParameter SetResizeHeight(Int32 h)
        {
            Contract.Requires(h >= 1 && h <= LengthLimited);

            AddParameter(OP_RESIZE, "h_" + h.ToString());
            return this;
        }

        /// <summary>
        /// 指定缩略的模式，默认为LFit模式，即按长边缩放。示例：/resize,m_lfit
        /// </summary>
        /// <param name="m"></param>
        public ResizeImageProcessParameter SetResizeMode(ResizeMode m)
        {
            AddParameter(OP_RESIZE, "m_" + m.ToString().ToLowerInvariant());
            return this;
        }

        /// <summary>
        /// 指定缩略图的最长边，取值范围[1-4096]。示例：/resize,l_100
        /// </summary>
        /// <param name="l">缩略图的最长边长度</param>
        public ResizeImageProcessParameter SetResizeLongEdges(Int32 l)
        {
            Contract.Requires(l >= 1 && l <= LengthLimited);

            AddParameter(OP_RESIZE, "l_" + l.ToString());
            return this;
        }

        /// <summary>
        /// 指定缩略图的最短边，取值范围[1-4096]。示例：/resize,s_100
        /// </summary>
        /// <param name="s">缩略图的最短边长度</param>
        public ResizeImageProcessParameter SetResizeShortEdges(Int32 s)
        {
            Contract.Requires(s >= 1 && s <= LengthLimited);

            AddParameter(OP_RESIZE, "s_" + s.ToString());
            return this;
        }

        /// <summary>
        /// 指定当目标缩略图大于原图时是否处理。默认为True。
        /// </summary>
        /// <param name="limit">True 表示不处理；False 表示处理。</param>
        public ResizeImageProcessParameter SetResizeLimit(Boolean limit)
        {
            AddParameter(OP_RESIZE, "limit_" + (limit ? "1" : "0"));
            return this;
        }

        /// <summary>
        /// 当缩放模式选择为Pad（缩略填充）时，可以选择填充的颜色，默认是白色。/resize,color_ff0000
        /// </summary>
        /// <param name="color">采用16进制颜色码表示，如00FF00（绿色），取值范围[000000-FFFFFF]。</param>
        public ResizeImageProcessParameter SetFillColor(String color)
        {
            if (Regex.IsMatch(color, "^[a-zA-Z0-9]{6}$"))
            {
                AddParameter(OP_RESIZE, "color_" + color.ToLowerInvariant());
            }

            return this;
        }

        /// <summary>
        /// 按比例缩放。示例：/resize,p_120
        /// </summary>
        /// <param name="p">倍数百分比。 小于100，即是缩小，大于100即是放大，取值范围[1-1000]。</param>
        public ResizeImageProcessParameter SetResizePercent(Int32 p)
        {
            AddParameter(OP_RESIZE, "p_" + p.ToString());
            return this;
        }
    }
}
