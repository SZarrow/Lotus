using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using System.Net;

//批量付款API应用
#region 大批量付款结算应用类
/// <summary>
/// 包含签名/验签，压缩/解压缩，加密/解密，字符类型转换，文件操作，SFTP上传下载
/// </summary>
public class bigpay
{

    #region 参数类型转换
    /// <summary>
    ///把两个 byte数组拼凑成一个数组
    /// </summary>
    public static byte[] AppendArrays(byte[] a, byte[] b)
    {
        byte[] c = new byte[a.Length + b.Length]; // just one array allocation
        Buffer.BlockCopy(a, 0, c, 0, a.Length);
        Buffer.BlockCopy(b, 0, c, a.Length, b.Length);
        return c;
    }

    /// <summary>
    /// 获取字节值，将字符型转换成字节型【strData：原字符；TypeId编码类型1：Base64编码， 2：UTF8编码， 3：ASCII编码】
    /// </summary>
    public static byte[] CodingToByte(string strData, int TypeId)
    {
        try
        {
            byte[] byteData;
            switch (TypeId)
            {
                case 1:
                    byteData = Convert.FromBase64String(strData);
                    break;
                case 2:
                    byteData = System.Text.Encoding.UTF8.GetBytes(strData);
                    break;
                default:
                    System.Text.ASCIIEncoding ByteConverter = new ASCIIEncoding();
                    byteData = ByteConverter.GetBytes(strData);
                    break;
            }
            return byteData;
        }
        catch
        {
            byte[] bnull ={ };
            return bnull;
        }
    }

    /// <summary>
    /// 获取字符值， 将字节型转换成字符型【byteData：原字节；TypeId编码类型1：Base64编码， 2：UTF8编码， 3：ASCII编码】
    /// </summary>
    public static string CodingToString(byte[] byteData, int TypeId)
    {
        try
        {
            string stringData;
            switch (TypeId)
            {
                case 1:
                    stringData = System.Convert.ToBase64String(byteData);
                    break;
                case 2:
                    stringData = System.Text.Encoding.UTF8.GetString(byteData);
                    break;
                default:
                    System.Text.ASCIIEncoding ByteConverter = new ASCIIEncoding();
                    stringData = ByteConverter.GetString(byteData);
                    break;
            }
            return stringData;
        }
        catch
        {
            string bnull = "";
            return bnull;
        }
    }

    #endregion

    #region 随机key：64位、128位、192位
    /// <summary>
    /// 获取随机key 【BitType：位数  传入1：64位/8个字符；传入2：128位/16个字符；传入3：192位/24个字符；】
    /// </summary>
    public static byte[] randomKey(int BitType)
    {
        try
        {
            string typeName = "";
            int typeSize = 0;
            if (BitType == 1)
            {
                typeName = "DES";
                typeSize = 64;
            }
            if (BitType == 2)
            {
                typeName = "TripleDES";
                typeSize = 128;
            }
            if (BitType == 3)
            {
                typeName = "3DES";
                typeSize = 192;
            } 

            SymmetricAlgorithm symmProvider = SymmetricAlgorithm.Create(typeName);
            symmProvider.KeySize = typeSize;
            byte[] bkey = symmProvider.Key;
            return bkey;
        }
        catch
        {
            byte[] bnull ={ };
            return bnull;
        }
    }
    #endregion

