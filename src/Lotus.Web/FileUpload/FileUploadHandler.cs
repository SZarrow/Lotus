using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Lotus.Web.FileUpload
{
    public class FileUploadHandler : IHttpHandler
    {

        private readonly static String UploadAuthKey;
        private readonly static String UploadAllowedTypes;
        private readonly static Hashtable StatusContainer;

        public Boolean IsReusable
        {
            get { return false; }
        }

        static FileUploadHandler()
        {
            UploadAuthKey = WebConfigurationManager.AppSettings["UploadAuthKey"];
            UploadAllowedTypes = WebConfigurationManager.AppSettings["UploadAllowedTypes"];
            StatusContainer = new Hashtable();
        }

        public void ProcessRequest(HttpContext context)
        {
            String sign = context.Request.Unvalidated["sign"];
            if (String.IsNullOrWhiteSpace(sign)) { return; }

            var arg = new RequestArgument()
            {
                Sign = sign
            };

            String method = context.Request.HttpMethod.ToUpperInvariant();

            if (method == "GET") { DoGet(context, arg); }

            if (method == "POST")
            {

                Int32 taskKey = arg.Sign.GetHashCode();

                #region 组织参数
                String origin = context.Request.Unvalidated["origin"];
                if (String.IsNullOrWhiteSpace(origin))
                {
                    SetError(taskKey, "origin参数值为空！");
                    return;
                }

                origin = HttpUtility.UrlDecode(origin);
                SortedList<String, String> paras = new SortedList<String, String>();
                paras.Add("origin", origin);
                arg.Origin = origin;

                Int32 ow = context.Request.Unvalidated["overwrite"].ToInt32(0);
                Boolean overwrite = (ow == 1);
                paras.Add("overwrite", ow.ToString());
                arg.Overwrite = overwrite;

                String dir = context.Request.Unvalidated["dir"];
                if (String.IsNullOrWhiteSpace(dir) ||
                    !Regex.IsMatch(dir, @"^[a-z\d\-_~/\w]+$", RegexOptions.IgnoreCase))
                {
                    SetError(taskKey, "dir参数格式错误！");
                    return;
                }
                paras.Add("dir", dir);
                arg.Dir = dir;

                String computedSign = SignHelper.Sign(paras, UploadAuthKey);
                if (computedSign != sign)
                {
                    SetError(taskKey, "签名不一致！");
                    return;
                }
                #endregion

                DoPost(context, arg);
            }

        }

        private void DoGet(HttpContext context, RequestArgument arg)
        {

            Int32 taskKey = arg.Sign.GetHashCode();
            if (StatusContainer.ContainsKey(taskKey))
            {
                context.Response.ContentEncoding = Encoding.UTF8;
                context.Response.ContentType = "application/json";
                Print(context.Response, StatusContainer[taskKey]);
                SPLock.Lock(() =>
                {
                    StatusContainer.Remove(taskKey);
                });
            }
            else
            {
                SetError(taskKey, null);
            }
        }
        private void DoPost(HttpContext context, RequestArgument arg)
        {
            if (String.IsNullOrWhiteSpace(UploadAuthKey))
            {
                throw new KeyNotFoundException("appSettings/UploadAuthKey配置节不能为空！");
            }

            if (String.IsNullOrWhiteSpace(UploadAllowedTypes))
            {
                throw new KeyNotFoundException("appSettings/UploadAllowedTypes配置节不能为空！");
            }

            //将上传目录映射到根目录下
            String dir = arg.Dir;
            if (!dir.StartsWith("~/") && !dir.StartsWith("/"))
            {
                dir = VirtualPathUtility.AppendTrailingSlash("~/" + dir);
            }

            context.Response.Headers.Add("Access-Control-Allow-Origin", arg.Origin);
            context.Response.Headers.Add("Access-Control-Request-Methods", "POST");
            //context.Response.Headers.Add("Access-Control-Allow-Headers", "*");

            String uploadPhysicalDir = context.Server.MapPath(dir);
            if (!Directory.Exists(uploadPhysicalDir))
            {
                Directory.CreateDirectory(uploadPhysicalDir);
            }

            var uploadFiles = context.Request.Files;
            var fileMgr = FileManagerFactory.CreateFileManager(new UploadLimits()
            {
                AllowedFileTypes = UploadAllowedTypes,
                AllowOverride = arg.Overwrite
            });

            for (Int32 i = 0; i < uploadFiles.Count; i++)
            {
                var file = uploadFiles[i];
                fileMgr.AddFile(new UploadFileInfo(new HttpPostedFileWrapper(file), uploadPhysicalDir));
            }

            //执行上传操作
            var result = fileMgr.Upload();

            //处理结果
            Int32 taskKey = arg.Sign.GetHashCode();
            if (result.Errors != null)
            {
                SetError(taskKey, result.Errors[0].Message);
                return;
            }

            var successFilePaths = result.Value;
            var returnFileUrls = from t0 in successFilePaths
                                 select WebHelper.GetAbsUrl(dir + Path.GetFileName(t0), true);

            SetResult(taskKey, returnFileUrls);
        }

        private void SetError(Int32 taskKey, String error)
        {
            SPLock.Lock(() =>
            {
                StatusContainer[taskKey] = JsonConvert.SerializeObject(new { error = error });
            });
        }
        private void SetResult(Int32 taskKey, Object result)
        {
            SPLock.Lock(() =>
            {
                StatusContainer[taskKey] = JsonConvert.SerializeObject(new { result = result });
            });
        }
        private void Print(HttpResponse resp, Object cont)
        {
            resp.Write(cont);
        }
    }

    [Serializable]
    internal class RequestArgument
    {
        public String Sign { get; set; }
        public String Origin { get; set; }
        public Boolean Overwrite { get; set; }
        public String Dir { get; set; }
    }
}
