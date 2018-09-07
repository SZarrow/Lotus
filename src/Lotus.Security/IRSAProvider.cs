﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Lotus.Core;

namespace Lotus.Security
{
    public interface IRSAProvider
    {
        XResult<String> Encrypt(String rawText, String publicKeyPem, String charset);
        XResult<Byte[]> Encrypt(Stream stream, String publicKeyPem);
        XResult<String> Decrypt(String encryptedString, String privateKeyPem, String charset);
        XResult<Byte[]> Decrypt(Stream stream, String privateKeyPem);
        XResult<KeyValuePair<String, String>> MakeRSAKeyPairs();
        XResult<String> MakeSign(String signContent, String privateKeyPem, HashAlgorithmName algName, String charset);
        XResult<Boolean> VerifySign(String signNeedToVerify, String signContent, String publicKeyPem, HashAlgorithmName algName, String charset);
    }
}
