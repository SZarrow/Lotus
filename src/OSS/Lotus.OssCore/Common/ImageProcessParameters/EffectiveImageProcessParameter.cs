using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lotus.OssCore.Common.ImageProcessParameters
{
    /// <summary>
    /// 图片效果参数类
    /// </summary>
    public class EffectiveImageProcessParameter : ImageProcessParameter
    {
        private const String OP_BLUR = "blur";
        private const String OP_BRIGHT = "bright";
        private const String OP_CONTRAST = "contrast";
        private const String OP_SHARPEN = "sharpen";

        /// <summary>
        /// 设置图片的模糊半径。示例：/blur,r_10
        /// </summary>
        /// <param name="r">图片的模糊半径，r 越大图片越模糊，取值范围[1-50]</param>
        public EffectiveImageProcessParameter SetBlurRadius(Int32 r)
        {
            AddParameter(OP_BLUR, "r_" + r.ToString());
            return this;
        }

        /// <summary>
        /// 设置正态分布的标准差。示例：/blur,s_10
        /// </summary>
        /// <param name="s">正态分布的标准差，s 越大图片越模糊，取值范围[1-50]</param>
        public EffectiveImageProcessParameter SetBlurStandardDeviation(Int32 s)
        {
            AddParameter(OP_BLUR, "s_" + s.ToString());
            return this;
        }

        /// <summary>
        /// 对图片进行亮度调节。示例：/bright,5
        /// </summary>
        /// <param name="value">亮度值，0 表示原图亮度，小于 0 表示低于原图亮度，大于 0 表示高于原图亮度。取值范围[-100, 100]</param>
        public EffectiveImageProcessParameter SetBright(Int32 value)
        {
            AddParameter(OP_BRIGHT, value.ToString());
            return this;
        }

        /// <summary>
        /// 设置图片的对比度。示例：/contrast,1
        /// </summary>
        /// <param name="value">对比度值，0 表示原图对比度，小于 0 表示低于原图对比度，大于 0 表示高于原图对比度。取值范围[-100, 100]</param>
        public EffectiveImageProcessParameter SetContrast(Int32 value)
        {
            AddParameter(OP_CONTRAST, value.ToString());
            return this;
        }

        /// <summary>
        /// 对图片进行锐化，使图片变得清晰。示例：/sharpen,100
        /// </summary>
        /// <param name="value">锐化值，参数越大，越清晰。取值范围[50, 399]，为达到较优效果，推荐取值为 100。</param>
        public EffectiveImageProcessParameter SetSharpen(Int32 value)
        {
            AddParameter(OP_SHARPEN, value.ToString());
            return this;
        }

    }
}
