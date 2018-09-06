using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lotus.OssCore.Common;
using Lotus.OssCore.Common.ImageProcessParameters;

namespace Lotus.OssCore.Common
{
    /// <summary>
    /// 图片处理参数助手类，提供方便的接口创建图片参数类的实例。
    /// </summary>
    public class ImageProcessParameterBuilder
    {
        private List<ImageProcessParameter> _paras;

        /// <summary>
        /// 实例化ImageProcessParameterBuilder。
        /// </summary>
        public ImageProcessParameterBuilder()
        {
            _paras = new List<ImageProcessParameter>(6);
        }

        /// <summary>
        /// 构建出图片处理参数集合。
        /// </summary>
        public ImageProcessParameter[] Build()
        {
            if (_paras.Count > 0)
            {
                return _paras.ToArray();
            }

            return new ImageProcessParameter[0];
        }

        /// <summary>
        /// 创建一个表示图片裁剪参数的实例。
        /// </summary>
        public CropImageProcessParameter CreateCropImageProcessParameter()
        {
            var p = new CropImageProcessParameter();
            _paras.Add(p);
            return p;
        }

        /// <summary>
        /// 创建一个表示图片效果参数的实例。
        /// </summary>
        public EffectiveImageProcessParameter CreateEffectiveImageProcessParameter()
        {
            var p = new EffectiveImageProcessParameter();
            _paras.Add(p);
            return p;
        }

        /// <summary>
        /// 创建一个表示图片格式参数的实例。
        /// </summary>
        public FormatImageProcessParameter CreateFormatImageProcessParameter()
        {
            var p = new FormatImageProcessParameter();
            _paras.Add(p);
            return p;
        }

        /// <summary>
        /// 创建一个表示图片缩放参数的实例。
        /// </summary>
        public ResizeImageProcessParameter CreateResizeImageProcessParameter()
        {
            var p = new ResizeImageProcessParameter();
            _paras.Add(p);
            return p;
        }

        /// <summary>
        /// 创建一个表示图片旋转参数的实例。
        /// </summary>
        public RotateImageProcessParameter CreateRotateImageProcessParameter()
        {
            var p = new RotateImageProcessParameter();
            _paras.Add(p);
            return p;
        }

        /// <summary>
        /// 创建一个表示图片水印参数的实例。
        /// </summary>
        public WaterMarkImageProcessParameter CreateWaterMarkImageProcessParameter()
        {
            var p = new WaterMarkImageProcessParameter();
            _paras.Add(p);
            return p;
        }
    }
}
