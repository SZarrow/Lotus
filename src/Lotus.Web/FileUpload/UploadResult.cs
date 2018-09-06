using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lotus.Core;

namespace Lotus.Web.FileUpload
{
    /// <summary>
    /// The state of uploads.
    /// </summary>
    [Serializable]
    public class UploadResult : XResult<IEnumerable<String>>
    {
        internal UploadResult(IEnumerable<String> data, params Exception[] exceptions) : base(data, exceptions) { }
    }
}
