using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml.Linq;
using Lotus.Payment.Bill99.Domain;
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
                Version = "1.0",
                IndAuthContent = new IndAuthRequestContent()
                {
                    BindType = "0",
                    CustomerId = "C0001",
                    ExternalRefNumber = "ERF0001",
                    Pan = "Pan0001",
                    PhoneNO = "12345678901"
                }
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
        public void TestSerializeCollection()
        {
            var extDates = new List<ExtDate>(2);
            extDates.Add(new ExtDate()
            {
                Key = "phone",
                Value = "12345678901"
            });
            extDates.Add(new ExtDate()
            {
                Key = "validCode",
                Value = "352623"
            });

            var model = new AgreementPayRequest()
            {
                Version = "1.0",
                TxnMsgContent = new TxnMsgRequestContent()
                {
                    Amount = "12.34",
                    CustomerId = "C0001",
                    EntryTime = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    ExternalRefNumber = "ERF0001",
                    InteractiveStatus = "TR1",
                    NotifyUrl = "http://www.baidu.com",
                    PayToken = "8120000000000001717",
                    SpFlag = "QPay02",
                    TxnType = "PUR",
                    ExtMap = new ExtMap()
                    {
                        ExtDates = extDates
                    }
                }
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
            String input = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><MasMessage xmlns=\"http://www.99bill.com/mas_cnp_merchant_interface\"><version>1.0</version><ErrorMsgContent><errorCode>10001</errorCode><errorMessage>参数错误</errorMessage></ErrorMsgContent><indAuthContent><merchantId>104110045112012</merchantId><terminalId>00002012</terminalId><customerId>C0001</customerId><externalRefNumber>ERF0001</externalRefNumber><storablePan>6217003690</storablePan><token>9001223771</token><responseCode>00</responseCode><responseTextMessage>交易成功</responseTextMessage></indAuthContent></MasMessage>";

            var xs = new XSerializer();
            var result = xs.Deserialize<AgreementApplyResponse>(input);

            Assert.NotNull(result);
        }

        [Fact]
        public void TestDeserializeCollection()
        {
            String input = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?><MasMessage xmlns=\"http://www.99bill.com/mas_cnp_merchant_interface\"><version>1.0</version><TxnMsgContent><txnType>PUR</txnType><interactiveStatus>TR1</interactiveStatus><entryTime>20180920092713</entryTime><externalRefNumber>ERF0001</externalRefNumber><amount>12.34</amount><spFlag>QPay02</spFlag><customerId>C0001</customerId><payToken>8120000000000001717</payToken><tr3Url>http://www.baidu.com</tr3Url><extMap><extDate><key>validCode</key><value>352623</value></extDate><extDate><key>phone</key><value>12345678901</value></extDate></extMap></TxnMsgContent></MasMessage>";

            var xs = new XSerializer();
            var result = xs.Deserialize<AgreementPayRequest>(input);

            Assert.NotNull(result);
        }
    }
}
