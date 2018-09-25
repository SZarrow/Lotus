﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Serialization;
using Lotus.Logging;
using Lotus.Payment.Bill99.Domain;
using Newtonsoft.Json;
using Xunit;

namespace Lotus.Payment.Bill99.Tests
{
    public class AgreementPaymentApiTest
    {
        private const Boolean IsTest = false;

        [Fact]
        public void TestAgreementApply()
        {
            var api = CreateAgreementPaymentApi();

            String url = "https://sandbox.99bill.com:9445/cnp/ind_auth";

            if (!IsTest)
            {
                url = "https://mas.99bill.com/cnp/ind_auth";
            }

            var result = api.AgreementApply(url, new AgreementApplyRequest()
            {
                Version = "1.0",
                IndAuthContent = new IndAuthRequestContent()
                {
                    BindType = "0",
                    CustomerId = "C0001",
                    ExternalRefNumber = "ERF0001",
                    Pan = "6217002000038983690",
                    CardHolderName = "范波",
                    CardHolderId = "320503199107041753",
                    PhoneNO = "13382185203"
                }
            });

            WriteLog("TestAgreementApply()返回：" + JsonConvert.SerializeObject(result.Value));

            Assert.True(result.Success);
            Assert.NotNull(result.Value);
        }

        [Fact]
        public void TestAgreementBind()
        {
            var api = CreateAgreementPaymentApi();

            String url = "https://sandbox.99bill.com:9445/cnp/ind_auth_verify";

            if (!IsTest)
            {
                url = "https://mas.99bill.com/cnp/ind_auth_verify";
            }

            var result = api.AgreementVerify(url, new AgreementBindRequest()
            {
                Version = "1.0",
                IndAuthDynVerifyContent = new IndAuthDynVerifyRequestContent()
                {
                    BindType = "0",
                    CustomerId = "C0001",
                    ExternalRefNumber = "ERF0001",
                    Pan = "6217002000038983690",
                    PhoneNO = "13382185203",
                    Token = "9001262281",
                    ValidCode = "526702"
                }
            });

            WriteLog("TestAgreementBind()返回：" + JsonConvert.SerializeObject(result.Value));

            Assert.True(result.Success);
            Assert.NotNull(result.Value);
        }

        [Fact]
        public void TestAgreementPay()
        {
            var extDates = new List<ExtDate>(2);
            extDates.Add(new ExtDate()
            {
                Key = "phone",
                Value = "13382185203"
            });
            extDates.Add(new ExtDate()
            {
                Key = "validCode",
                Value = "526702"
            });

            var api = CreateAgreementPaymentApi();

            String url = "https://sandbox.99bill.com:9445/cnp/purchase";

            if (!IsTest)
            {
                url = "https://mas.99bill.com/cnp/purchase";
            }

            var result = api.AgreementPay(url, new AgreementPayRequest()
            {
                Version = "1.0",
                TxnMsgContent = new TxnMsgRequestContent()
                {
                    Amount = "0.01",
                    CustomerId = "C0001",
                    EntryTime = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    ExternalRefNumber = "ERF0001",
                    InteractiveStatus = "TR1",
                    NotifyUrl = "http://www.baidu.com",
                    PayToken = "8120000000000567527",
                    SpFlag = "QPay02",
                    TxnType = "PUR",
                    ExtMap = new ExtMap()
                    {
                        ExtDates = extDates
                    }
                }
            });

            WriteLog("TestAgreementPay()返回：" + JsonConvert.SerializeObject(result.Value));

            Assert.True(result.Success);
            Assert.NotNull(result.Value);
        }

        [Fact]
        public void TestAgreementQuery()
        {
            var api = CreateAgreementPaymentApi();

            String url = "https://sandbox.99bill.com:9445/cnp/query_txn";

            if (!IsTest)
            {
                url = "https://mas.99bill.com/cnp/query_txn";
            }

            var result = api.AgreementQuery(url, new AgreementQueryRequest()
            {
                Version = "1.0",
                QryTxnMsgContent = new QryTxnMsgRequestContent()
                {
                    ExternalRefNumber = "ERF0001",
                    TxnType = "PUR"
                }
            });

            WriteLog("TestAgreementQuery()返回：" + JsonConvert.SerializeObject(result.Value));

            Assert.True(result.Success);
            Assert.NotNull(result.Value);
        }

        //证书建立安全通道
        public Boolean CheckValidationResult(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        private AgreementPaymentApi CreateAgreementPaymentApi()
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

            String auth = Convert.ToBase64String(Encoding.ASCII.GetBytes("812310060510212:vpos123"));
            client.DefaultRequestHeaders.Add("Authorization", $"Basic {auth}");
            return new AgreementPaymentApi(client, "812310060510212", "13196988");
        }

        private void WriteLog(String content)
        {
            String logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"log\{DateTime.Now.ToString("yyyy-MM-dd")}.txt");
            File.AppendAllText(logFile, Environment.NewLine + Environment.NewLine + content);
        }
    }
}
