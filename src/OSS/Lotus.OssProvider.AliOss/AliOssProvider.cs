using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Aliyun.OSS;
using Newtonsoft.Json;
using Lotus.OssCore;
using Lotus.OssCore.Common;
using Lotus.OssCore.Domain;
using Lotus.OssCore.Util;
using Lotus.OssProvider.AliOss.Util;
using Lotus.Core;

namespace Lotus.OssProvider.AliOss
{
    public class AliOssProvider : IOssProvider
    {
        private OssClient _client;

        public AliOssProvider()
        {
            var result = OssClientFactory.GetOssClient();
            if (result.Success)
            {
                _client = result.Value;
            }
        }

        public XResult<String> GetSignedAccessUrl(String bucketName, String objectKey, TimeSpan expireInterval, IDictionary<String, String> subResources = null)
        {
            if (String.IsNullOrWhiteSpace(bucketName))
            {
                return new XResult<String>(null, new ArgumentNullException("bucketName"));
            }

            if (String.IsNullOrWhiteSpace(objectKey))
            {
                return new XResult<String>(null, new ArgumentNullException("objectKey"));
            }

            AliOssUtil.EnsureObjectKey(ref objectKey);

            var configResult = AliAccountConfig.GetConfig(AliAccountConfig.ReaderRoleId);
            if (!configResult.Success)
            {
                return new XResult<String>(null, configResult.Exceptions.ToArray());
            }

            var config = configResult.Value;
            const Int64 TicksOf1970 = 621355968000000000;
            String expires = ((DateTime.Now.AddSeconds(expireInterval.TotalSeconds).ToUniversalTime().Ticks - TicksOf1970) / 10000000L).ToString();
            String subRes = String.Empty;

            if (subResources != null && subResources.Count > 0)
            {
                subRes = "?" + String.Join("&", (from t0 in subResources select t0.Key + "=" + t0.Value));
            }

            String resourcePath = $"/{bucketName}/{objectKey}{subRes}";
            String protocalString = $"GET\n\n\n{expires}\n{resourcePath}";

            var signedResult = SignUtil.HMACSHA1Base64String(protocalString, config.AccessKeySecret);
            if (!signedResult.Success)
            {
                return new XResult<String>(null, signedResult.Exceptions.ToArray());
            }

            String objectPath = $"/{objectKey}{subRes}";
            String splitChar = subRes.Length > 0 ? "&" : "?";
            String resultUrl = $"{config.EndPoint.AbsoluteUri.TrimEnd('/')}{objectPath}{splitChar}OSSAccessKeyId={config.AccessKeyId}&Expires={expires}&Signature={HttpUtility.UrlEncode(signedResult.Value)}";
            return new XResult<String>(resultUrl);
        }

        public XResult<Boolean> AppendObject(XOssObject obj)
        {
            if (obj == null)
            {
                return new XResult<Boolean>(false, new ArgumentNullException("obj"));
            }

            if (obj.Content == null || obj.Content.Length == 0)
            {
                return new XResult<Boolean>(false, new ArgumentException("obj.Content is null or empty"));
            }

            AliOssUtil.EnsureXOssObject(obj);

            AppendObjectResult result = null;
            try
            {
                result = _client.AppendObject(new AppendObjectRequest(obj.BucketName, obj.ObjectKey)
                {
                    Content = obj.Content
                });
            }
            catch (Exception ex)
            {
                return new XResult<Boolean>(false, ex);
            }

            if (result == null)
            {
                return new XResult<Boolean>(false);
            }

            if (result.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                return new XResult<Boolean>(false, new Exception($"the \"Append\" ocurs error：httpStatusCode={result.HttpStatusCode}"));
            }

            return new XResult<Boolean>(true);
        }

        public XResult<Boolean> DeleteObject(XOssObject deletingObject)
        {
            if (deletingObject == null)
            {
                return new XResult<Boolean>(false, new ArgumentNullException("deletingObject"));
            }

            AliOssUtil.EnsureXOssObject(deletingObject);

            try
            {
                _client.DeleteObject(deletingObject.BucketName, deletingObject.ObjectKey);
                return new XResult<Boolean>(true);
            }
            catch (Exception ex)
            {
                return new XResult<Boolean>(false, ex);
            }
        }

        public XResult<IEnumerable<String>> DeleteObjects(String bucketName, String[] objectKeys)
        {
            if (String.IsNullOrWhiteSpace(bucketName))
            {
                return new XResult<IEnumerable<String>>(null, new ArgumentNullException("bucketName"));
            }

            if (objectKeys == null || objectKeys.Count() == 0)
            {
                return new XResult<IEnumerable<String>>(null, new ArgumentNullException("objectKeys"));
            }

            AliOssUtil.EnsureObjectKeys(ref objectKeys);

            try
            {
                var result = _client.DeleteObjects(new DeleteObjectsRequest(bucketName, objectKeys, false));
                if (result == null || result.Keys == null)
                {
                    return new XResult<IEnumerable<String>>(null);
                }

                return new XResult<IEnumerable<String>>(result.Keys.Select(x => x.Key));
            }
            catch (Exception ex)
            {
                return new XResult<IEnumerable<String>>(null, ex);
            }
        }

