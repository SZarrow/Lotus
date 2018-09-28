using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Aliyun.OSS;
using Newtonsoft.Json;
using Lotus.OssCore;
using Lotus.OssCore.Common;
using Lotus.OssCore.Domain;
using Lotus.OssCore.Util;
using Lotus.OssProvider.AliOss;
using Xunit;
using Lotus.Core;

namespace Lotus.OssProvider.AliOss.Tests
{
    public class AliOssProviderTest
    {
        private TimeSpan ExpireInterval = new TimeSpan(0, 15, 0);

        [Fact]
        public void TestPutObject()
        {
            IOssProvider provider = new AliOssProvider();

            String key = "~/Resource/PictureFile/20180605/CarBodyPhoto/Original_3225c473-0648-4791-9a83-ddb1ee15f1d8.jpeg";//"/carpic/CarBodyPhoto/1687114014.jpeg";//"TestPut.jpg";
            String buckName = "sh-oss-1";
            String filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"images\01.jpg");

            using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                var result = provider.PutObject(new XOssObject(buckName, key, fs));
                if (result.Success)
                {
                    Trace.WriteLine($"成功上传文件。");
                    Assert.True(true);
                }
            }

            Assert.False(false);
        }

        [Fact]
        public void TestPutObjects()
        {
            IOssProvider provider = new AliOssProvider();

            List<XOssObject> images = new List<XOssObject>();
            for (var i = 2; i <= 4; i++)
            {
                images.Add(new XOssObject("sh-oss-1",
                    $"Test{i.ToString().PadLeft(2, '0')}.jpg",
                    File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"images\{i.ToString().PadLeft(2, '0')}.jpg"))));
            }

            var result = provider.PutObjects(images);
            Assert.True(result.Success);
        }

        [Fact]
        public void TestPutObjectWithUrlEncodedName()
        {
            IOssProvider provider = new AliOssProvider();

            String key = "~/Resource/PictureFile/20180605/CarBodyPhoto/关于思维科学 [钱学森主编][上海人民出版社][1986][458页].pdf";
            key = HttpUtility.UrlEncode(key);
            String buckName = "sh-oss-1";
            String filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"images\01.pdf");

            using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                var result = provider.PutObject(new XOssObject(buckName, key, fs));
                if (result.Success)
                {
                    Trace.WriteLine($"成功上传文件。");
                    Assert.True(true);
                }
            }

            Assert.False(false);
        }

        [Fact]
        public void TestAppendObject()
        {
            IOssProvider provider = new AliOssProvider();

            String bucketName = "sh-oss-1";
            String key = $"images/2018/{DateTime.Now.ToString("yyyyMMddHHmmss")}.jpg";

            String filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"images\05.jpg");

            XResult<Boolean> result = null;

            using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                result = provider.AppendObject(new XOssObject(bucketName, key, fs));
            }

            if (result.Success)
            {
                Trace.WriteLine("上传成功。");
            }
            else
            {
                Trace.WriteLine($"上传失败。原因：{result.Exceptions[0].Message}");
            }

            Assert.True(result.Success);
        }

        [Fact]
        public void TestDeleteObjects()
        {
            IOssProvider provider = new AliOssProvider();

            String filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"images\06.jpg");
            using (var fs = File.OpenRead(filePath))
            {
                provider.PutObject(new XOssObject("sh-oss-1", "TestAppend.jpg", fs));
            }

            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"images\07.jpg");
            using (var fs = File.OpenRead(filePath))
            {
                provider.PutObject(new XOssObject("sh-oss-1", "TestPut.jpg", fs));
            }

            var result = provider.DeleteObjects("sh-oss-1", new String[] { "TestAppend.jpg", "TestPut.jpg" });
            if (result.Success)
            {
                Trace.WriteLine("删除成功。");
            }
            else
            {
                Trace.WriteLine($"删除失败，原因：{result.Exceptions[0].Message}");
            }

            Assert.True(result.Success);
        }

        [Fact]
        public void TestDeleteObjectsByDirectory()
        {
            IOssProvider provider = new AliOssProvider();
            var result = provider.DeleteObjects("sh-oss-1", "images/2018/");
            if (result.Success)
            {
                Trace.WriteLine("删除成功。");
            }
            else
            {
                Trace.WriteLine($"删除失败，原因：{result.Exceptions[0].Message}");
            }

            Assert.True(result.Success);
        }

        [Fact]
        public void TestGetObject()
        {
            IOssProvider provider = new AliOssProvider();
            var result = provider.GetObject("sh-oss-1", "images/2018/20180530104558.jpg");

            if (result.Success)
            {
                XOssObject obj = result.Value;
                Trace.WriteLine($"成功获取对象：{obj.ObjectKey}");

                //将对象的流保存到文件
                Stream stream = obj.Content;
                String savePath = Path.Combine(AppContext.BaseDirectory, obj.ObjectKey);

                String saveDir = Path.GetDirectoryName(savePath);
                if (!Directory.Exists(saveDir))
                {
                    Directory.CreateDirectory(saveDir);
                }

                if (stream != null && stream.Length > 0)
                {
                    using (var fs = new FileStream(savePath, FileMode.Create, FileAccess.Write))
                    {
                        Byte[] buffer = new Byte[2048];
                        Int32 read = 0;

                        while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fs.Write(buffer, 0, read);
                        }
                    }
                }
            }
            else
            {
                Trace.WriteLine($"获取对象失败，原因：{result.Exceptions[0].Message}");
            }

            Assert.True(result.Success);
        }

        [Fact]
        public void TestGetObjects()
        {
            IOssProvider provider = new AliOssProvider();

            var result = provider.GetObjects("sh-oss-1");
            if (result.Success)
            {
                String info = String.Join(", ", (from t0 in result.Value
                                                 select t0.ObjectKey));
                Trace.WriteLine($"获取目录下的所有文件：{info}");
            }

            Assert.True(result.Success);

            String dir = "images/2018/";
            result = provider.GetObjects("sh-oss-1", dir);
            if (result.Success)
            {
                String info = String.Join(", ", (from t0 in result.Value
                                                 select t0.ObjectKey));
                Trace.WriteLine($"获取目录下的所有文件：{info}");
            }

            Assert.True(result.Success);
        }

        [Fact]
        public void TestGenAccessUri()
        {
            const Int64 TicksOf1970 = 621355968000000000;
            Int32 interval = 30;
            String expires = ((DateTime.Now.AddSeconds(interval).ToUniversalTime().Ticks - TicksOf1970) / 10000000L).ToString();
            String accessId = "LTAIEhA9nSIwFvdN";
            String accessSecret = "4t4NGrvesx26LlN70wQfAaDTLx74Dh";
            String bucketName = "sh-oss-1";
            String objectKey = "images/2018/upload-201805291744.jpg";
            String styleName = "s-med";
            //resourcePath区分大小写
            String resourcePath = $"/{bucketName}/{objectKey}?x-oss-process=style/{styleName}";
            String protocalString = $"GET\n\n\n{expires}\n{resourcePath}";
            var signedResult = SignUtil.HMACSHA1Base64String(protocalString, accessSecret);
            if (signedResult.Success)
            {
                String resultUrl = "https://oss.suziware.com" + resourcePath.Substring(9) + $"&OSSAccessKeyId={accessId}&Expires={expires}&Signature={HttpUtility.UrlEncode(signedResult.Value)}";
                Trace.WriteLine(resultUrl);
            }
            else
            {
                Trace.Write("签名失败:" + signedResult.Exceptions[0].Message);
            }
        }

        [Fact]
        public void TestGetSignedAccessUrlWithStyle()
        {
            IOssProvider provider = new AliOssProvider();
            var dic = new Dictionary<String, String>(1);
            dic["x-oss-process"] = "style/s-min";

            var testExistedFile = provider.GetSignedAccessUrl("sh-oss-1", "images/2018/upload-201805291744.jpg", ExpireInterval, dic);
            Assert.True(testExistedFile.Success);
            Trace.WriteLine("GetExistedFileAccessUrl: " + testExistedFile.Value);

            var testNonExistedFile = provider.GetSignedAccessUrl("sh-oss-1", "~/Resource/PictureFile/20181217/CarBodyPhoto/f5b14a68-a16b-4f96-b075-0fdffd4fa03c.png", ExpireInterval, dic);
            Assert.True(testNonExistedFile.Success);
            Trace.WriteLine("GetNonExistedFileAccessUrl: " + testNonExistedFile.Value);
        }

        [Fact]
        public void TestGetSignedAccessUrl()
        {
            IOssProvider provider = new AliOssProvider();
            String key = "~/Resource/PictureFile/20180605/CarBodyPhoto/关于思维科学 [钱学森主编][上海人民出版社][1986][458页].pdf";
            key = HttpUtility.UrlEncode(key);
            var testChineseFileName = provider.GetSignedAccessUrl("sh-oss-1", key, ExpireInterval);
            Assert.True(testChineseFileName.Success);
            Trace.WriteLine("GetAccessUrl: " + testChineseFileName.Value);
        }

        [Fact]
        public void TestListObjects()
        {
            IOssProvider provider = new AliOssProvider();

            String dir = "images/2018/";
            var result = provider.ListObjects("sh-oss-1", dir);

            Assert.True(result.Success);

            String info = String.Join(", ", (from t0 in result.Value
                                             select t0.ObjectKey));
            Trace.WriteLine($"获取目录下的所有文件：{info}");
        }

        [Fact]
        public void TestExists()
        {
            IOssProvider provider = new AliOssProvider();

            String key = "images/2018/20180910132805-1.jpg";
            var result = provider.Exists("sh-oss-1", key);

            Assert.False(result.Success);

            key = "images/2018/20180910132805.jpg";
            result = provider.Exists("sh-oss-1", key);

            Assert.True(result.Success);
        }

    }
}
