using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace Lotus.Web.FileUpload
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFileManager
    {
        /// <summary>
        /// The event will be triggered after validated and before writing.
        /// </summary>
        event FileSavingDelegate FileSaving;
        /// <summary>
        /// The event will be triggered after writing.
        /// </summary>
        event FileSavedDelegate FileSaved;
        /// <summary>
        /// Add file to the upload queue list.
        /// </summary>
        /// <param name="file"></param>
        void AddFile(UploadFileInfo file);
        /// <summary>
        /// Execute uploading.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        UploadResult Upload(Object state = null);
    }


    /// <summary>
    /// A delegate, triggered after validated and before writing.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void FileSavingDelegate(Object sender, FileSavingEventArgs e);

    /// <summary>
    /// A delegate, triggered after saved.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void FileSavedDelegate(Object sender,FileSavedEventArgs e);
}

