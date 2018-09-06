using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lotus.OssCore.Common.ImageProcessParameters
{
    /// <summary>
    /// 图片水印参数类
    /// </summary>
    public class WaterMarkImageProcessParameter : ImageProcessParameter
    {
        private const String OP_WATERMARK = "watermark";

        /// <summary>
        /// 设置水印的透明度。示例：/watermark,t_20
        /// </summary>
        /// <param name="t">透明度, 如果是图片水印，就是让图片变得透明，如果是文字水印，就是让水印变透明 默认值：100， 表示 100%（不透明） 取值范围: [0-100]</param>
        public WaterMarkImageProcessParameter SetTransparency(Int32 t)
        {
            if (t >= 0 && t <= 100)
            {
                AddParameter(OP_WATERMARK, "t_" + t.ToString());
            }

            return this;
        }

        /// <summary>
        /// 设置水印的位置。示例：/watermark,g_ne
        /// </summary>
        /// <param name="g">水印打在图的位置，采用九宫格方位</param>
        public WaterMarkImageProcessParameter SetPosition(GridPosition g)
        {
            AddParameter(OP_WATERMARK, "g_" + g.ToString().ToLowerInvariant());
            return this;
        }

        /// <summary>
        /// 设置距离图片边缘的水平距离， 这个参数只有当水印位置是左上，左中，左下， 右上，右中，右下才有意义。示例：/watermark,x_10
        /// </summary>
        /// <param name="x">水平距离，默认值：10，取值范围：[0 – 4096]，单位：像素（px）</param>
        public WaterMarkImageProcessParameter SetX(Int32 x)
        {
            if (x >= 0 && x <= 4096)
            {
                AddParameter(OP_WATERMARK, "x_" + x.ToString());
            }

            return this;
        }

        /// <summary>
        /// 设置距离图片边缘的垂直距离， 这个参数只有当水印位置是左上，中上， 右上，左下，中下，右下才有意义。示例：/watermark,y_10
        /// </summary>
        /// <param name="y">垂直距离，默认值：10 取值范围：[0 – 4096]，单位：像素(px)</param>
        public WaterMarkImageProcessParameter SetY(Int32 y)
        {
            if (y >= 0 && y <= 4096)
            {
                AddParameter(OP_WATERMARK, "y_" + y.ToString());
            }

            return this;
        }

        /// <summary>
        /// 设置中线垂直偏移，当水印位置在左中，中部，右中时，可以指定水印位置根据中线往上或者往下偏移。示例：/watermark,voffset_0
        /// </summary>
        /// <param name="offset">中线垂直偏移，当水印位置在左中，中部，右中时，可以指定水印位置根据中线往上或者往下偏移，默认值：0，取值范围：[-1000, 1000]，单位：像素(px)</param>
        public WaterMarkImageProcessParameter SetVOffset(Int32 offset)
        {
            if (offset >= -1000 && offset <= 1000)
            {
                AddParameter(OP_WATERMARK, "voffset_" + offset.ToString());
            }

            return this;
        }

        /// <summary>
        /// 在图片上设置另外一张图片做为水印。示例：/watermark,image=cGFuZGEucG5n
        /// </summary>
        /// <param name="base64Value">图片对应的base64值</param>
        public WaterMarkImageProcessParameter SetImageMark(UrlSafeBase64String base64Value)
        {
            AddParameter(OP_WATERMARK, "image_" + base64Value.ToString());
            return this;
        }

        /// <summary>
        /// 设置文字水印的文本内容。示例：/watermark,text_d3F5LXplbmhlaQ==
        /// </summary>
        /// <param name="base64Value">水印文本内容，base64值。</param>
        public WaterMarkImageProcessParameter SetFontText(UrlSafeBase64String base64Value)
        {
            AddParameter(OP_WATERMARK, "text_" + base64Value.ToString());

            return this;
        }

        /// <summary>
        /// 设置文字水印的文字类型（就是字体的英文名称）。示例：/watermark,type_ZmFuZ3poZW5nZmFuZ3Nvbmc=
        /// </summary>
        /// <param name="base64Value">文字水印的文本类型，base64值。</param>
        public WaterMarkImageProcessParameter SetFontType(UrlSafeBase64String base64Value)
        {
            AddParameter(OP_WATERMARK, "type_" + base64Value.ToString());

            return this;
        }

        /// <summary>
        /// 设置文字水印文字的颜色。示例：/watermark,color_ff0000
        /// </summary>
        /// <param name="color">颜色值，6位十六进制数。</param>
        public WaterMarkImageProcessParameter SetFontColor(String color)
        {
            if (Regex.IsMatch(color, @"^[a-zA-Z0-9]{6}$"))
            {
                AddParameter(OP_WATERMARK, "color_" + color.ToLowerInvariant());
            }

            return this;
        }

        /// <summary>
        /// 设置文字水印文字大小。示例：/watermark,size=50
        /// </summary>
        /// <param name="size">文字大小，单位：px，取值范围：(0，1000]，默认值：40</param>
        public WaterMarkImageProcessParameter SetFontSize(Int32 size)
        {
            if (size > 0 && size <= 1000)
            {
                AddParameter(OP_WATERMARK, "size_" + size.ToString());
            }

            return this;
        }

        /// <summary>
        /// 设置文本阴影大小。/watermark,shadow_10
        /// </summary>
        /// <param name="value">文本阴影大小，取值范围(0,100]</param>
        public WaterMarkImageProcessParameter SetFontShadow(Int32 value)
        {
            if (value > 0 && value <= 100)
            {
                AddParameter(OP_WATERMARK, "shadow_" + value.ToString());
            }

            return this;
        }

        /// <summary>
        /// 设置文本旋转的角度。示例：/watermark,rotate_45
        /// </summary>
        /// <param name="angle">文本旋转的角度，取值范围(0,360]</param>
        public WaterMarkImageProcessParameter SetFontRotate(Int32 angle)
        {
            if (angle > 0 && angle <= 360)
            {
                AddParameter(OP_WATERMARK, "rotate_" + angle.ToString());
            }

            return this;
        }

        /// <summary>
        /// 设置是否进行水印铺满的效果。示例：/watermark,fill_1
        /// </summary>
        /// <param name="value">True表示铺满，False表示无效果</param>
        public WaterMarkImageProcessParameter SetFontFill(Boolean value)
        {
            AddParameter(OP_WATERMARK, "fill_" + (value ? "1" : "0"));

            return this;
        }

        /// <summary>
        /// 设置文字，图片水印前后顺序。示例：/watermark,order_0
        /// </summary>
        /// <param name="order">order = 0 图片在前(默认值)； order = 1 文字在前</param>
        public WaterMarkImageProcessParameter SetOrder(Int32 order)
        {
            if (order == 0 || order == 1)
            {
                AddParameter(OP_WATERMARK, "order_" + order.ToString());
            }

            return this;
        }

        /// <summary>
        /// 文字、图片对齐方式。示例：/watermark,align_1
        /// </summary>
        /// <param name="align">align = 0 上对齐(默认值) align = 1 中对齐 align = 2 下对齐</param>
        public WaterMarkImageProcessParameter SetAlign(Int32 align)
        {
            if (align == 0 || align == 1 || align == 2)
            {
                AddParameter(OP_WATERMARK, "align_" + align.ToString());
            }

            return this;
        }

        /// <summary>
        /// 设置文字和图片间的间距。示例：/watermark,interval_20
        /// </summary>
        /// <param name="interval">文字和图片间的间距，取值范围: [0, 1000]</param>
        public WaterMarkImageProcessParameter SetInterval(Int32 interval)
        {
            if (interval >= 0 && interval <= 1000)
            {
                AddParameter(OP_WATERMARK, "interval_" + interval.ToString());
            }

            return this;
        }
    }
}
