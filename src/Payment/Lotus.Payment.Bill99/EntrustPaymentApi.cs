using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Bill99WS;
using Lotus.Core;
using Lotus.Payment.Bill99.Domain;

namespace Lotus.Payment.Bill99
{
    /// <summary>
    /// 委托代付API
    /// </summary>
    public class EntrustPaymentApi
    {

        private String _privateKeyFilePath;
        private String _publicKeyFilePath;

        /// <summary>
        /// 初始化EntrustPaymentApi类的实例
        /// </summary>
        /// <param name="membercode">商户在快钱的会员编号</param>
        /// <param name="merchantAcctId">商户在快钱的收款结算账号</param>
        /// <param name="contracted">商户与快钱签订的委托代收合同号</param>
        /// <param name="privateKeyFilePath">商户私钥证书路径</param>
        /// <param name="publicKeyFilePath">快钱公钥证书路径</param>
        public EntrustPaymentApi(String membercode, String merchantAcctId, String contracted, String privateKeyFilePath, String publicKeyFilePath)
        {
            if (String.IsNullOrWhiteSpace(membercode))
            {
                throw new ArgumentNullException(nameof(membercode));
            }

            if (String.IsNullOrWhiteSpace(merchantAcctId))
            {
                throw new ArgumentNullException(nameof(merchantAcctId));
            }

            if (String.IsNullOrWhiteSpace(contracted))
            {
                throw new ArgumentNullException(nameof(contracted));
            }

            if (String.IsNullOrWhiteSpace(privateKeyFilePath))
            {
                throw new ArgumentNullException(nameof(privateKeyFilePath));
            }

            if (String.IsNullOrWhiteSpace(publicKeyFilePath))
            {
                throw new ArgumentNullException(nameof(publicKeyFilePath));
            }

            this.Membercode = membercode;
            this.MerchantAcctId = merchantAcctId;
            this.Contracted = contracted;
            _privateKeyFilePath = privateKeyFilePath;
            _publicKeyFilePath = publicKeyFilePath;
        }

        /// <summary>
        /// 获取商户在快钱的会员编号
        /// </summary>
        public String Membercode { get; }
        /// <summary>
        /// 获取商户在快钱的收款结算账号
        /// </summary>
        public String MerchantAcctId { get; }
        /// <summary>
        /// 获取商户与快钱签订的委托代收合同号
        /// </summary>
        public String Contracted { get; }

