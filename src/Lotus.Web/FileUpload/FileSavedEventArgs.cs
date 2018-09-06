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
    public sealed class FileSavedEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public Object State { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public String FilePath { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="state"></param>
        public FileSavedEventArgs(String filePath, Object state)
        {
            this.FilePath = filePath;
            this.State = state;
        }
    }
}
