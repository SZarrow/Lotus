using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lotus.Core;
using Lotus.OssCore.Common;
using Lotus.OssCore.Domain;

namespace Lotus.OssCore
{
    /// <summary>
    /// 图片处理接口，图片处理支持的格式：jpg、png、bmp、gif、webp、tiff。
    /// </summary>
    public interface IImageOssService
    {
        /// <summary>
        /// 根据参数获取经过签名的图片访问地址
        /// </summary>
        /// <param name="bucketName">The name of the bucket which the key is saved.</param>
        /// <param name="imageKey">The key value of image.</param>
        /// <param name="expireInterval">Set the interval seconds before expiration.</param>
        /// <param name="styleName">The style name applied to the image.</param>
        XResult<String> GetSignedAccessUrl(String bucketName, String imageKey, TimeSpan expireInterval, String styleName = null);

        /// <summary>
        /// 根据参数处理指定图片
        /// </summary>
        /// <param name="image">要处理的图片</param>
        /// <param name="parameters">图片的处理参数</param>
        XResult<XOssObject> Process(XOssObject image, params ImageProcessParameter[] parameters);

        /// <summary>
        /// Process specified images with parameters.
        /// </summary>
        /// <param name="images">The images will be processed.</param>
        /// <param name="parameters">The parameters for processing.</param>
        /// <returns>Returns the images which are processed successfully.</returns>
        XResult<IEnumerable<XOssObject>> Process(IEnumerable<XOssObject> images, params ImageProcessParameter[] parameters);

        /// <summary>
        /// 上传一张图片
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="image">要上传的图片</param>
        XResult<XOssObject> Upload(XOssObject image, params ImageProcessParameter[] parameters);

        /// <summary>
        /// 上传多张图片
        /// </summary>
        /// <param name="images">要上传的多张图片</param>
        /// <param name="parameters"></param>
        XResult<IEnumerable<XOssObject>> Upload(IEnumerable<XOssObject> images, params ImageProcessParameter[] parameters);

        /// <summary>
        /// 删除一张图片
        /// </summary>
        /// <param name="image">要删除的图片</param>
        XResult<Boolean> Delete(XOssObject image);

        /// <summary>
        /// 删除多张图片
        /// </summary>
        /// <param name="bucketName">要删除的图片所在的bucket的名称</param>
        /// <param name="imageKeys">要删除的图片的key值</param>
        /// <returns>返回删除成功的图片的key值</returns>
        XResult<IEnumerable<String>> Delete(String bucketName, String[] imageKeys);

        /// <summary>
        /// 删除多张图片
        /// </summary>
        /// <param name="bucketName">要删除的图片所在的bucket的名称</param>
        /// <param name="directoryPath">要删除的目录路径</param>
        /// <returns>返回删除成功的图片的key值</returns>
        XResult<IEnumerable<String>> Delete(String bucketName, String directoryPath);
    }
}
