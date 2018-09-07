using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lotus.Core;

namespace Lotus.Web.FileUpload
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFileWriter
    {
        /// <summary>
        /// 写入磁盘
        /// </summary>
        /// <param name="file">要上传的文件信息</param>
        /// <param name="allowOverride">是否覆盖同名文件，True表示覆盖。</param>
        XResult<String> Write(UploadFileInfo file, Boolean allowOverride);
    }
}