        public XResult<EntrustPayResponse> BatchPay(IEnumerable<EntrustPayAccount> accounts)
        {
            if (accounts == null || accounts.Count() == 0)
            {
                return new XResult<EntrustPayResponse>(null, new ArgumentNullException(nameof(accounts)));
            }

            version ver = new version
            {
                service = "ddp.product.debit",
                version1 = "1"
            };

            merchantDebitHead requestHead = new merchantDebitHead
            {
                version = ver
            };

            Decimal totalAmount = accounts.Sum(x => x.Amount);
            DateTime now = DateTime.Now;

            //提交数据xml报文
            StringBuilder sb = new StringBuilder();
            sb.Append("<tns:merchant-debit-request xmlns:ns0=\"http://www.99bill.com/schema/ddp/product/head\" xmlns:ns1=\"http://www.99bill.com/schema/ddp/product/pki\" xmlns:ns2=\"http://www.99bill.com/schema/commons\" xmlns:tns=\"http://www.99bill.com/schema/ddp/product\">");
            sb.Append("<tns:inputCharset>1</tns:inputCharset>");
            sb.Append("<tns:bgUrl>http://www.99bill.com</tns:bgUrl>");
            sb.Append($"<tns:memberCode>{this.Membercode}</tns:memberCode>");
            sb.Append($"<tns:merchantAcctId>{this.MerchantAcctId}</tns:merchantAcctId>");
            sb.Append($"<tns:contractId>{this.Contracted}</tns:contractId>");
            sb.Append("<tns:requestId>" + now.ToString("yyyyMMddHHmmss") + "</tns:requestId>");
            sb.Append("<tns:requestTime>" + now.ToString("yyyyMMddHHmmss") + "</tns:requestTime>");
            sb.Append("<tns:numTotal>1</tns:numTotal>");
            sb.Append($"<tns:amountTotal>{totalAmount}</tns:amountTotal>");
            sb.Append("<tns:ext1/>");
            sb.Append("<tns:ext2/>");

            foreach (var payAccount in accounts)
            {
                sb.Append(payAccount.ToString());
            }

            sb.Append("</tns:merchant-debit-request>");

            //string prikey_path = HttpContext.Current.Server.MapPath("") + "\\certificate\\" + "tester-rsa.pfx";//商户私钥证书路径
            //string pubkey_path = HttpContext.Current.Server.MapPath("") + "\\certificate\\" + "99bill.cert.rsa.20340630_sandbox.cer";//快钱公钥证书路径
            String priPW = "123456";
            String pubPW = String.Empty;//快钱公钥密码

            //获取随机KEY
            Byte[] enKey = bigpay.randomKey(2);
            //selDataXml经过utf-8转码
            Byte[] byteorg = bigpay.CodingToByte(sb.ToString(), 2);//UTF-8编码

            //引用证书对数据进行加密 byteorg
            Byte[] sigData = bigpay.CerRSASignature(byteorg, _privateKeyFilePath, priPW, 2);
            Byte[] toEnData = bigpay.SymmetryEncryptType(byteorg, enKey, 1);
            Byte[] digData = bigpay.CerRSAEncrypt(enKey, _publicKeyFilePath, pubPW);

            //zip
            Byte[] zipSigData = bigpay.CompressGZipExt(sigData);
            Byte[] ziptoEnData = bigpay.CompressGZipExt(toEnData);
            Byte[] zipdigData = bigpay.CompressGZipExt(digData);

            sealDataType sealData = new sealDataType
            {
                digitalenvelope = bigpay.CodingToString(zipdigData, 1),//数字信封
                encrypteddata = bigpay.CodingToString(ziptoEnData, 1),//加密数据
                originaldata = String.Empty,//xml文
                signeddata = bigpay.CodingToString(zipSigData, 1)//签名数据
            };

            merchantDebitPkiRequestBody requestBody = new merchantDebitPkiRequestBody
            {
                membercode = this.Membercode,
                data = sealData
            };


            merchantdebitpkirequest pkiRequest = new merchantdebitpkirequest
            {
                head = requestHead,
                body = requestBody
            };

            MerchantDebitPkiClient service = new MerchantDebitPkiClient();

            ServicePointManager.ServerCertificateValidationCallback += RemoteCertificateCallback;

            var pkiResponse = service.merchantdebitpkiAsync(pkiRequest).GetAwaiter().GetResult().merchantdebitpkiresponse;
            merchantDebitPkiResponseBody responseBody = pkiResponse.body;
            String backmembercode = responseBody.membercode;
            String backstatus = responseBody.status;             //返回的整批次提交处理状态
            String backerrorcode = responseBody.errorcode;       //返回的总订单的错误代码
            String backerrormsg = responseBody.errormsg;         //返回的总订单的错误信息

            //Response.Write("backmembercode:" + backmembercode);

            sealDataType responseSealDataType = responseBody.data;
            String backsigneddata = responseSealDataType.signeddata;
            String backoriginaldata = responseSealDataType.originaldata;
            String backdigitalenvelope = responseSealDataType.digitalenvelope;
            String backencrypteddata = responseSealDataType.encrypteddata;

            String backsignedXmldata = bigpay.DecompressGZip(backsigneddata, 1);
            String backoriginalXmldata = bigpay.DecompressGZip(backoriginaldata, 1);
            String backdigitalenvelopeXmldata = bigpay.DecompressGZip(backdigitalenvelope, 1);
            String backencryptedXmldata = bigpay.DecompressGZip(backencrypteddata, 1);

            Byte[] byteorgdata = null;//返回的xml结果数据
            Byte[] bytesigndata = null;//返回的签名
            Byte[] bytebackkey = null;//返回的对称算法密钥
            try
            {
                if (!String.IsNullOrWhiteSpace(backsignedXmldata))
                {
                    bytesigndata = bigpay.CodingToByte(backsignedXmldata, 1);//将获取的string签名转成byte
                }

                if (!String.IsNullOrWhiteSpace(backdigitalenvelopeXmldata))
                {
                    bytebackkey = bigpay.CerRSADecrypt(bigpay.CodingToByte(backdigitalenvelopeXmldata, 1), _privateKeyFilePath, priPW);
                }

                if (!String.IsNullOrWhiteSpace(backencryptedXmldata))
                {
                    byteorgdata = bigpay.SymmetryDecryptType(bigpay.CodingToByte(backencryptedXmldata, 1), bytebackkey, 1);
                }
            }
            catch
            {
                backerrormsg = "解密失败请检查程序。";
            }

            if (bigpay.CerRSAVerifySignature(byteorgdata, bytesigndata, _publicKeyFilePath, pubPW, 2))//SHA1
            {
                String orgxmldata = bigpay.CodingToString(byteorgdata, 2);//将返回的数据解压缩获取xml
            }
        }

        public static Boolean RemoteCertificateCallback(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
