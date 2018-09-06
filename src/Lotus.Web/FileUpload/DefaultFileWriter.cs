using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Lotus.Core;

namespace Lotus.Web.FileUpload
{
    //internal class DefaultFileWriter : IFileWriter
    //{
    //    public XResult<String> Write(UploadFileInfo file, Boolean allowOverride)
    //    {
    //        String uploadDirectory = file.UploadPhysicalDirectory;
    //        if (!Directory.Exists(uploadDirectory))
    //        {
    //            return new XResult<String>(null, new DirectoryNotFoundException(uploadDirectory));
    //        }

    //        //Gets the full upload path.
    //        String filePath = Path.Combine(uploadDirectory, file.FileName);
    //        // if there is the file which have the same filename 
    //        if (File.Exists(filePath))
    //        {
    //            //if the user allows overriding, then overrides the same-name file.
    //            if (allowOverride)
    //            {
    //                try
    //                {
    //                    File.Delete(filePath);
    //                }
    //                catch (Exception ex)
    //                {
    //                    return new XResult<String>(null, ex);
    //                }
    //            }
    //            else
    //            {
    //                //if the user don't permit overriding, then renames the same-name file.
    //                String fileNameNonExt = Guid.NewGuid().ToString("N").Substring(0, 8);
    //                String extName = Path.GetExtension(file.FileName);
    //                filePath = Path.Combine(uploadDirectory, fileNameNonExt + extName);
    //            }
    //        }

    //        FileStream fs = null;
    //        try
    //        {
    //            Stream stream = file.UploadFile.OpenReadStream();
    //            fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write);
    //            using (var bs = new BufferedStream(stream, 2 * 1024))
    //            {
    //                Int32 read = 0;
    //                Byte[] buf = new Byte[1024];
    //                while ((read = bs.Read(buf, 0, buf.Length)) > 0)
    //                {
    //                    fs.Write(buf, 0, read);
    //                }
    //            }
    //            //if (file.UploadFile.ContentType.StartsWith("text/")) {
    //            //    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8)) {
    //            //        using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8)) {
    //            //            Char[] buf;
    //            //            while (reader.Peek() >= 0) {
    //            //                buf = new Char[2 * 1024];
    //            //                reader.Read(buf, 0, buf.Length);
    //            //                writer.Write(buf);
    //            //            }
    //            //        }
    //            //    }
    //            //}
    //            //else {

    //            //}
    //            return new XResult<String>(filePath);
    //        }
    //        catch (Exception ex)
    //        {
    //            String errorMsg = String.Format("文件{0}保存失败，原因：{1}", file.FileName, ex.Message);
    //            return new XResult<String>(null, ex);
    //        }
    //        finally
    //        {
    //            if (fs != null) { fs.Dispose(); }
    //        }
    //    }

    //}
}
