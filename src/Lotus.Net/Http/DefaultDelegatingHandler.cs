using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Lotus.Net.Http
{
    internal class DefaultDelegatingHandler : DelegatingHandler
    {
        public DefaultDelegatingHandler(HttpClientHandler handler) : base(handler) { }
    }
}