    #region GZip压缩/解压缩字符串
    /// <summary>
    /// GZip压缩函数(支持版本：2.0),返回字符串
    /// </summary>
    public static string CompressGZip(byte[] strSource)
    {
        try
        {
            if (strSource == null)
                throw new System.ArgumentException("字符串为空！");
            //byte[] buffer = System.Text.Encoding.UTF8.GetBytes(strSource);
            byte[] buffer = strSource;
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.IO.Compression.GZipStream stream = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Compress, true);
            stream.Write(buffer, 0, buffer.Length);
            stream.Close();
            byte[] buffer1 = ms.ToArray();
            ms.Close();
            return Convert.ToBase64String(buffer1, 0, buffer1.Length); //将压缩后的byte[]转换为Base64String
        }
        catch
        {
            string bnull = "";
            return bnull;
        }
    }
    /// <summary>
    /// GZip压缩函数(支持版本：2.0),返回字节
    /// </summary>
    public static byte[] CompressGZipExt(byte[] strSource)
    {
        try
        {
            if (strSource == null)
                throw new System.ArgumentException("字符串为空！");
            //byte[] buffer = System.Text.Encoding.UTF8.GetBytes(strSource);
            byte[] buffer = strSource;
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.IO.Compression.GZipStream stream = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Compress, true);
            stream.Write(buffer, 0, buffer.Length);
            stream.Close();
            byte[] buffer1 = ms.ToArray();

            return buffer1;
        }
        catch
        {
            byte[] bnull = new byte[0];
            return bnull;
        }
    }
    /// <summary>
    /// Gzip解压缩函数(支持版本：2.0)【参数strSource：原文；codeType：指定编码类型，值1：Base64，值2：UTF8】
    /// </summary>
    public static string DecompressGZip(string strSource, int codeType)
    {
        try
        {
            if (strSource == null)
                throw new System.ArgumentException("字符串不能为空！");
            byte[] buffer = Convert.FromBase64String(strSource);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            ms.Write(buffer, 0, buffer.Length);
            ms.Position = 0;
            System.IO.Compression.GZipStream stream = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Decompress);
            stream.Flush();
            int nSize = 6000 * 1024 + 256; //字符串不超过6000K
            byte[] decompressBuffer = new byte[nSize];
            int nSizeIncept = stream.Read(decompressBuffer, 0, nSize);
            stream.Close();
            string backStr = "";
            if (codeType == 1)
            {
                backStr = System.Convert.ToBase64String(decompressBuffer, 0, nSizeIncept);
            }
            if (codeType == 2)
            {
                backStr = System.Text.Encoding.UTF8.GetString(decompressBuffer, 0, nSizeIncept);
            }
            ms.Close();
            return backStr;//转换为普通的字符串
        }
        catch
        {
            string bnull = "";
            return bnull;
        }

    }
    #endregion

    #region 签名算法及摘要

    #region MD5签名
    /// <summary>
    /// MD5签名，然后返回string类型签名数据（1：要签名的参数，2：编码方式）。
    /// </summary>
    public static string MD5Signature(string dataStr, string codeType)
    {
        try
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(System.Text.Encoding.GetEncoding(codeType).GetBytes(dataStr));
            System.Text.StringBuilder sb = new System.Text.StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }
        catch
        {
            string bnull = "";
            return bnull;
        }

    }
    #endregion

    #region MD5/SHA1摘要
    /// <summary>
    /// MD5摘要，然后返回byte类型摘要数据，传入字符串。
    /// </summary>
    public static byte[] MD5summary(string strSource)
    {
        try
        {
            byte[] bSource = System.Text.Encoding.UTF8.GetBytes(strSource);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(bSource);//摘要值
            //string backStr = Convert.ToBase64String(result);
            return result;
        }
        catch
        {
            byte[] bnull ={ };
            return bnull;
        }
    }

    /// <summary>
    /// MD5摘要，然后返回byte类型摘要数据，传入字节。
    /// </summary>
    public static byte[] MD5summaryExt(byte[] strSource)
    {
        try
        {
            // byte[] bSource = System.Text.Encoding.UTF8.GetBytes(strSource);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(strSource);//摘要值
            //string backStr = Convert.ToBase64String(result);
            return result;
        }
        catch
        {
            byte[] bnull = { };
            return bnull;
        }
    }

    /// <summary>
    /// SHA1摘要，然后返回byte类型摘要数据。
    /// </summary>
    public static byte[] SHA1summary(string strSource)
    {
        try
        {
            byte[] bSource = System.Text.Encoding.UTF8.GetBytes(strSource);
            SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
            byte[] HashData = sha.ComputeHash(bSource);
            //string backStr = Convert.ToBase64String(HashData);
            return HashData;
        }
        catch
        {
            byte[] bnull ={ };
            return bnull;
        }
    }
    #endregion

    #region 引用证书非对称签名/验签RSA
    /// <summary>
    /// 引用证书非对称签名/验签RSA-私钥签名【OriginalString：原文（有中文用utf-8编码的字节）；prikey_path：证书路径；CertificatePW：证书密码；SignType：签名摘要类型（1：MD5，2：SHA1）】
    /// </summary>
    public static byte[] CerRSASignature(byte[] OriginalString, string prikey_path, string CertificatePW, int SignType)
    {
        try
        {
            X509Certificate2 x509_Cer1 = new X509Certificate2(prikey_path, CertificatePW);
            RSACryptoServiceProvider rsapri = (RSACryptoServiceProvider)x509_Cer1.PrivateKey;
            RSAPKCS1SignatureFormatter f = new RSAPKCS1SignatureFormatter(rsapri);
            byte[] result;
            switch (SignType)
            {
                case 1:
                    f.SetHashAlgorithm("MD5");//摘要算法MD5
                    MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                    result = md5.ComputeHash(OriginalString);//摘要值
                    break;
                default:
                    f.SetHashAlgorithm("SHA1");//摘要算法SHA1
                    SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
                    result = sha.ComputeHash(OriginalString);//摘要值
                    break;
            }
            byte[] SignData = f.CreateSignature(result);
            return SignData;
        }
        catch
        {
            byte[] bnull ={ };
            return bnull;
        }
    }

    /// <summary>
    /// 引用证书非对称签名/验签RSA-公钥验签【OriginalString：原文（有中文用utf-8编码的字节）；SignatureString：签名字符；pubkey_path：证书路径；CertificatePW：证书密码；SignType：签名摘要类型（1：MD5，2：SHA1）】
    /// </summary>
    public static bool CerRSAVerifySignature(byte[] OriginalString, byte[] SignatureString, string pubkey_path, string CertificatePW, int SignType)
    {
        try
        {
            X509Certificate2 x509_Cer1 = new X509Certificate2(pubkey_path, CertificatePW);
            RSACryptoServiceProvider rsapub = (RSACryptoServiceProvider)x509_Cer1.PublicKey.Key;
            rsapub.ImportCspBlob(rsapub.ExportCspBlob(false));
            RSAPKCS1SignatureDeformatter f = new RSAPKCS1SignatureDeformatter(rsapub);
            byte[] HashData;
            switch (SignType)
            {
                case 1:
                    f.SetHashAlgorithm("MD5");//摘要算法MD5
                    MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                    HashData = md5.ComputeHash(OriginalString);
                    break;
                default:
                    f.SetHashAlgorithm("SHA1");//摘要算法SHA1
                    SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
                    HashData = sha.ComputeHash(OriginalString);
                    break;
            }
            if (f.VerifySignature(HashData, SignatureString))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }
    #endregion

    #region 引用证书非对称加密/解密RSA
    /// <summary>
    /// 引用证书非对称加密/解密RSA-公钥加密获取密文【DataToEncrypt：原文（有中文用utf-8编码的字节）；pubkey_path：证书路径；CertificatePW：证书密码】
    /// </summary>
    public static byte[] CerRSAEncrypt(byte[] DataToEncrypt, string pubkey_path, string CertificatePW)
    {
        try
        {
            X509Certificate2 x509_Cer1 = new X509Certificate2(pubkey_path, CertificatePW);
            RSACryptoServiceProvider rsapub = (RSACryptoServiceProvider)x509_Cer1.PublicKey.Key;
            byte[] bytes_Cypher_Text = rsapub.Encrypt(DataToEncrypt, false);
            return bytes_Cypher_Text;
        }
        catch
        {
            byte[] bnull ={ };
            return bnull;
        }
    }
    /// <summary>
    /// 引用证书非对称加密/解密RSA-私钥解密获取原文【DataToDecrypt：密文；prikey_path：证书路径；CertificatePW：证书密码】【原文有中文返回字节用utf-8编码转换成字符】
    /// </summary>
    public static byte[] CerRSADecrypt(byte[] DataToDecrypt, string prikey_path, string CertificatePW)
    {
        try
        {
            X509Certificate2 x509_Cer2 = new X509Certificate2(prikey_path, CertificatePW);
            RSACryptoServiceProvider rsapri = (RSACryptoServiceProvider)x509_Cer2.PrivateKey;
            byte[] bytes_Plain_Text = rsapri.Decrypt(DataToDecrypt, false);
            return bytes_Plain_Text;
        }
        catch
        {
            byte[] bnull ={ };
            return bnull;
        }
    }
    #endregion

    #region Des 64Bit 对称加密/解密算法
    /// <summary>
    /// Des 64Bit 算法加密字串【DataToEncrypt：原文；ekey：随机密码，8个字符】【原文有中文传入的原文字符用utf-8编码转换成字节】
    /// </summary>
    public static byte[] DesEncrypt(byte[] DataToEncrypt, byte[] ekey)
    {
        try
        {
            SymmetricAlgorithm mCSP = new DESCryptoServiceProvider();
            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            //向量指定的是：byte[] eiv ={0,0,0,0,0,0,0,0};
            byte[] eiv = CodingToByte("AAAAAAAAAAA=", 1);
            ct = mCSP.CreateEncryptor(ekey, eiv);
            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(DataToEncrypt, 0, DataToEncrypt.Length);
            cs.FlushFinalBlock();
            cs.Close();
            return ms.ToArray();
        }
        catch
        {
            byte[] bnull ={ };
            return bnull;
        }
    }
    /// <summary>
    /// Des 64Bit 算法解密字串【DataToDecrypt：密文；ekey：随机密码，8个字符】【原文有中文返回字节用utf-8编码转换成字符】
    /// </summary>
    public static byte[] DesDecrypt(byte[] DataToDecrypt, byte[] ekey)
    {
        try
        {
            SymmetricAlgorithm mCSP = new DESCryptoServiceProvider();
            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            //向量指定的是：byte[] eiv ={0,0,0,0,0,0,0,0};
            byte[] eiv = CodingToByte("AAAAAAAAAAA=", 1);
            ct = mCSP.CreateDecryptor(ekey, eiv);
            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(DataToDecrypt, 0, DataToDecrypt.Length);
            cs.FlushFinalBlock();
            cs.Close();
            return ms.ToArray();
        }
        catch
        {
            byte[] bnull ={ };
            return bnull;
        }
    }
    #endregion

    #region TripleDes 128Bit 对称加密/解密算法
    /// <summary>
    /// TripleDes 128Bit 算法加密字串【DataToEncrypt：原文；priKkey：随机密码，十六个字符】【原文有中文传入的原文字符用utf-8编码转换成字节】
    /// </summary>
    public static byte[] TripleDesEncrypt(byte[] DataToEncrypt, byte[] ekey)
    {
        try
        {
            SymmetricAlgorithm mCSP = new TripleDESCryptoServiceProvider();
            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            //向量指定的是：byte[] eiv ={0,0,0,0,0,0,0,0};
            byte[] eiv = CodingToByte("AAAAAAAAAAA=", 1);
            ct = mCSP.CreateEncryptor(ekey, eiv);
            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(DataToEncrypt, 0, DataToEncrypt.Length);
            cs.FlushFinalBlock();
            cs.Close();
            return ms.ToArray();
        }
        catch
        {
            byte[] bnull ={ };
            return bnull;
        }
    }
    /// <summary>
    /// TripleDes 128Bit 算法解密字串【DataToDecrypt：密文；priKkey：随机密码，十六个字符】【原文有中文返回字节用utf-8编码转换成字符】
    /// </summary>
    public static byte[] TripleDesDecrypt(byte[] DataToDecrypt, byte[] ekey)
    {
        try
        {
            SymmetricAlgorithm mCSP = new TripleDESCryptoServiceProvider();
            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            //向量指定的是：byte[] eiv ={0,0,0,0,0,0,0,0};
            byte[] eiv = CodingToByte("AAAAAAAAAAA=", 1);
            ct = mCSP.CreateDecryptor(ekey, eiv);
            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(DataToDecrypt, 0, DataToDecrypt.Length);
            cs.FlushFinalBlock();
            cs.Close();
            return ms.ToArray();
        }
        catch
        {
            byte[] bnull ={ };
            return bnull;
        }
    }
    #endregion

    #region Aes 128Bit 对称加密/解密算法
    /// <summary>
    /// AES加密 128Bit 算法加密字串【Data：原文；bKey：随机密码，16个字符】【原文有中文传入的原文字符用utf-8编码转换成字节】
    /// </summary>
    public static byte[] AesEncrypt(byte[] Data, byte[] bKey)
    {
        try
        {
            //向量指定的是：byte[] bVector ={0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
            byte[] bVector = Convert.FromBase64String("AAAAAAAAAAAAAAAAAAAAAA==");
            byte[] Cryptograph = null; // 加密后的密文
            Rijndael Aes = Rijndael.Create();

            // 开辟一块内存流
            using (MemoryStream Memory = new MemoryStream())
            {
                // 把内存流对象包装成加密流对象
                using (CryptoStream Encryptor = new CryptoStream(Memory,
                 Aes.CreateEncryptor(bKey, bVector),
                 CryptoStreamMode.Write))
                {
                    // 明文数据写入加密流
                    Encryptor.Write(Data, 0, Data.Length);
                    Encryptor.FlushFinalBlock();
                    Cryptograph = Memory.ToArray();
                }
            }
            return Cryptograph;
        }
        catch
        {
            byte[] bnull ={ };
            return bnull;
        }
    }
    /// <summary>
    /// Aes解密 128Bit 算法解密字串【Data：原文；bKey：随机密码，16个字符】【原文有中文传入的原文字符用utf-8编码转换成字节】
    /// </summary>
    public static byte[] AesDecrypt(byte[] Data, byte[] bKey)
    {
        try
        {
            //向量指定的是：byte[] bVector ={0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
            byte[] bVector = Convert.FromBase64String("AAAAAAAAAAAAAAAAAAAAAA==");
            byte[] original = null; // 解密后的明文
            Rijndael Aes = Rijndael.Create();

            // 开辟一块内存流，存储密文
            using (MemoryStream Memory = new MemoryStream(Data))
            {
                // 把内存流对象包装成加密流对象
                using (CryptoStream Decryptor = new CryptoStream(Memory,
                Aes.CreateDecryptor(bKey, bVector),
                CryptoStreamMode.Read))
                {
                    // 明文存储区
                    using (MemoryStream originalMemory = new MemoryStream())
                    {
                        byte[] Buffer = new byte[1024];
                        Int32 readBytes = 0;
                        while ((readBytes = Decryptor.Read(Buffer, 0, Buffer.Length)) > 0)
                        {
                            originalMemory.Write(Buffer, 0, readBytes);
                        }
                        original = originalMemory.ToArray();
                    }
                }
            }
            return original;
        }
        catch
        {
            byte[] bnull ={ };
            return bnull;
        }
    }
    #endregion

    #region 对称加密算法集合（Aes，Des，TripleDes）
    /// <summary>
    /// 对称加密算法集合【orgData：原数据；enKey：密码；EncryptType：加密类型|1：AES 需要128位key，2：DES 需要64位key，3：TripleDES 需要128位key】
    /// </summary>
    /// <param name="orgData">原文</param>
    /// <param name="enKey">密码</param>
    /// <param name="EncryptType">解密算法类型</param>
    /// <returns>SymmetryEncryptType：密文</returns>
    public static byte[] SymmetryEncryptType(byte[] orgData, byte[] enKey, int EncryptType)
    {
        try
        {
            byte[] badkData = null;
            if (EncryptType == 1)
            {
                badkData = bigpay.AesEncrypt(orgData, enKey);
            }
            if (EncryptType == 2)
            {
                badkData = bigpay.DesEncrypt(orgData, enKey);
            }
            if (EncryptType == 3)
            {
                badkData = bigpay.TripleDesEncrypt(orgData, enKey);
            }
            return badkData;
        }
        catch
        {
            byte[] bnull ={ };
            return bnull;
        }
    }

    /// <summary>
    /// 对称解密算法集合【orgData：原数据；enKey：密码；EncryptType：加密类型|1：AES 需要128位key，2：DES 需要64位key，3：TripleDES 需要128位key】
    /// </summary>
    /// <param name="orgData">密文</param>
    /// <param name="enKey">密码</param>
    /// <param name="DecryptType">解密算法类型</param>
    /// <returns>SymmetryDecryptType：明文</returns>
    public static byte[] SymmetryDecryptType(byte[] orgData, byte[] enKey, int DecryptType)
    {
        try
        {
            byte[] badkData = null;
            if (DecryptType == 1)
            {
                badkData = bigpay.AesDecrypt(orgData, enKey);
            }
            if (DecryptType == 2)
            {
                badkData = bigpay.DesDecrypt(orgData, enKey);
            }
            if (DecryptType == 3)
            {
                badkData = bigpay.TripleDesDecrypt(orgData, enKey);
            }
            return badkData;
        }
        catch
        {
            byte[] bnull ={ };
            return bnull;
        }
    }
    #endregion

    #endregion

}

