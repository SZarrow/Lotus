using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lotus.OssCore.Common.ImageProcessParameters
{
    /// <summary>
    /// 裁剪图片参数类
    /// </summary>
    public class CropImageProcessParameter : ImageProcessParameter
    {
        private const String OP_CROP = "crop";
        private const String OP_CIRCLE = "circle";
        private const String OP_INDEXCROP = "indexcrop";
        private const String OP_ROUNDEDCORNERS = "rounded-corners";

        /// <summary>
        /// 指定裁剪宽度。示例：/crop,w_100
        /// </summary>
        /// <param name="w">裁剪宽度，取值范围[0-图片宽度]</param>
        public CropImageProcessParameter SetCropWidth(Int32 w)
        {
            AddParameter(OP_CROP, "w_" + w.ToString());
            return this;
        }

        /// <summary>
        /// 指定裁剪高度。示例：/crop,h_100
        /// </summary>
        /// <param name="h">裁剪高度，取值范围[0-图片高度]</param>
        public CropImageProcessParameter SetCropHeight(Int32 h)
        {
            AddParameter(OP_CROP, "h_" + h.ToString());
            return this;
        }

        /// <summary>
        /// 指定裁剪起点横坐标（默认左上角为原点）。示例：/crop,x_20
        /// </summary>
        /// <param name="x">裁剪起点横坐标（默认左上角为原点），[0-图片边界]</param>
        public CropImageProcessParameter SetCropX(Int32 x)
        {
            AddParameter(OP_CROP, "x_" + x.ToString());
            return this;
        }

        /// <summary>
        /// 指定裁剪起点纵坐标（默认左上角为原点）。示例：/crop,y_20
        /// </summary>
        /// <param name="y"></param>
        public CropImageProcessParameter SetCropY(Int32 y)
        {
            AddParameter(OP_CROP, "y_" + y.ToString());
            return this;
        }

        /// <summary>
        /// 设置裁剪的原点位置，由九宫格的格式，一共有九个地方可以设置，每个位置位于每个九宫格的左上角。示例：/crop,g_center
        /// </summary>
        /// <param name="g">九宫格方位</param>
        public CropImageProcessParameter SetCropGridPosition(GridPosition g)
        {
            AddParameter(OP_CROP, "g_" + g.ToString().ToLowerInvariant());
            return this;
        }

        /// <summary>
        /// 指定做内切圆时的圆形区域的半径。示例：/circle,r_50
        /// </summary>
        /// <param name="r">内切圆时的圆形区域的半径，半径 r 不能超过原图的最小边的一半。如果超过，则圆的大小仍然是原圆的最大内切圆。</param>
        public CropImageProcessParameter SetCircleRadius(Int32 r)
        {
            AddParameter(OP_CIRCLE, "r_" + r.ToString());
            return this;
        }

        /// <summary>
        /// 进行水平切割，每块图片的长度。x 参数与 y 参数只能任选其一。示例：/indexcrop,x_10
        /// </summary>
        /// <param name="x">指定水平切割每块图片的长度，取值范围[1,图片宽度]</param>
        public CropImageProcessParameter SetIndexCropX(Int32 x)
        {
            AddParameter(OP_INDEXCROP, "x_" + x.ToString());
            return this;
        }

        /// <summary>
        /// 进行垂直切割，每块图片的长度。x 参数与 y 参数只能任选其一。示例：/indexcrop,y_10
        /// </summary>
        /// <param name="y">指定垂直切割每块图片的长度，取值范围[1,图片宽度]</param>
        public CropImageProcessParameter SetIndexCropY(Int32 y)
        {
            AddParameter(OP_INDEXCROP, "y_" + y.ToString());
            return this;
        }

        /// <summary>
        /// 选择切割后第几个块，0表示第一块。示例：/indexcrop,i_5
        /// </summary>
        /// <param name="i">选择切割后第几个块。（0表示第一块）</param>
        public CropImageProcessParameter SetIndexCropIndex(Int32 i)
        {
            AddParameter(OP_INDEXCROP, "i_" + i.ToString());
            return this;
        }

        /// <summary>
        /// 将图片切出圆角，指定圆角的半径。示例：/rounded-corners,r_50
        /// </summary>
        /// <param name="r">圆角的半径，取值范围[1, 4096]且生成的最大圆角的半径不能超过原图的最小边的一半。</param>
        public CropImageProcessParameter SetRoundedCornersRadius(Int32 r)
        {
            AddParameter(OP_ROUNDEDCORNERS, "r_" + r.ToString());
            return this;
        }
    }
}
