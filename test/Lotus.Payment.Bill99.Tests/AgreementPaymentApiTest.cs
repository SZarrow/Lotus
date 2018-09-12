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
                 
            });
            Assert.True(result.Success);
        }
    }
}
