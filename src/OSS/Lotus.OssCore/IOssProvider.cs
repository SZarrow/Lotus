using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lotus.Core;
using Lotus.OssCore.Domain;

namespace Lotus.OssCore
{
    /// <summary>
    /// 表示Oss提供程序接口，不同第三方Oss服务商提供的Oss服务均要实现此接口。
    /// </summary>
    public interface IOssProvider
    {
        /// <summary>
        /// 获取指定对象的经过签名的访问地址。
        /// </summary>
        /// <param name="bucketName">对象所在的bucket的名称</param>
        /// <param name="objectKey">对象的路径</param>
        /// <param name="expireInterval">过期时间间隔</param>
        /// <param name="subResources"></param>
        XResult<String> GetSignedAccessUrl(String bucketName, String objectKey, TimeSpan expireInterval, IDictionary<String, String> subResources = null);
        /// <summary>
        /// 追加一个对象到云端。
        /// </summary>
        /// <param name="obj">要添加的对象。</param>
        /// <returns>添加成功返回True，否则返回False。</returns>
        XResult<Boolean> AppendObject(XOssObject obj);
        /// <summary>
        /// 删除云端一个对象。
        /// </summary>
        /// <param name="deletingObject">要删除的对象。</param>
        /// <returns>删除成功返回True，否则Fasle。</returns>
        XResult<Boolean> DeleteObject(XOssObject deletingObject);
        /// <summary>
        /// 删除指定bucket下的指定名称的对象集合。
        /// </summary>
        /// <param name="bucketName">要删除的对象所在的bucket的名称</param>
        /// <param name="objectKeys">要删除的对象的路径</param>
        /// <returns>返回删除成功的对象的集合。</returns>
        XResult<IEnumerable<String>> DeleteObjects(String bucketName, String[] objectKeys);
        /// <summary>
        /// 上传一个对象到云端。
        /// </summary>
        /// <param name="obj">要上传的对象。</param>
        /// <param name="callback"></param>
        /// <returns>返回上传成功的对象，如果对图片进行了处理，则返回处理后的对象。</returns>
        XResult<XOssObject> PutObject(XOssObject obj, Func<XOssObject, XResult<XOssObject>> callback = null);
        /// <summary>
        /// 上传多个对象到云端。
        /// </summary>
        /// <param name="objects">要上传的多个对象。</param>
        /// <param name="callback"></param>
        /// <returns>返回上传成功的对象的集合，如果对图片进行了处理，则返回处理后的对象。</returns>
        XResult<IEnumerable<XOssObject>> PutObjects(IEnumerable<XOssObject> objects, Func<XOssObject, XResult<XOssObject>> callback = null);
        /// <summary>
        /// 获取指定bucket下指定名称的对象。
        /// </summary>
        /// <param name="bucketName">要获取的对象所在的bucket的名称。</param>
        /// <param name="objectKey">要获取的对象的路径。</param>
        /// <param name="args">附加参数</param>
        XResult<XOssObject> GetObject(String bucketName, String objectKey, Object args = null);
        /// <summary>
        /// 如果未指定searchDirectory将获取指定bucket下的所有对象，否则只获取searchDirectory目录下的对象。
        /// </summary>
        /// <param name="bucketName">要获取的对象所在的bucket的名称。</param>
        /// <param name="searchDirectory">要获取的对象所在的目录。注意：目录要从根目录开始，子目录间分隔符要使用"/"，必须以"/"结尾但不能以"/"开头。例如：可以指定这样的目录"images/2018/"。</param>
        /// <param name="args">附加参数</param>
        XResult<IEnumerable<XOssObject>> GetObjects(String bucketName, String searchDirectory = null, Object args = null);
    }
}
