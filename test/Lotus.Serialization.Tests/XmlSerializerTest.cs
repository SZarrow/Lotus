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

            var xs = new XmlSerializer();
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
            String input = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?><MasMessage xmlns=\"http://www.99bill.com/mas_cnp_merchant_interface\"><version>1.0</version><indAuthContent><customerId>C0001</customerId><externalRefNumber>ERF0001</externalRefNumber><pan>Pan0001</pan><phoneNO>12345678901</phoneNO><bindType>0</bindType></indAuthContent></MasMessage>";

            var xs = new XmlSerializer();
            var result = xs.Deserialize<AgreementApplyResponse>(input);

            Assert.NotNull(result);
        }
    }
}
