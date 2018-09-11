using System;
using Lotus.Payment.Bill99.Domain;
using Xunit;

namespace Lotus.Payment.Bill99.Tests
{
    public class AgreementPaymentApiTest
    {
        [Fact]
        public void TestAgreementApply()
        {
            AgreementPaymentApi.AgreementApply(new AgreementApplyRequest()
            {

            });
        }
    }
}
