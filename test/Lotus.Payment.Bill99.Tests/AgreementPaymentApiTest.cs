using System;
using Lotus.Payment.Bill99.Domain;
using Xunit;

namespace Lotus.Payment.Bill99.Tests
{
    public class AgreementPaymentApiTest
    {
        [Fact]
        public async void TestAgreementApply()
        {
            var api = new AgreementPaymentApi("aaa", "bbb");
            await api.AgreementApply(new AgreementApplyRequest()
            {

            });
        }
    }
}
