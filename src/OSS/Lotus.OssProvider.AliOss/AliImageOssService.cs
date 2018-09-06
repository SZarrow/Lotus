using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lotus.Core;
using Lotus.OssCore;
using Lotus.OssCore.Common;
using Lotus.OssCore.Domain;

namespace Lotus.OssProvider.AliOss
{
    public class AliImageOssService : IImageOssService
    {
        private IOssProvider _ossProvider;

        public AliImageOssService() : this(null) { }

        public AliImageOssService(IOssProvider provider)
        {
            if (provider == null)
            {
                provider = new AliOssProvider();
            }

            _ossProvider = provider;
        }

        public XResult<String> GetSignedAccessUrl(String bucketName, String imageKey, TimeSpan expireInterval, String styleName = null)
        {
            if (String.IsNullOrWhiteSpace(bucketName))
            {
                return new XResult<String>(null, new ArgumentNullException("bucketName"));
            }

            if (String.IsNullOrWhiteSpace(imageKey))
            {
                return new XResult<String>(null, new ArgumentNullException("imageKey"));
            }

            IDictionary<String, String> subRes = null;

            if (!String.IsNullOrWhiteSpace(styleName))
            {
                subRes = new Dictionary<String, String>(1);
                subRes["x-oss-process"] = "style/" + styleName;
            }

            return _ossProvider.GetSignedAccessUrl(bucketName, imageKey, expireInterval, subRes);
        }

        public XResult<XOssObject> Process(XOssObject image, params ImageProcessParameter[] parameters)
        {
            if (image == null)
            {
                return new XResult<XOssObject>(null, new ArgumentNullException("image"));
            }

            if (parameters == null || parameters.Length == 0)
            {
                return new XResult<XOssObject>(null, new ArgumentNullException("未指定图片处理参数"));
            }

            XResult<XOssObject> result = null;

            if (parameters != null && parameters.Length > 0)
            {
                result = _ossProvider.GetObject(image.BucketName, image.ObjectKey, MergeImageProcessParameters(parameters));
            }
            else
            {
                result = _ossProvider.GetObject(image.BucketName, image.ObjectKey);
            }

            return result;
        }

        public XResult<IEnumerable<XOssObject>> Process(IEnumerable<XOssObject> images, params ImageProcessParameter[] parameters)
        {
            if (images == null)
            {
                return new XResult<IEnumerable<XOssObject>>(null, new ArgumentNullException("images"));
            }

            if (parameters == null || parameters.Length == 0)
            {
                return new XResult<IEnumerable<XOssObject>>(null, new ArgumentNullException("parameters"));
            }

            Int32 capacity = images.Count();
            List<XOssObject> succeedObjects = new List<XOssObject>(capacity);
            List<Exception> innerExceptions = new List<Exception>(capacity);

            foreach (var image in images)
            {
                var result = Process(image, parameters);
                if (result.Success)
                {
                    succeedObjects.Add(result.Value);
                }
                else
                {
                    innerExceptions.Add(result.Exceptions[0]);
                }
            }

            return new XResult<IEnumerable<XOssObject>>(succeedObjects, innerExceptions.ToArray());
        }

        public XResult<XOssObject> Upload(XOssObject image, params ImageProcessParameter[] parameters)
        {
            if (image == null)
            {
                return new XResult<XOssObject>(null, new ArgumentNullException("image"));
            }

            if (image.Content == null || image.Content.Length == 0)
            {
                return new XResult<XOssObject>(null, new FormatException("the format of image is invalid"));
            }

            Func<XOssObject, XResult<XOssObject>> callback = null;

            if (parameters != null && parameters.Length > 0)
            {
                callback = img =>
                {
                    return Process(img, parameters);
                };
            }

            return _ossProvider.PutObject(image, callback);
        }

        public XResult<IEnumerable<XOssObject>> Upload(IEnumerable<XOssObject> images, params ImageProcessParameter[] parameters)
        {
            if (images == null || images.Count() == 0)
            {
                return new XResult<IEnumerable<XOssObject>>(null, new ArgumentNullException("images"));
            }

            Func<XOssObject, XResult<XOssObject>> callback = null;

            if (parameters != null && parameters.Length > 0)
            {
                callback = img =>
                {
                    return Process(img, parameters);
                };
            }

            return _ossProvider.PutObjects(images, callback);
        }

        public XResult<Boolean> Delete(XOssObject image)
        {
            if (image == null)
            {
                return new XResult<Boolean>(false, new ArgumentNullException("image"));
            }

            return _ossProvider.DeleteObject(image);
        }

        public XResult<IEnumerable<String>> Delete(String bucketName, String[] imageKeys)
        {
            if (String.IsNullOrWhiteSpace(bucketName))
            {
                return new XResult<IEnumerable<String>>(null, new ArgumentNullException("bucketName"));
            }

            if (imageKeys == null || imageKeys.Length == 0)
            {
                return new XResult<IEnumerable<String>>(null, new ArgumentNullException("imageKeys"));
            }

            return _ossProvider.DeleteObjects(bucketName, imageKeys);
        }

        private String MergeImageProcessParameters(ImageProcessParameter[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
            {
                return String.Empty;
            }

            return "image" + String.Concat(from p in parameters select p.GetProcessArguments());
        }

    }
}
