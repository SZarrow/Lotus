﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lotus.CommonService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RSAController : ControllerBase
    {
        [HttpPost]
        public ActionResult GenKeyPairs()
        {
            String id = Guid.NewGuid().ToString("n");
            String privateKeyPemFileName = $"{id}_priv.pem";
            String publicKeyPemFileName = $"{id}_pub.pem";

            String cacheDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache");
            if (!Directory.Exists(cacheDir))
            {
                try
                {
                    Directory.CreateDirectory(cacheDir);
                }
                catch (Exception ex)
                {
                    return new JsonResult(ex);
                }
            }

            String privateKeySavePath = Path.Combine(cacheDir, privateKeyPemFileName);
            try
            {
                var p = Process.Start("openssl", $"genrsa -out {privateKeySavePath} 1024");
                p.WaitForExit();
            }
            catch (Exception ex)
            {
                return this.FormatJson(null, privateKeySavePath + "|" + ex.Message);
            }

            if (!System.IO.File.Exists(privateKeySavePath))
            {
                return this.FormatJson(null, $"File '{privateKeySavePath}' not found.");
            }

            String privateKey = null;
            try
            {
                var lines = System.IO.File.ReadAllLines(privateKeySavePath);
                lines[0] = String.Empty;
                lines[lines.Length - 1] = String.Empty;
                privateKey = String.Concat(lines);
            }
            catch (Exception ex)
            {
                return this.FormatJson(null, ex.Message);
            }

            String publicKeySavePath = Path.Combine(cacheDir, publicKeyPemFileName);
            try
            {
                var p = Process.Start("openssl", $"rsa -pubout -in {privateKeySavePath} -out {publicKeySavePath}");
                p.WaitForExit();
            }
            catch (Exception ex)
            {
                return this.FormatJson(null, ex.Message);
            }

            if (!System.IO.File.Exists(publicKeySavePath))
            {
                return this.FormatJson(null, $"File '{publicKeySavePath}' not found.");
            }

            String publicKey = null;
            try
            {
                var lines = System.IO.File.ReadAllLines(publicKeySavePath);
                lines[0] = String.Empty;
                lines[lines.Length - 1] = String.Empty;
                publicKey = String.Concat(lines);
            }
            catch (Exception ex)
            {
                return this.FormatJson(null, ex.Message);
            }

            try
            {
                System.IO.File.Delete(privateKeySavePath);
                System.IO.File.Delete(publicKeySavePath);

                return this.FormatJson(new
                {
                    PrivateKey = privateKey,
                    PublicKey = publicKey
                });
            }
            catch (Exception ex)
            {
                return this.FormatJson(null, ex.Message);
            }
        }
    }
}