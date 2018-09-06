using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Lotus.OssCore.Domain;

namespace Lotus.OssProvider.AliOss.Util
{
    public class AliOssUtil
    {
        public static void EnsureObjectKey(ref String objectKey)
        {
            if (!String.IsNullOrWhiteSpace(objectKey))
            {
                objectKey = HttpUtility.UrlDecode(objectKey);
                objectKey = objectKey.TrimStart('~', '/').Replace('\\', '/');
            }
        }

        public static void EnsureObjectKeys(ref String[] objectKeys)
        {
            if (objectKeys != null && objectKeys.Length > 0)
            {
                for (var i = 0; i < objectKeys.Length; i++)
                {
                    EnsureObjectKey(ref objectKeys[i]);
                }
            }
        }

        public static void EnsureXOssObject(XOssObject obj)
        {
            if (obj != null && !String.IsNullOrWhiteSpace(obj.ObjectKey))
            {
                obj.ObjectKey = HttpUtility.UrlDecode(obj.ObjectKey);
                obj.ObjectKey = obj.ObjectKey.TrimStart('~', '/').Replace('\\', '/');
            }
        }

        public static void EnsureXOssObjects(IEnumerable<XOssObject> objects)
        {
            if (objects != null)
            {
                foreach (var obj in objects)
                {
                    EnsureXOssObject(obj);
                }
            }
        }
    }
}
