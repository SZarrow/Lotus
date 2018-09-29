using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Lotus.Net.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HttpClientServiceCollectionExtensions
    {
        public static IHttpClientBuilder AddHttpClientHandler(this IHttpClientBuilder builder, Action<HttpClientHandler> configHandler)
        {
            var handler = new HttpClientHandler();

            if (configHandler != null)
            {
                configHandler(handler);
            }

            return builder.AddHttpMessageHandler(() =>
            {
                return new DefaultDelegatingHandler(handler);
            });
        }
    }
}
