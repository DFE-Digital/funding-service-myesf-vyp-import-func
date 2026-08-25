using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ViewYourPayments.DataImport.Domain.Services
{
    /// <summary>
    /// Proxy class for HttpClient.
    /// </summary>
    public class HttpService : IHttpService
    {
        /// <summary>
        /// Http client.
        /// </summary>
        private HttpClient _client;

        public HttpService()
        {
            _client = new HttpClient();
            _client.Timeout = TimeSpan.FromMinutes(10);
        }

        /// <summary>
        /// Make a Async call to a uri.
        /// </summary>
        /// <param name="uri">Url to query.</param>
        /// <returns>Return Task containing response.</returns>
        public Task<HttpResponseMessage> GetAsync(string uri) => _client.GetAsync(uri);

        /// <summary>
        /// Dispose client instance.
        /// </summary>
        public void Dispose() => _client.Dispose();


        /// <summary>
        /// Add Header to default header list.
        /// </summary>
        /// <param name="header">Header.</param>
        /// <param name="value">Value.</param>
        public void AddDefaultRequestHeader(string header, string value) => _client.DefaultRequestHeaders.Add(header, value);
    }
}