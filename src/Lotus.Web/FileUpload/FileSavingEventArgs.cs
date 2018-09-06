using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace Lotus.Web.FileUpload
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class FileSavingEventArgs : EventArgs
    {

        /// <summary>
        /// 
        /// </summary>
        public Object State { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="file"></param>
        public FileSavingEventArgs(IFormFile file, Object state)
        {
            this.File = file;
            this.State = state;
        }

        /// <summary>
        /// 指定是否取消后续操作
        /// </summary>
        public Boolean Cancel { get; set; }

        /// <summary>
        /// 上传的文件
        /// </summary>
        public IFormFile File { get; }

    }
}
