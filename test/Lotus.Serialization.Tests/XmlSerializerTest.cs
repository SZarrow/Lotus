using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml.Linq;
using Lotus.Serialization.Tests.Models;
using Xunit;

namespace Lotus.Serialization.Tests
{
    public class XmlSerializerTest
    {
        [Fact]
        public void TestSerialize()
        {
            var model = new AgreementApplyRequest()
            {
                BindType = "0",
                CustomerId = "C0001",
                ExternalRefNumber = "ERF0001",
                Pan = "Pan0001",
                PhoneNO = "12345678901",
                Version = "1.0"
            };

            var xs = new XSerializer();
            var xml = xs.Serialize(model);

            XDocument doc = null;
            try
            {
                doc = XDocument.Parse(xml);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }

            Assert.NotNull(doc);
        }

        [Fact]
        public void TestDeserialize()
        {
            String input = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><MasMessage xmlns=\"http://www.99bill.com/mas_cnp_merchant_interface\"><version>1.0</version><indAuthContent><merchantId>104110045112012</merchantId><terminalId>00002012</terminalId><customerId>C0001</customerId><externalRefNumber>ERF0001</externalRefNumber><storablePan>6217003690</storablePan><token>9001223771</token><responseCode>00</responseCode><responseTextMessage>交易成功</responseTextMessage></indAuthContent></MasMessage>";

            var xs = new XSerializer();
            var result = xs.Deserialize<AgreementApplyResponse>(input);

            Assert.NotNull(result);
        }
    }
}
