using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lotus.Web.FileUpload
{
    /// <summary>
    /// 
    /// </summary>
    public static class FileManagerFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="limits"></param>
        /// <returns></returns>
        public static IFileManager CreateFileManager(UploadLimits limits = null)
        {
            return new FormFileManager(limits, null);
        }
    }
}
