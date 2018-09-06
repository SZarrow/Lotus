using System;
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
    public class AliImageProcessor : IImageProcess
    {
        private IOssProvider _provider;

        public AliImageProcessor() : this(null) { }

        public AliImageProcessor(IOssProvider provider)
        {
            if (provider == null)
            {
                provider = new AliOssProvider();
            }

            _provider = provider;
        }

        public XResult<XOssObject> Process(String bucketName, String objectKey, params ImageProcessParameter[] parameters)
        {
            XResult<XOssObject> result = null;

            if (parameters != null && parameters.Length > 0)
            {
                result = _provider.GetObject(bucketName, objectKey, MergeImageProcessParameters(parameters));
            }
            else
            {
                result = _provider.GetObject(bucketName, objectKey);
            }

            return result;
        }

        private String MergeImageProcessParameters(ImageProcessParameter[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
            {
                return String.Empty;
            }

            return "image/" + String.Join("/", (from t0 in parameters
                                                select t0.GetProcessArguments()));
        }
    }
}
