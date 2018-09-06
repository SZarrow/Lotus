using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lotus.OssCore.Common.ImageProcessParameters
{
    /// <summary>
    /// 图片格式处理参数类
    /// </summary>
    public class FormatImageProcessParameter : ImageProcessParameter
    {
        private const String OP_FORMAT = "format";
        private const String OP_INTERLACE = "interlace";
        private const String OP_QUALITY = "quality";

        /// <summary>
        /// 将图片转换成指定格式。示例：/format,jpg
        /// </summary>
        /// <param name="format">要转换成的图片格式</param>
        public FormatImageProcessParameter SetFormat(SupportedImageFormat format)
        {
            AddParameter(OP_FORMAT, format.ToString().ToLowerInvariant());
            return this;
        }

        /// <summary>
        /// 指定是否将jpg图片的呈现方式设置为自上而下的扫描式。示例：/interlace,1
        /// </summary>
        /// <param name="value">如果为True，则设置为自上而下的扫描式；否则为先模糊后逐渐清晰。</param>
        public FormatImageProcessParameter SetInterlace(Boolean value)
        {
            AddParameter(OP_INTERLACE, value ? "1" : "0");
            return this;
        }

        /// <summary>
        /// 设置图片的相对质量，仅支持jpg或webp格式。示例：/quality,q_100
        /// </summary>
        /// <param name="q">图片的相对质量，对原图按照 q% 进行质量压缩。如果原图质量是 100%，使用 90q 会得到质量为 90％ 的图片；如果原图质量是 80%，使用 90q 会得到质量72%的图片。只能在原图是 jpg 格式的图片上使用，才有相对压缩的概念。如果原图为 webp，那么相对质量就相当于绝对质量。取值范围[1-100]</param>
        public FormatImageProcessParameter SetRelativeQuality(Int32 q)
        {
            AddParameter(OP_QUALITY, "q_" + q.ToString());
            return this;
        }

        /// <summary>
        /// 设置图片的绝对质量，仅支持jpg或webp格式。示例：/quality,Q_100
        /// </summary>
        /// <param name="Q">图片的绝对质量，把原图质量压到Q%，如果原图质量小于指定数字，则不压缩。如果原图质量是100%，使用”90Q”会得到质量90％的图片；如果原图质量是95%，使用“90Q”还会得到质量90%的图片；如果原图质量是80%，使用“90Q”不会压缩，返回质量80%的原图。只能在保存格式为jpg/webp效果上使用，其他格式无效果。 如果同时指定了q和Q，按Q来处理。取值范围[1-100]</param>
        public FormatImageProcessParameter SetAbsoluteQuality(Int32 Q)
        {
            AddParameter(OP_QUALITY, "Q_" + Q.ToString());
            return this;
        }
    }
}
