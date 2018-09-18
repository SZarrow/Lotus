using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Lotus.Core;
using Lotus.Serialization;
using Newtonsoft.Json;

namespace Lotus.Net.Http
{
    public class HttpX : IDisposable
    {
        private readonly HttpClient _client = null;

        public HttpX(HttpClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            _client = client;
        }

        public async Task<XResult<HttpContent>> PostAsync(String postUrl, HttpContent postContent)
        {
            HttpResponseMessage respMsg = null;
            try
            {
                respMsg = await _client.PostAsync(postUrl, postContent);
            }
            catch (Exception ex)
            {
                return new XResult<HttpContent>(null, ex);
            }

            try
            {
                return new XResult<HttpContent>(respMsg.Content);
            }
            catch (Exception ex)
            {
                return new XResult<HttpContent>(null, ex);
            }
        }

        public async Task<XResult<TResult>> PostJsonAsync<TResult>(String postUrl, String jsonString)
        {
            HttpContent content = new StringContent(jsonString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Headers.ContentEncoding.Add("UTF-8");

            var respContent = await PostAsync(postUrl, content);
            if (!respContent.Success)
            {
                return new XResult<TResult>(default(TResult), respContent.Exceptions.ToArray());
            }

            try
            {
                String respString = await respContent.Value.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TResult>(respString);
                return new XResult<TResult>(result);
            }
            catch (Exception ex)
            {
                return new XResult<TResult>(default(TResult), ex);
            }
        }

        public async Task<XResult<HttpContent>> PostFormAsync<TResult>(String postUrl, IDictionary<String, String> formData)
        {
            var content = new FormUrlEncodedContent(formData);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            content.Headers.ContentEncoding.Add("UTF-8");
            return await PostAsync(postUrl, content);
        }

        public async Task<XResult<TResult>> PostXmlAsync<TResult>(String postUrl, String xml)
        {
            HttpContent content = new StringContent(xml);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            content.Headers.ContentEncoding.Add("UTF-8");

            var respContent = await PostAsync(postUrl, content);
            if (!respContent.Success)
            {
                return new XResult<TResult>(default(TResult), respContent.Exceptions.ToArray());
            }

            XSerializer serializer = new XSerializer();
            try
            {
                var respString = await respContent.Value.ReadAsStringAsync();
                var result = serializer.Deserialize<TResult>(respString);
                return result != null ? new XResult<TResult>(result) : new XResult<TResult>(default(TResult), new NullReferenceException(nameof(result)));
            }
            catch (Exception ex)
            {
                return new XResult<TResult>(default(TResult), ex);
            }
        }

        public async Task<XResult<HttpContent>> GetAsync<TResult>(String requestUrl)
        {
            HttpResponseMessage respMsg = null;
            try
            {
                respMsg = await _client.GetAsync(requestUrl);
            }
            catch (Exception ex)
            {
                return new XResult<HttpContent>(null, ex);
            }

            try
            {
                return new XResult<HttpContent>(respMsg.Content);
            }
            catch (Exception ex)
            {
                return new XResult<HttpContent>(null, ex);
            }
        }

        public void Dispose()
        {
            if (_client != null)
            {
                _client.Dispose();
            }
        }
    }
}
