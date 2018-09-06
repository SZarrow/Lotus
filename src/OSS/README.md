# OSS - Object Storage Service
This repo provides an easy way to use object storage services, currently supports only AliOss.

### Project Dependency

The dependent nuget packages：
* Aliyun.OSS.SDK -v2.8.0
* Newtonsoft.Json -v11.0.2

### The base interfaces calling example
Notice: The way to instantiate the IOssProvider interface is just for demo, the actual use should be changed to IoC, so don't be obsessed with the way of creating below.

The description of the bucketName and key in the following interface:

* **bucketName** It can be understood as a directory on the cloud server, which is a root directory under your cloud account.
* **key** In fact, it is the subpath under the root directory defined by bucketName.

If key is just a file name and does not contain a directory, then the object will exist in the root directory defined by bucketName.
If key is a path containing directory, then this object will be stored according to the path defined in key.
For eaxmple: if key="images/2018/aaa.jpg", then this object will be saved to "{bucketName}/images/2018/aaa.jpg".


### Upload Object

* Using PutObject()

```csharp
IOssProvider provider = new AliOssProvider();

String bucketName = "sh-oss-1";
String key = "TestPut.jpg";

String filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"images\1.jpg");

using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read))
{
    var result = provider.PutObject(new XOssObject(bucketName, key, fs));
    if (result.Sucess)
    {
        Trace.WriteLine($"Upload Succ.");
    }
    else
    {
        Trace.WriteLine("Upload Fail.");
    }
}
```
* Using AppendObject()

```csharp
IOssProvider provider = new AliOssProvider();

String bucketName = "sh-oss-1";
String key = "images/2018/TestAppend.jpg";
            
String filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"images\2.jpg");

XResult<Boolean> result = null;

using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read))
{
    result = provider.AppendObject(new XOssObject(bucketName, key, fs));
}

if (result.Sucess)
{
    Trace.WriteLine("Upload Succ.");
}
else
{
    Trace.WriteLine($"Upload Fail, Reason: {result.ErrorMessage}");
}
```
### Delete Object

```csharp
IOssProvider provider = new AliOssProvider();

var result = provider.DeleteObjects("sh-oss-1", new String[] { "TestAppend.jpg", "TestPut.jpg" });
if (result.Sucess)
{
    Trace.WriteLine("Delete Succ.");
}
else
{
    Trace.WriteLine($"Delete Fail, Reason: {result.ErrorMessage}");
}
```
### Get Object
```csharp
IOssProvider provider = new AliOssProvider();

var result = provider.GetObject("sh-oss-1", "TestAppend.jpg");

if (result.Sucess)
{
    XOssObject obj = result.Value;
    Trace.WriteLine($"Get: {obj.Key}");

    //Save stream to file
    Stream stream = obj.Content;
    String savePath = Path.Combine(AppContext.BaseDirectory, obj.Key);

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
    Trace.WriteLine($"Get Fail, Reason: {result.ErrorMessage}");
}
```
### Get Objects
```csharp
IOssProvider provider = new AliOssProvider();

var result = provider.GetObjects("sh-oss-1");
if (result.Sucess)
{
    String info = String.Join(", ", (from t0 in result.Value
                                        select t0.Key));
    Trace.WriteLine($"Get all files: {info}");
}

Assert.IsTrue(result.Sucess);

//Notice: The directory must start with the root directory. 
//The separator between the subdirectories must use "/", and the directory must end with "/", but cannot start with "/".
String dir = "images/2018/";

result = provider.GetObjects("sh-oss-1", dir);
if (result.Sucess)
{
    String info = String.Join(", ", (from t0 in result.Value
                                        select t0.Key));
    Trace.WriteLine($"Get all files: {info}");
}

Assert.IsTrue(result.Sucess);
```

### Process Images

```csharp
[TestMethod]
public void TestImageProcessParameters()
{
    var pb = new ImageProcessParameterBuilder();

    //Add Resize parameter
    pb.CreateResizeImageProcessParameter()
        .SetResizeMode(ResizeMode.LFit)
        .SetWidth(800)
        .SetResizeLimit(false);

    //Add Rotate parameter
    pb.CreateRotateImageProcessParameter()
        .SetRotateOrientation(true);

    //Add Format parameter
    pb.CreateFormatImageProcessParameter()
        .SetRelativeQuality(90)
        .SetFormat(SupportedImageFormat.jpg)
        .SetInterlace(true);

    //Add Watermark parameter
    pb.CreateWaterMarkImageProcessParameter()
        .SetFontText(new UrlSafeBase64String("Hello World"))
        .SetFontType(new UrlSafeBase64String("wqy-microhei"))
        .SetFontSize(80)
        .SetTransparency(100)
        .SetFontColor("ffffff")
        .SetPosition(GridPosition.Center)
        .SetX(10)
        .SetY(10);

    IImageOssService imgServ = new AliImageOssService();

    //Assume we want to process pictures at "/sh-oss-1/images/2018/TestAppend.jpg".
    var ossObject = new XOssObject("sh-oss-1", "images/2018/TestAppend.jpg");

    //Begin process
    var result = imgServ.Process(ossObject, pb.Build());

    //Write the picture to local after the processing is successful
    if (result.Sucess)
    {
        var imgStream = result.Value.Content;
        String filePath = @"D:\test\3_processed.jpg";

        using (var fs = File.OpenWrite(filePath))
        {
            Byte[] buf = new Byte[1024];
            Int32 read = 0;

            while ((read = imgStream.Read(buf, 0, buf.Length)) > 0)
            {
                fs.Write(buf, 0, read);
            }
        }

        Trace.WriteLine($"Process Succ: {filePath}");
    }
    else
    {
        Trace.WriteLine($"Process Fail: {result.ErrorMessage}");
    }
}
```

### Get Signed AccessUrl

```csharp
[TestMethod]
public void TestGetImageSignedAccessUrl()
{
    IImageOssService imgServ = new AliImageOssService();
    var result = imgServ.GetSignedAccessUrl("sh-oss-1", "TestPut.jpg");
    Assert.IsTrue(result.Sucess);
    String accessUrl = result.Value;
    Trace.WriteLine(accessUrl);
}
```