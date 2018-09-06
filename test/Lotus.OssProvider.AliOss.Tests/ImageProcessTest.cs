using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Lotus.OssCore;
using Lotus.OssCore.Common;
using Lotus.OssCore.Common.ImageProcessParameters;
using Lotus.OssCore.Domain;
using Lotus.OssProvider.AliOss;
using Xunit;

namespace Lotus.OssProvider.AliOss.Tests
{
    public class ImageProcessTest
    {
        private TimeSpan ExpireInterval = new TimeSpan(0, 15, 0);

        [Fact]
        public void TestImageProcessParameters()
        {
            var pb = new ImageProcessParameterBuilder();

            //添加Resize处理参数
            pb.CreateResizeImageProcessParameter()
                .SetResizeMode(ResizeMode.LFit)//设置缩略模式
                .SetWidth(800)//设置宽度
                .SetResizeLimit(false);//设置目标缩略图大于原图不做处理

            //添加Rotate处理参数
            pb.CreateRotateImageProcessParameter()
                .SetRotateOrientation(true);//设置旋转方向为自适应

            //添加Format处理参数
            pb.CreateFormatImageProcessParameter()
                .SetRelativeQuality(90)//设置图片的相对质量
                .SetFormat(SupportedImageFormat.jpg)//设置处理后的图片的格式
                .SetInterlace(true);//设置图片的呈现方式为自上而下扫描式

            //添加Watermark处理参数
            pb.CreateWaterMarkImageProcessParameter()
                .SetFontText(new UrlSafeBase64String("Hello World"))//设置文字水印的内容
                .SetFontType(new UrlSafeBase64String("wqy-microhei"))//设置文字水印的字体名称
                .SetFontSize(80)//设置文字水印的字体大小
                .SetTransparency(100)//设置文字水印的透明度
                .SetFontColor("ffffff")//设置文字水印的字体颜色
                .SetPosition(GridPosition.Center)//设置文字水印的位置
                .SetX(10)//设置文字水印的水平距离
                .SetY(10);//设置文字水印的垂直距离

            IImageOssService imgServ = new AliImageOssService();

            //假设我们要处理位于/sh-oss-1/images/snow.jpg位置的图片
            var ossObject = new XOssObject("sh-oss-1", "images/snow.jpg");

            //执行处理
            var result = imgServ.Process(ossObject, pb.Build());

            //处理成功后将图片写到本地
            if (result.Success)
            {
                var imgStream = result.Value.Content;
                String filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", $"processed_{DateTime.Now.ToString("yyyyMMddHHmmss")}.jpg");

                using (var fs = File.OpenWrite(filePath))
                {
                    Byte[] buf = new Byte[1024];
                    Int32 read = 0;

                    while ((read = imgStream.Read(buf, 0, buf.Length)) > 0)
                    {
                        fs.Write(buf, 0, read);
                    }
                }

                Trace.WriteLine($"处理成功: {filePath}");
            }
            else
            {
                Trace.WriteLine($"处理失败：{result.Exceptions[0].Message}");
            }
        }

        [Fact]
        public void TestUploadImage()
        {
            IImageOssService imgServ = new AliImageOssService();
            var pb = new ImageProcessParameterBuilder();

            //添加Resize处理参数
            pb.CreateResizeImageProcessParameter()
                .SetResizeMode(ResizeMode.LFit)//设置缩略模式
                .SetWidth(800)//设置宽度
                .SetResizeLimit(false);//设置目标缩略图大于原图不做处理

            var fs = File.OpenRead(Path.Combine(AppContext.BaseDirectory, @"images\21.jpg"));
            var result = imgServ.Upload(new XOssObject("sh-oss-1", "Test_upload_21.jpg", fs), pb.Build());

            Assert.True(result.Success);
        }

        [Fact]
        public void TestUploadImages()
        {
            IImageOssService imgServ = new AliImageOssService();
            var pb = new ImageProcessParameterBuilder();

            //添加Resize处理参数
            pb.CreateResizeImageProcessParameter()
                .SetResizeMode(ResizeMode.LFit)//设置缩略模式
                .SetWidth(800)//设置宽度
                .SetResizeLimit(false);//设置目标缩略图大于原图不做处理

            List<XOssObject> images = new List<XOssObject>();
            for (var i = 11; i <= 13; i++)
            {
                images.Add(new XOssObject("sh-oss-1",
                    $"Test{i.ToString().PadLeft(2, '0')}.jpg",
                    File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"images\{i.ToString().PadLeft(2, '0')}.jpg"))));
            }

            var result = imgServ.Upload(images, pb.Build());

            Assert.True(result.Success);
        }

        [Fact]
        public void TestDeleteImage()
        {
            IImageOssService imgServ = new AliImageOssService();

            String filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"images\20.jpg");
            using (var fs = File.OpenRead(filePath))
            {
                imgServ.Upload(new XOssObject("sh-oss-1", "TestDeleteImage.jpg", fs));
            }

            var result = imgServ.Delete(new XOssObject("sh-oss-1", "TestDeleteImage.jpg"));
            Assert.True(result.Value);
        }

        [Fact]
        public void TestDeleteImages()
        {
            IImageOssService imgServ = new AliImageOssService();
            String filePath;

            for (var i = 14; i <= 16; i++)
            {
                filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"images\{i.ToString()}.jpg");
                using (var fs = File.OpenRead(filePath))
                {
                    imgServ.Upload(new XOssObject("sh-oss-1", $"Test{i.ToString()}.jpg", fs));
                }
            }
            var result = imgServ.Delete("sh-oss-1", new String[] { "Test14.jpg", "Test15.jpg", "Test16.jpg" });
            Assert.True(result.Success);
            Trace.WriteLine(result.Value);
        }

        [Fact]
        public void TestGetImageSignedAccessUrl()
        {
            IImageOssService imgServ = new AliImageOssService();
            var result = imgServ.GetSignedAccessUrl("sh-oss-1", "TestPut.jpg", ExpireInterval, "s-min");
            Assert.True(result.Success);
            String accessUrl = result.Value;
            Trace.WriteLine(accessUrl);
        }

    }
}
