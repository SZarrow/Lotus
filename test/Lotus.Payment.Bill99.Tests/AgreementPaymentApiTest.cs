using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Serialization;
using Lotus.Payment.Bill99.Domain;
using Xunit;

namespace Lotus.Payment.Bill99.Tests
{
    public class AgreementPaymentApiTest
    {
        [Fact]
        public void TestAgreementApply()
        {
            using (var handler = new HttpClientHandler())
            {
                using (var fs = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "10411004511201290.pfx")))
                {
                    Byte[] certData = new Byte[fs.Length];
                    fs.Read(certData, 0, certData.Length);
                    handler.ClientCertificates.Add(new X509Certificate2(certData, "vpos123"));
                }

                using (var client = new HttpClient(handler))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);

                    String auth = Convert.ToBase64String(Encoding.ASCII.GetBytes("104110045112012:vpos123"));
                    client.DefaultRequestHeaders.Add("Authorization", $"Basic {auth}");
                    var api = new AgreementPaymentApi(client, "104110045112012", "00002012");
                    var result = api.AgreementApply("https://sandbox.99bill.com:9445/cnp/ind_auth", new AgreementApplyRequest()
                    {
                        Version = "1.0",
                        IndAuthContent = new IndAuthContent()
                        {
                            BindType = "0",
                            CustomerId = "C0001",
                            ExternalRefNumber = "ERF0001",
                            Pan = "6217002000038983690",
                            PhoneNO = "13382185203"
                        }
                    });
                    Assert.True(result.Success);
                    Assert.NotNull(result.Value);
                }
            }
        }

        //证书建立安全通道
        public Boolean CheckValidationResult(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
    }
}