#endregion

#region 根据应答码返回应答文本
/// <summary>
/// 根据应答码返回应答文本
/// 使用方法：初始化返回代码 KQMsg Msg = new KQMsg(); 给变量赋值string kmsg = Msg.KuaiQianMsg["0000"].ToString(); 变量值kmsg=支付成功。
/// </summary>
public class KQMsg
{
    public Hashtable KuaiQianMsg = new Hashtable();
    public KQMsg()
    {
        if (KuaiQianMsg.Count == 0)
        {
            KuaiQianMsg.Add("0000", "处理成功");
            KuaiQianMsg.Add("0001", "会员号或会员账户不能为空");
            KuaiQianMsg.Add("0002", "查询类型必须是1 或2");
            KuaiQianMsg.Add("0003", "加密方式错误，必须为1");
            KuaiQianMsg.Add("0004", "查询类型为按订单号查询时，订单号不能为空");
            KuaiQianMsg.Add("0005", "开始或结束时间格式错误");
            KuaiQianMsg.Add("0006", "查询时间必须在30 天内");
            KuaiQianMsg.Add("0007", "提交内容验签错误");
            KuaiQianMsg.Add("0008", "会员没有开通该功能");
            KuaiQianMsg.Add("0009", "未指定正确的查询条件,0,1,2");
            KuaiQianMsg.Add("0010", "开始或结束时间不能为空");
            KuaiQianMsg.Add("0011", "批次号不允许为空");
            KuaiQianMsg.Add("0012", "非法的IP 地址");
            KuaiQianMsg.Add("0013", "会员号或会员账户格式错误");
            KuaiQianMsg.Add("1000", "身份验证失败");
            KuaiQianMsg.Add("1001", "信息填写不全");
            KuaiQianMsg.Add("1002", "付款方帐户无效");
            KuaiQianMsg.Add("1003", "加密方式不正确");
            KuaiQianMsg.Add("1004", "收款方帐户无效");
            KuaiQianMsg.Add("1005", "收款方email 或mobile 格式不正确");
            KuaiQianMsg.Add("1006", "交易描述不能大于100 位");
            KuaiQianMsg.Add("1007", "收款超过限额");
            KuaiQianMsg.Add("1008", "费用大于付款金额");
            KuaiQianMsg.Add("1009", "验签错误");
            KuaiQianMsg.Add("1010", "IP 地址不符");
            KuaiQianMsg.Add("1011", "不匹配的接口类型");
            KuaiQianMsg.Add("1012", "支付金额格式不正确");
            KuaiQianMsg.Add("1014", "银行卡号不能为空");
            KuaiQianMsg.Add("1016", "不匹配的交易类型");
            KuaiQianMsg.Add("1017", "货币代码错误");
            KuaiQianMsg.Add("1018", "不能给自己付款(收款方和付款方不允许相同)");
            KuaiQianMsg.Add("1019", "查询请求为空");
            KuaiQianMsg.Add("1020", "查询类型无效");
            KuaiQianMsg.Add("1021", "订单不存在");
            KuaiQianMsg.Add("1022", "未指定查询时间范围");
            KuaiQianMsg.Add("1023", "时间格式错误");
            KuaiQianMsg.Add("1025", "商家订单必须是0-9a-zA-Z 和_的字符组合");
            KuaiQianMsg.Add("1026", "交易总笔数格式错误");
            KuaiQianMsg.Add("1027", "附言超过30个汉字");
            KuaiQianMsg.Add("1028", "单笔付款给个人用户银行卡超过限制");
            KuaiQianMsg.Add("1029", "备注长度超过限制");
            KuaiQianMsg.Add("1030", "付款方email 或mobile 格式不正确");
            KuaiQianMsg.Add("2003", "付款账户被冻结");
            KuaiQianMsg.Add("2004", "其他异常");
            KuaiQianMsg.Add("3001", "不是授权的会员");
            KuaiQianMsg.Add("3003", "匹配收款人名称");
            KuaiQianMsg.Add("3004", "不能给非快钱用户支付");
            KuaiQianMsg.Add("3005", "通知异常");
            KuaiQianMsg.Add("3006", "验签不能为空");
            KuaiQianMsg.Add("3007", "收款人户名不能为空");
            KuaiQianMsg.Add("3010", "付款超过限额");
            KuaiQianMsg.Add("3014", "单笔付款超过限额");
            KuaiQianMsg.Add("3015", "单笔收款超过限额");
            KuaiQianMsg.Add("3016", "总金额与明细中金额之和不同");
            KuaiQianMsg.Add("3017", "支付总条数与支付明细之和不同");
            KuaiQianMsg.Add("4007", "开户行不能为空");
            KuaiQianMsg.Add("4008", "省份格式不正确");
            KuaiQianMsg.Add("4009", " 城市格式不正确");
            KuaiQianMsg.Add("4013", "银行名称错误");
            KuaiQianMsg.Add("5000", "重复提交");
            KuaiQianMsg.Add("5201", "订单不能被退款");
            KuaiQianMsg.Add("5203", "不是订单的所有者");
            KuaiQianMsg.Add("5204", "超出退款时限");
            KuaiQianMsg.Add("5211", "订单号必须输入");
            KuaiQianMsg.Add("5212", "商家订单号已经存在");
            KuaiQianMsg.Add("6001", "余额不足");
            KuaiQianMsg.Add("6003", "收款账户被冻结");
            KuaiQianMsg.Add("6006", "交易引擎出错");
            KuaiQianMsg.Add("7000", "批次号必须是A-Z/0-9 和_的字符组合");
            KuaiQianMsg.Add("7001", "批次号已经存在");
            KuaiQianMsg.Add("7002", "付款会员号不能为空");
            KuaiQianMsg.Add("7003", "付款账户不能为空");
            KuaiQianMsg.Add("7004", "付款账户不存在");
            KuaiQianMsg.Add("7005", "付款账户不是人民币账户");
            KuaiQianMsg.Add("7006", "会员号和账户号不匹配");
            KuaiQianMsg.Add("7007", "不匹配的字符集");
            KuaiQianMsg.Add("7008", "主题信息验签不能为空");
            KuaiQianMsg.Add("7009", "主题信息验签错误");
            KuaiQianMsg.Add("7010", "发起方会员号不能为空");
            KuaiQianMsg.Add("7011", "发起方帐户不能为空");
            KuaiQianMsg.Add("7012", "发起方帐户不存在");
            KuaiQianMsg.Add("7013", "发起方帐户不是人民币帐户");
            KuaiQianMsg.Add("7014", "付款用户会员不在允许新业务扣款名单白名单.");
            KuaiQianMsg.Add("7015", "不能对私人付款");
            //格式错误相关
            KuaiQianMsg.Add("8801", "收款方户名错误");
            KuaiQianMsg.Add("8802", "金额输入格式错误，必须为正浮点数，请重新输入");
            KuaiQianMsg.Add("8803", "银行账号格式错误");
            KuaiQianMsg.Add("8804", "银行账号不相同，请重新输入");
            KuaiQianMsg.Add("8805", "省份输入有误！");
            KuaiQianMsg.Add("8806", "城市输入有误！");
            KuaiQianMsg.Add("8807", "银行名称输入有误！");
            KuaiQianMsg.Add("8808", "开户行名称格式错误");
            KuaiQianMsg.Add("8809", "业务参考号必须输入");
            KuaiQianMsg.Add("8810", "您输入的业务参考号已经存在");
            KuaiQianMsg.Add("8811", "业务参考号的长度不能超过30 个字符");
            KuaiQianMsg.Add("8812", "您本次上传的总记录数已经超过最大限制（6000 条）。请您重新上传！");
            KuaiQianMsg.Add("8813", "请输入附加码");
            KuaiQianMsg.Add("8814", "附加码错误，请重新输入");
            KuaiQianMsg.Add("8815", "重复银行账号");
            KuaiQianMsg.Add("8816", "两次输入的银行账号有误，请重新输入");
            KuaiQianMsg.Add("8817", "备注文本输入过长");
            KuaiQianMsg.Add("8818", "信息填写不完整，请您补充完整！");
            KuaiQianMsg.Add("8819", "银行账户信息有遗漏，请填写完整");
            KuaiQianMsg.Add("8820", "您上传的是空文件，无法支付！");
            KuaiQianMsg.Add("8821", "文件格式填写不正确，请您重新填写并上传！");
            KuaiQianMsg.Add("8822", "请输入付款密码");
            KuaiQianMsg.Add("8823", "收款方账户不能使用后缀为@99bill 的邮箱！");
            KuaiQianMsg.Add("8824", "收款方账户不符合账户格式规范，请重新输入");
            KuaiQianMsg.Add("8825", "您本次上传的总记录数已经超过最大限制（2000 条）。请您重新上传！");
            //金额检查错误相关
            KuaiQianMsg.Add("8701", "支付金额小于费用");
            KuaiQianMsg.Add("8702", "付款金额必须大于费用{0}元");
            KuaiQianMsg.Add("8703", "账户余额不足，请先充值！");
            KuaiQianMsg.Add("8704", "超过日限额");
            KuaiQianMsg.Add("8705", "账户留存金额最低为0 元，请重新输入");
            KuaiQianMsg.Add("8706", "每次提现金额最低为1000 元，请重新输入");
            KuaiQianMsg.Add("8707", "订单金额与充值金额不同不能确认");
            KuaiQianMsg.Add("8708", "付款金额不正确");
            //用户账户属性错误相关
            KuaiQianMsg.Add("8601", "您的账户已经被冻结，您暂时不能使用这个功能");
            KuaiQianMsg.Add("8602", "您的账户已经被止出，您的账户暂时不能使用这个功能");
            KuaiQianMsg.Add("8603", "您的账户不能付款，您暂时不能对此账户进行支付");
            KuaiQianMsg.Add("8604", "您的账户不能转出，您暂时不能对此账户进行支付");
            KuaiQianMsg.Add("8605", "您付款的账户已被注销，您暂时不能向此账户支付");
            KuaiQianMsg.Add("8606", "您付款的账户已被止入，您暂时不能向此账户支付");
            KuaiQianMsg.Add("8607", "您付款的账户不可转账入，您暂时不能向此账户支付");
            KuaiQianMsg.Add("8608", "您付款的账户不可收款，您暂时不能向此账户支付");
            KuaiQianMsg.Add("8609", "不能对个人付款");
            KuaiQianMsg.Add("8610", "此账户不允许提现，请换别的账户！");
            KuaiQianMsg.Add("8611", "您未开通不同名提现，不可提现");
            KuaiQianMsg.Add("8612", "该快钱账户无法收款，付款已被退回！");
            KuaiQianMsg.Add("8613", "不能自己给自己付款");
            KuaiQianMsg.Add("8614", "付款方账户不能付款");
            KuaiQianMsg.Add("8615", "收款方没有开通收款功能");
            KuaiQianMsg.Add("8616", "付款已被取消，不能取款");
            KuaiQianMsg.Add("8617", "收款方不能进行收款");
            //查询错误
            KuaiQianMsg.Add("8501", "请选择账户");
            KuaiQianMsg.Add("8502", "起始时间不应晚于结束时间");
            //其他
            KuaiQianMsg.Add("8401", "交易不存在不能确认");
            KuaiQianMsg.Add("8402", "收款方不允许收款");
            //银行交易相关
            KuaiQianMsg.Add("9101", "户名错误：户名不正确，您提供的银行卡账号和银行记录的真实姓名不符");
            KuaiQianMsg.Add("9102", "账号不正确：账户填写错误（账户/卡号不存在、账户/卡号错误，请检查账号是否填写正确）");
            KuaiQianMsg.Add("9103", "账号不正确：开户行错误，非我行账号");
            KuaiQianMsg.Add("9104", "账号不正确：账户已冻结");
            KuaiQianMsg.Add("9105", "账号不正确：账户已挂失");
            KuaiQianMsg.Add("9106", "账号不正确：账户已过期或账户已销户");
            KuaiQianMsg.Add("9107", "账号不正确：不支持向此类账户付款");
            KuaiQianMsg.Add("9108", "账号不正确：此账户为无效账户，请确认您的卡是否已作废、卡片状态是否正常");
            KuaiQianMsg.Add("9109", "账号不正确：该银行不支持向旧账号付款");
            KuaiQianMsg.Add("9110", "账号不正确：银行未查询到该客户帐户记录，付款处理失败");
            KuaiQianMsg.Add("9111", "账号不正确：该账户因长期不使用，请到银行办理相关手续");
            KuaiQianMsg.Add("9112", "帐户不正确：收款银行名称或支行信息不准确");
            KuaiQianMsg.Add("9113", "账号不正确：长期不动账账户，请到银行办理相关手续");
            KuaiQianMsg.Add("9201", "开户地错误：收款方账户与开户地区不符");
            KuaiQianMsg.Add("9202", "付款金额不能超过5万元");
            KuaiQianMsg.Add("9301", "信用卡未开卡：银行卡为信用卡或贷记卡，目前还未到银行办理开通手续，暂不能使用");
            KuaiQianMsg.Add("9302", "付款金额不能超过5 万元");
            KuaiQianMsg.Add("9401", "银行系统错误，详情请咨询快钱客服。");
            KuaiQianMsg.Add("9501", "您的账户未缴纳账户管理费，无法入帐");
            KuaiQianMsg.Add("9601", "收款帐号长度位数不正确！");
            KuaiQianMsg.Add("9999", "其它错误：当出现新的错误代码或未知的错误代码，统一设置成“其它”。");
        }
    }
}
#endregion

