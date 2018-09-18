using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Lotus.Core;

namespace Lotus.Security
{
    public static class CryptoHelper
    {
        private static IOneWayHash _onewayhash;
        private static IRSAProvider _rsaProvider;

        static CryptoHelper()
        {
            _onewayhash = new OneWayHash();
            _rsaProvider = new RSAProvider();
        }

        public static XResult<String> GetMD5(String input)
        {
            return _onewayhash.GetMD5(input);
        }

        public static XResult<String> GetSHA1(String input)
        {
            return _onewayhash.GetSHA(input, HashAlgorithmName.SHA1);
        }

        public static XResult<String> GetSHA256(String input)
        {
            return _onewayhash.GetSHA(input, HashAlgorithmName.SHA256);
        }

        public static XResult<String> RSAEncrypt(String rawText, String publicKeyPem, String charset = "UTF-8")
        {
            return _rsaProvider.Encrypt(rawText, publicKeyPem, charset);
        }

        public static XResult<Byte[]> RSAEncrypt(Stream stream, String publicKeyPem)
        {
            return _rsaProvider.Encrypt(stream, publicKeyPem);
        }

        public static XResult<String> RSADecrypt(String encryptedString, String privateKeyPem, String charset = "UTF-8")
        {
            return _rsaProvider.Decrypt(encryptedString, privateKeyPem, charset);
        }

        public static XResult<Byte[]> RSADecrypt(Stream stream, String privateKeyPem)
        {
            return _rsaProvider.Decrypt(stream, privateKeyPem);
        }

        public static XResult<String> MakeSign(String signContent, String privateKeyPem, HashAlgorithmName algName)
        {
            if (String.IsNullOrWhiteSpace(signContent))
            {
                return new XResult<String>(null, new ArgumentNullException("signContent is null"));
            }

            if (String.IsNullOrWhiteSpace(privateKeyPem))
            {
                return new XResult<String>(null, new ArgumentNullException("privateKeyPem is null"));
            }

            return _rsaProvider.MakeSign(signContent, privateKeyPem, algName, "UTF-8");
        }

        public static XResult<Boolean> VerifySign(String signNeedToVerify, String signContent, String publicKeyPem, HashAlgorithmName algName)
        {
            if (String.IsNullOrWhiteSpace(signNeedToVerify))
            {
                return new XResult<Boolean>(false, new ArgumentNullException("signNeedToVerify is null"));
            }

            if (String.IsNullOrWhiteSpace(signContent))
            {
                return new XResult<Boolean>(false, new ArgumentNullException("signContent is null"));
            }

            if (String.IsNullOrWhiteSpace(publicKeyPem))
            {
                return new XResult<Boolean>(false, new ArgumentNullException("publicKeyPem is null"));
            }

            return _rsaProvider.VerifySign(signNeedToVerify, signContent, publicKeyPem, algName, "UTF-8");
        }
    }
}
