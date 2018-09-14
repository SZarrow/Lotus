using System;
using System.IO;
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
            var api = new AgreementPaymentApi(new System.Net.Http.HttpClient(), "aaa", "bbb");
            var result = api.AgreementApply("http://api.suziyun.com/api/rsa/genkeys", new AgreementApplyRequest()
            {
                BindType = "0",
                CustomerId = "C0001",
                ExternalRefNumber = "ERF0001",
                Pan = "Pan0001",
                PhoneNO = "12345678901",
                Version = "1.0"
            });
            Assert.True(result.Success);
        }
    }
}
