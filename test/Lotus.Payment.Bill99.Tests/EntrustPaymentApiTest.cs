using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Lotus.Payment.Bill99.Domain;
using Newtonsoft.Json;
using Xunit;

namespace Lotus.Payment.Bill99.Tests
{
    public class EntrustPaymentApiTest
    {
        [Fact]
        public void TestEntrustPay()
        {
            var api = CreateEntrustPaymentApi();

            String url = "https://sandbox.99bill.com:9445/cnp/purchase";
            url = "https://mas.99bill.com/cnp/purchase";

            var extDates = new ExtDate[] { new ExtDate() { Key = "phone", Value = "13382185203" } };

            var result = api.EntrustPay(url, new EntrustPayRequest()
            {
                Version = "1.0",
                EntrustPayRequestContent = new EntrustPayRequestContent()
                {
                    Amount = "0.01",
                    CardHolderId = "320503199107041753",
                    CardHolderName = "范波",
                    CardNo = "6217002000038983690",
                    EntryTime = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    ExternalRefNumber = "ERF0001",
                    IdType = "0",
                    InteractiveStatus = "TR1",
                    TxnType = "PUR",
                    ExtMap = new ExtMap()
                    {
                        ExtDates = extDates
                    }
                }
            });

            Assert.True(result.Success);
            Assert.NotNull(result.Value);

            WriteLog("TestEntrustPay()返回：" + JsonConvert.SerializeObject(result.Value));
        }

        [Fact]
        public void TestEntrustQuery()
        {
            var api = CreateEntrustPaymentApi();

            String url = "https://sandbox.99bill.com:9445/cnp/query_txn";
            url = "https://mas.99bill.com/cnp/query_txn";

            var result = api.EntrustQuery(url, new EntrustQueryRequest()
            {
                Version = "1.0",
                EntrustQueryRequestContent = new EntrustQueryRequestContent()
                {
                    ExternalRefNumber = "ERF0001",
                    TxnType = "PUR"
                }
            });

            Assert.True(result.Success);
            Assert.NotNull(result.Value);

            WriteLog("TestEntrustQuery()返回：" + JsonConvert.SerializeObject(result.Value));
        }

        private EntrustPaymentApi CreateEntrustPaymentApi()
        {
            var handler = new HttpClientHandler();
            using (var fs = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "812310060510214.pfx")))
            {
                Byte[] certData = new Byte[fs.Length];
                fs.Read(certData, 0, certData.Length);
                handler.ClientCertificates.Add(new X509Certificate2(certData, "vpos123"));
            }

            var client = new HttpClient(handler);

            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);

            String auth = Convert.ToBase64String(Encoding.ASCII.GetBytes("812310060510214:vpos123"));
            client.DefaultRequestHeaders.Add("Authorization", $"Basic {auth}");
            return new EntrustPaymentApi(client, "812310060510214", "00007302");
        }

        private void WriteLog(String content)
        {
            String logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"log\{DateTime.Now.ToString("yyyy-MM-dd")}.txt");
            File.AppendAllText(logFile, Environment.NewLine + Environment.NewLine + content);
        }

        public Boolean CheckValidationResult(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
    }
}
