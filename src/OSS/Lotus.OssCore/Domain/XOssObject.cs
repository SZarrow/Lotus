using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lotus.OssCore.Domain
{
    /// <summary>
    /// 用来存储Oss对象信息的类。
    /// </summary>
    [Serializable]
    public class XOssObject
    {
        /// <summary>
        /// 实例化XOssObject类。
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="objectKey"></param>
        public XOssObject(String bucketName, String objectKey)
            : this(bucketName, objectKey, null) { }

        /// <summary>
        /// 实例化XOssObject类。
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="objectKey"></param>
        /// <param name="content"></param>
        public XOssObject(String bucketName, String objectKey, Stream content)
        {
            if (String.IsNullOrWhiteSpace(bucketName))
            {
                throw new ArgumentNullException("bucketName");
            }

            if (String.IsNullOrWhiteSpace(objectKey))
            {
                throw new ArgumentNullException("objectKey");
            }

            this.ObjectKey = objectKey;
            this.BucketName = bucketName;

            if (content != null && content.CanSeek)
            {
                content.Position = 0;
            }

            this.Content = content;
        }

        /// <summary>
        /// 
        /// </summary>
        public String ObjectKey { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String BucketName { get; private set; }

        /// <summary>
        /// 获取Oss对象的内容，注意要判断内容是否为空。
        /// 上传图片时，此属性可不赋值。
        /// </summary>
        public Stream Content { get; private set; }

        /// <summary>
        /// 获取或设置对象的处理参数
        /// </summary>
        public String Process { get; set; }
    }
}