        public XResult<XOssObject> GetObject(String bucketName, String objectKey, Object args = null)
        {
            if (String.IsNullOrWhiteSpace(bucketName))
            {
                return new XResult<XOssObject>(null, new ArgumentNullException("bucketName"));
            }

            if (String.IsNullOrWhiteSpace(objectKey))
            {
                return new XResult<XOssObject>(null, new ArgumentNullException("objectKey"));
            }

            AliOssUtil.EnsureObjectKey(ref objectKey);

            try
            {
                OssObject result = null;

                if (args == null)
                {
                    result = _client.GetObject(bucketName, objectKey);
                }
                else
                {
                    result = _client.GetObject(new GetObjectRequest(bucketName, objectKey, args.ToString()));
                }

                return new XResult<XOssObject>(new XOssObject(bucketName, objectKey, result.Content));
            }
            catch (Exception ex)
            {
                return new XResult<XOssObject>(null, ex);
            }
        }

        public XResult<IEnumerable<XOssObject>> GetObjects(String bucketName, String searchDirectory = null, Object args = null)
        {
            if (String.IsNullOrWhiteSpace(bucketName))
            {
                return new XResult<IEnumerable<XOssObject>>(null, new ArgumentNullException("bucketName"));
            }

            if (!String.IsNullOrWhiteSpace(searchDirectory))
            {
                if (searchDirectory.StartsWith("/"))
                {
                    searchDirectory = searchDirectory.TrimStart('/');
                }

                if (!searchDirectory.EndsWith("/"))
                {
                    searchDirectory += "/";
                }
            }

            var listObjectsRequest = new ListObjectsRequest(bucketName);

            if (!String.IsNullOrWhiteSpace(searchDirectory))
            {
                listObjectsRequest.Prefix = searchDirectory;
                listObjectsRequest.Delimiter = "/";
            }

            ObjectListing objectListing = null;
            try
            {
                objectListing = _client.ListObjects(listObjectsRequest);
            }
            catch (Exception ex)
            {
                return new XResult<IEnumerable<XOssObject>>(null, ex);
            }

            var summaries = objectListing.ObjectSummaries;
            if (summaries == null)
            {
                return new XResult<IEnumerable<XOssObject>>(null);
            }

            var tasks = new List<Task>(summaries.Count());
            var objects = new ConcurrentQueue<XOssObject>();

            foreach (var summary in summaries)
            {
                var task = Task.Run(() =>
                {
                    var xr = this.GetObject(summary.BucketName, summary.Key, args);
                    if (xr.Success)
                    {
                        objects.Enqueue(xr.Value);
                    }
                });

                tasks.Add(task);
            }

            try
            {
                Task.WaitAll(tasks.ToArray());
                return new XResult<IEnumerable<XOssObject>>(objects.ToArray());
            }
            catch (Exception ex)
            {
                return new XResult<IEnumerable<XOssObject>>(null, ex);
            }
        }

        public XResult<XOssObject> PutObject(XOssObject obj, Func<XOssObject, XResult<XOssObject>> callback = null)
        {
            if (obj == null)
            {
                return new XResult<XOssObject>(null, new ArgumentNullException("obj"));
            }

            if (obj.Content == null || obj.Content.Length == 0)
            {
                return new XResult<XOssObject>(null, new ArgumentNullException("obj.Content is null or empty"));
            }

            AliOssUtil.EnsureXOssObject(obj);

            PutObjectResult result = null;
            try
            {
                result = _client.PutObject(new PutObjectRequest(obj.BucketName, obj.ObjectKey, obj.Content));
            }
            catch (Exception ex)
            {
                return new XResult<XOssObject>(null, ex);
            }

            if (result == null)
            {
                return new XResult<XOssObject>(null);
            }

            if (result.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                return new XResult<XOssObject>(null, new Exception($"the \"Put\" operation ocurs error：httpStatusCode={result.HttpStatusCode}"));
            }

            if (callback != null)
            {
                try
                {
                    return callback(obj);
                }
                catch (Exception ex)
                {
                    return new XResult<XOssObject>(null, ex);
                }
            }

            return new XResult<XOssObject>(obj);
        }

        public XResult<IEnumerable<XOssObject>> PutObjects(IEnumerable<XOssObject> objects, Func<XOssObject, XResult<XOssObject>> callback = null)
        {
            if (objects == null || objects.Count() == 0)
            {
                return new XResult<IEnumerable<XOssObject>>(null, new ArgumentNullException("objects"));
            }

            AliOssUtil.EnsureXOssObjects(objects);

            var succeedObjects = new ConcurrentQueue<XOssObject>();
            var tasks = new List<Task>(objects.Count());

            foreach (var obj in objects)
            {
                if (!String.IsNullOrWhiteSpace(obj.BucketName) &&
                    !String.IsNullOrWhiteSpace(obj.ObjectKey))
                {
                    tasks.Add(Task.Run(() =>
                    {
                        var result = this.PutObject(obj, callback);
                        if (result.Success)
                        {
                            succeedObjects.Enqueue(obj);
                        }
                    }));
                }
            }

            try
            {
                Task.WaitAll(tasks.ToArray());
                return new XResult<IEnumerable<XOssObject>>(succeedObjects.ToArray());
            }
            catch (Exception ex)
            {
                return new XResult<IEnumerable<XOssObject>>(null, ex);
            }
        }
    }
}
