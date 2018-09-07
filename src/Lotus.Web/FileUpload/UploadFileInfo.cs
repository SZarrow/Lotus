using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Lotus.Web.FileUpload
{
    /// <summary>
    /// The info of upload file.
    /// </summary>
    [Serializable]
    public class UploadFileInfo
    {

        /// <summary>
        /// 
        /// </summary>
        /// /// <param name="fileName">文件名</param>
        /// <param name="uploadFile">上传文件</param>
        /// <param name="uploadPhysicalDirectory">上传文件的物理全路径</param>
        public UploadFileInfo(String fileName, Stream content, String uploadPhysicalDirectory)
        {
            if (String.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            if (String.IsNullOrWhiteSpace(uploadPhysicalDirectory))
            {
                throw new ArgumentNullException("uploadPhysicalDirectory");
            }

            this.Content = content;
            _uploadPhysicalDirectory = uploadPhysicalDirectory;
        }

        /// <summary>
        /// Gets the name of upload file.
        /// </summary>
        public String FileName
        {
            get
            {
                return !String.IsNullOrWhiteSpace(_newFileName) ? _newFileName : _uploadFile.FileName;
            }
        }
        /// <summary>
        /// Gets the upload file.
        /// </summary>
        public Stream Content { get; }

        /// <summary>
        /// Gets the physical upload path.
        /// </summary>
        public String UploadPhysicalDirectory
        {
            get { return _uploadPhysicalDirectory; }
        }
    }
}
