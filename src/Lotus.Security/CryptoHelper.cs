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

        static CryptoHelper()
        {
            _onewayhash = new OneWayHash();
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
    }
}
