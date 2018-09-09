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
            String cacheDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"cache");
            if (!Directory.Exists(cacheDir)) {
                try
                {
                    Directory.CreateDirectory(cacheDir);
                }
                catch (Exception ex)
                {
                    return new JsonResult(ex);
                }
            }
            var savePath = Path.Combine(cacheDir,id+".pem");
           var p = Process.Start("openssl", $"genrsa -out {savePath} 1024");
            p.WaitForExit();

            if (!System.IO.File.Exists(savePath)) {
                return new JsonResult(new { error = "" });
            }

            String privateKey = System.IO.File.ReadAllText(savePath);

            return new JsonResult(new { });
        }
    }
}