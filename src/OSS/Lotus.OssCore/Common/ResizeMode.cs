using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lotus.OssCore.Common
{
    /// <summary>
    /// 缩放模式
    /// </summary>
    public enum ResizeMode
    {
        /// <summary>
        /// 等比缩放，限制在设定在指定w与h的矩形内的最大图片。
        /// </summary>
        LFit,
        /// <summary>
        /// 等比缩放，延伸出指定w与h的矩形框外的最小图片。
        /// </summary>
        MFit,
        /// <summary>
        /// 固定宽高，将延伸出指定w与h的矩形框外的最小图片进行居中裁剪。
        /// </summary>
        Fill,
        /// <summary>
        /// 固定宽高，缩略填充。
        /// </summary>
        Pad,
        /// <summary>
        /// 固定宽高，强制缩略。
        /// </summary>
        Fixed
    }
}
