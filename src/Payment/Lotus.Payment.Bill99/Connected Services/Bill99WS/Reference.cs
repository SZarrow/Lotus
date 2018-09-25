//------------------------------------------------------------------------------
// <自动生成>
//     此代码由工具生成。
//     //
//     对此文件的更改可能导致不正确的行为，并在以下条件下丢失:
//     代码重新生成。
// </自动生成>
//------------------------------------------------------------------------------

namespace Bill99WS
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://www.99bill.com/schema/ddp/product", ConfigurationName="Bill99WS.MerchantDebitPki")]
    public interface MerchantDebitPki
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<Bill99WS.merchantdebitpkiresponse1> merchantdebitpkiAsync(Bill99WS.merchantdebitpkirequest1 request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.99bill.com/schema/ddp/product")]
    public partial class merchantdebitpkirequest
    {
        
        private merchantDebitHead headField;
        
        private merchantDebitPkiRequestBody bodyField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public merchantDebitHead head
        {
            get
            {
                return this.headField;
            }
            set
            {
                this.headField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public merchantDebitPkiRequestBody body
        {
            get
            {
                return this.bodyField;
            }
            set
            {
                this.bodyField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.99bill.com/schema/ddp/product")]
    public partial class merchantDebitHead
    {
        
        private string authenticationInfoField;
        
        private string serviceField;
        
        private version versionField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string authenticationInfo
        {
            get
            {
                return this.authenticationInfoField;
            }
            set
            {
                this.authenticationInfoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string service
        {
            get
            {
                return this.serviceField;
            }
            set
            {
                this.serviceField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public version version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.99bill.com/schema/ddp/product")]
    public partial class version
    {
        
        private string version1Field;
        
        private string serviceField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("version", Order=0)]
        public string version1
        {
            get
            {
                return this.version1Field;
            }
            set
            {
                this.version1Field = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string service
        {
            get
            {
                return this.serviceField;
            }
            set
            {
                this.serviceField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.99bill.com/schema/ddp/product")]
    public partial class merchantDebitPkiResponseBody
    {
        
        private string membercodeField;
        
        private string statusField;
        
        private string errorcodeField;
        
        private string errormsgField;
        
        private sealDataType dataField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("member-code")]
        public string membercode
        {
            get
            {
                return this.membercodeField;
            }
            set
            {
                this.membercodeField = value;
            }
        }
        
        /// <remarks/>
        public string status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("error-code")]
        public string errorcode
        {
            get
            {
                return this.errorcodeField;
            }
            set
            {
                this.errorcodeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("error-msg")]
        public string errormsg
        {
            get
            {
                return this.errormsgField;
            }
            set
            {
                this.errormsgField = value;
            }
        }
        
        /// <remarks/>
        public sealDataType data
        {
            get
            {
                return this.dataField;
            }
            set
            {
                this.dataField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.99bill.com/schema/ddp/product")]
    public partial class sealDataType
    {
        
        private string originaldataField;
        
        private string signeddataField;
        
        private string encrypteddataField;
        
        private string digitalenvelopeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("original-data")]
        public string originaldata
        {
            get
            {
                return this.originaldataField;
            }
            set
            {
                this.originaldataField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("signed-data")]
        public string signeddata
        {
            get
            {
                return this.signeddataField;
            }
            set
            {
                this.signeddataField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("encrypted-data")]
        public string encrypteddata
        {
            get
            {
                return this.encrypteddataField;
            }
            set
            {
                this.encrypteddataField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("digital-envelope")]
        public string digitalenvelope
        {
            get
            {
                return this.digitalenvelopeField;
            }
            set
            {
                this.digitalenvelopeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.99bill.com/schema/ddp/product")]
    public partial class merchantDebitPkiRequestBody
    {
        
        private string membercodeField;
        
        private sealDataType dataField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("member-code")]
        public string membercode
        {
            get
            {
                return this.membercodeField;
            }
            set
            {
                this.membercodeField = value;
            }
        }
        
        /// <remarks/>
        public sealDataType data
        {
            get
            {
                return this.dataField;
            }
            set
            {
                this.dataField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.99bill.com/schema/ddp/product")]
    public partial class merchantdebitpkiresponse
    {
        
        private merchantDebitHead headField;
        
        private merchantDebitPkiResponseBody bodyField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public merchantDebitHead head
        {
            get
            {
                return this.headField;
            }
            set
            {
                this.headField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public merchantDebitPkiResponseBody body
        {
            get
            {
                return this.bodyField;
            }
            set
            {
                this.bodyField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class merchantdebitpkirequest1
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="merchant-debit-pki-request", Namespace="http://www.99bill.com/schema/ddp/product", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute("merchant-debit-pki-request")]
        public Bill99WS.merchantdebitpkirequest merchantdebitpkirequest;
        
        public merchantdebitpkirequest1()
        {
        }
        
        public merchantdebitpkirequest1(Bill99WS.merchantdebitpkirequest merchantdebitpkirequest)
        {
            this.merchantdebitpkirequest = merchantdebitpkirequest;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class merchantdebitpkiresponse1
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="merchant-debit-pki-response", Namespace="http://www.99bill.com/schema/ddp/product", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute("merchant-debit-pki-response")]
        public Bill99WS.merchantdebitpkiresponse merchantdebitpkiresponse;
        
        public merchantdebitpkiresponse1()
        {
        }
        
        public merchantdebitpkiresponse1(Bill99WS.merchantdebitpkiresponse merchantdebitpkiresponse)
        {
            this.merchantdebitpkiresponse = merchantdebitpkiresponse;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    public interface MerchantDebitPkiChannel : Bill99WS.MerchantDebitPki, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    public partial class MerchantDebitPkiClient : System.ServiceModel.ClientBase<Bill99WS.MerchantDebitPki>, Bill99WS.MerchantDebitPki
    {
        
    /// <summary>
    /// 实现此分部方法，配置服务终结点。
    /// </summary>
    /// <param name="serviceEndpoint">要配置的终结点</param>
    /// <param name="clientCredentials">客户端凭据</param>
    static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public MerchantDebitPkiClient() : 
                base(MerchantDebitPkiClient.GetDefaultBinding(), MerchantDebitPkiClient.GetDefaultEndpointAddress())
        {
            this.Endpoint.Name = EndpointConfiguration.MerchantDebitPkiSoap11.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public MerchantDebitPkiClient(EndpointConfiguration endpointConfiguration) : 
                base(MerchantDebitPkiClient.GetBindingForEndpoint(endpointConfiguration), MerchantDebitPkiClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public MerchantDebitPkiClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(MerchantDebitPkiClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public MerchantDebitPkiClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(MerchantDebitPkiClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public MerchantDebitPkiClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<Bill99WS.merchantdebitpkiresponse1> Bill99WS.MerchantDebitPki.merchantdebitpkiAsync(Bill99WS.merchantdebitpkirequest1 request)
        {
            return base.Channel.merchantdebitpkiAsync(request);
        }
        
        public System.Threading.Tasks.Task<Bill99WS.merchantdebitpkiresponse1> merchantdebitpkiAsync(Bill99WS.merchantdebitpkirequest merchantdebitpkirequest)
        {
            Bill99WS.merchantdebitpkirequest1 inValue = new Bill99WS.merchantdebitpkirequest1();
            inValue.merchantdebitpkirequest = merchantdebitpkirequest;
            return ((Bill99WS.MerchantDebitPki)(this)).merchantdebitpkiAsync(inValue);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        public virtual System.Threading.Tasks.Task CloseAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndClose));
        }
        
        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.MerchantDebitPkiSoap11))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                return result;
            }
            throw new System.InvalidOperationException(string.Format("找不到名称为“{0}”的终结点。", endpointConfiguration));
        }
        
        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.MerchantDebitPkiSoap11))
            {
                return new System.ServiceModel.EndpointAddress("http://sandbox.99bill.com/ddpproduct/services/");
            }
            throw new System.InvalidOperationException(string.Format("找不到名称为“{0}”的终结点。", endpointConfiguration));
        }
        
        private static System.ServiceModel.Channels.Binding GetDefaultBinding()
        {
            return MerchantDebitPkiClient.GetBindingForEndpoint(EndpointConfiguration.MerchantDebitPkiSoap11);
        }
        
        private static System.ServiceModel.EndpointAddress GetDefaultEndpointAddress()
        {
            return MerchantDebitPkiClient.GetEndpointAddress(EndpointConfiguration.MerchantDebitPkiSoap11);
        }
        
        public enum EndpointConfiguration
        {
            
            MerchantDebitPkiSoap11,
        }
    }
}
