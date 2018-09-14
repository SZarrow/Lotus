using System;
using System.IO;
using System.Net.Http;
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
                handler.ClientCertificates.Add(new X509Certificate2("~/10411004511201290.pfx", "vpos123"));
                using (var client = new HttpClient(handler))
                {
                    String auth = "";
                    client.DefaultRequestHeaders.Add("Authorization", $"Basic {auth}");
                    var api = new AgreementPaymentApi(client, "812310060510214", "13196988");
                    var result = api.AgreementApply("https://sandbox.99bill.com:9445/cnp/ind_auth", new AgreementApplyRequest()
                    {
                        BindType = "0",
                        CustomerId = "C0001",
                        ExternalRefNumber = "ERF0001",
                        Pan = "Pan0001",
                        PhoneNO = "13382185203",
                        Version = "1.0"
                    });
                    Assert.True(result.Success);
                    Assert.NotNull(result.Value);
                }
            }
        }
    }
}
