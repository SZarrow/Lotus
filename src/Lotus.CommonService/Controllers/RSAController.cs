using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lotus.CommonService.Controllers
{
    [Route("api/[controller]")]
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
                return this.FormatJson(null, 10001, ex.Message);
            }

            if (!System.IO.File.Exists(privateKeySavePath))
            {
                return this.FormatJson(null, 10001, $"File '{privateKeySavePath}' not found.");
            }

            String privateKey = null;
            try
            {
                privateKey = System.IO.File.ReadAllText(privateKeySavePath);
            }
            catch (Exception ex)
            {
                return this.FormatJson(null, 10002, ex.Message);
            }

            String publicKeySavePath = Path.Combine(cacheDir, publicKeyPemFileName);
            try
            {
                var p = Process.Start("openssl", $"rsa -pubout -in {privateKeySavePath} -out {publicKeySavePath}");
                p.WaitForExit();
            }
            catch (Exception ex)
            {
                return this.FormatJson(null, 10003, ex.Message);
            }

            if (!System.IO.File.Exists(publicKeySavePath))
            {
                return this.FormatJson(null, 10001, $"File '{publicKeySavePath}' not found.");
            }

            String publicKey = null;
            try
            {
                publicKey = System.IO.File.ReadAllText(publicKeySavePath);
            }
            catch (Exception ex)
            {
                return this.FormatJson(null, 10002, ex.Message);
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
                return this.FormatJson(null, 10003, ex.Message);
            }
        }
    }
}