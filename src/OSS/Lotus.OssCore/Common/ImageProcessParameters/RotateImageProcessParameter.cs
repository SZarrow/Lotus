using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lotus.OssCore.Common.ImageProcessParameters
{
    /// <summary>
    /// 旋转图片参数类
    /// </summary>
    public class RotateImageProcessParameter : ImageProcessParameter
    {
        private const String OP_AUTOORIENT = "auto-orient";
        private const String OP_ROTATE = "rotate";

        /// <summary>
        /// 某些手机拍摄出来的照片可能带有旋转参数（存放在照片exif信息里面）。可以设置是否对这些图片进行旋转。默认是设置自适应方向。示例：/auto-orient,1
        /// </summary>
        /// <param name="value">True表示先进行图片进行旋转，然后再进行缩略；False表示按原图默认方向，不进行自动旋转。</param>
        public RotateImageProcessParameter SetRotateOrientation(Boolean value)
        {
            AddParameter(OP_AUTOORIENT, (value ? "1" : "0"));
            return this;
        }

        /// <summary>
        /// 设置图片的顺时针旋转角度。示例：/rotate,45
        /// </summary>
        /// <param name="angle">顺时针旋转角度，默认值为 0，表示不旋转。取值范围[0-360]</param>
        public RotateImageProcessParameter SetRotate(Int32 angle)
        {
            AddParameter(OP_ROTATE, angle.ToString());
            return this;
        }
    }
}
