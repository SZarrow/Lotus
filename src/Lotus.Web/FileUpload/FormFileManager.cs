using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Lotus.Web.FileUpload
{
    /// <summary>
    /// 
    /// </summary>
    //internal class FormFileManager : IFileManager
    //{
    //    private UploadLimits _limits;
    //    private IFileWriter _fileWritter;
    //    private List<UploadFileInfo> _fileList;
    //    private FileSavingDelegate _fileSavingCallback;
    //    private FileSavedDelegate _fileSavedCallback;

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="limits"></param>
    //    /// <param name="fileWritter"></param>
    //    public FormFileManager(UploadLimits limits = null, IFileWriter fileWritter = null)
    //    {
    //        _limits = limits != null ? limits : new UploadLimits();
    //        _fileWritter = fileWritter != null ? fileWritter : new DefaultFileWriter();
    //        _fileList = new List<UploadFileInfo>();
    //    }

    //    /// <summary>
    //    /// 添加文件到上传队列。
    //    /// </summary>
    //    /// <param name="file"></param>
    //    public void AddFile(UploadFileInfo file)
    //    {
    //        _fileList.Add(file);
    //    }

    //    /// <summary>
    //    /// 执行上传操作。
    //    /// </summary>
    //    /// <param name="state"></param>
    //    /// <returns></returns>
    //    public UploadResult Upload(Object state = null)
    //    {
    //        if (_fileList.Count == 0)
    //        {
    //            return new UploadResult(null, new ArgumentNullException("未选择任何上传的文件"));
    //        }

    //        if (_fileList.Count > _limits.MaxFileCount)
    //        {
    //            return new UploadResult(null, new ArgumentException(String.Format("一次最多只能上传{0}个文件", _limits.MaxFileCount)));
    //        }

    //        if (String.IsNullOrWhiteSpace(_limits.AllowedFileTypes))
    //        {
    //            return new UploadResult(null, new InvalidOperationException("不允许添加任何类型文件"));
    //        }

    //        String[] allowdFileTypes = _limits.AllowedFileTypes.Split('|');
    //        var postedFiles = new List<String>();
    //        var exceptions = new List<Exception>();

    //        foreach (var fileInfo in _fileList)
    //        {
    //            var file = fileInfo.UploadFile;

    //            if (file == null)
    //            {
    //                continue;
    //            }

    //            var findTypes = allowdFileTypes.Where(x => String.Compare(x, file.ContentType, StringComparison.OrdinalIgnoreCase) == 0);
    //            if (findTypes == null || findTypes.Count() == 0)
    //            {
    //                exceptions.Add(new ArgumentNullException(String.Format("文件{0}的ContentType类型不匹配", file.FileName)));
    //                continue;
    //            }

    //            if (file.ContentLength > _limits.SingleFileMaxSize * 1024)
    //            {
    //                exceptions.Add(new ArgumentOutOfRangeException(String.Format("文件{0}大小超过最大上传文件大小（{1}KB）", file.FileName, _limits.SingleFileMaxSize)));
    //                continue;
    //            }

    //            //执行验证通过后的回调函数
    //            if (_fileSavingCallback != null)
    //            {
    //                FileSavingEventArgs args = new FileSavingEventArgs(file, state);
    //                _fileSavingCallback(this, args);
    //                //如果用户取消了后续的保存操作，则直接处理下一个文件
    //                if (args.Cancel) { continue; }
    //            }

    //            var result = _fileWritter.Write(fileInfo, _limits.AllowOverride);
    //            if (result.Exceptions != null && result.Exceptions.Count > 0)
    //            {
    //                exceptions.Add(result.Exceptions[0]);
    //            }
    //            else
    //            {
    //                if (_fileSavedCallback != null)
    //                {
    //                    _fileSavedCallback(this, new FileSavedEventArgs(result.Value, state));
    //                }

    //                postedFiles.Add(result.Value);
    //            }
    //        }

    //        var retval = postedFiles.Count > 0 ? postedFiles : null;
    //        return new UploadResult(retval, exceptions.ToArray());
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public event FileSavingDelegate FileSaving
    //    {
    //        add
    //        {
    //            _fileSavingCallback += value;
    //        }
    //        remove
    //        {
    //            _fileSavingCallback -= value;
    //        }
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public event FileSavedDelegate FileSaved
    //    {
    //        add
    //        {
    //            _fileSavedCallback += value;
    //        }
    //        remove
    //        {
    //            _fileSavedCallback -= value;
    //        }
    //    }

    //}
}
