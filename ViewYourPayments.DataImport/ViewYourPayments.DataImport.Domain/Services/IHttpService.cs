using System.Net.Http;
using System.Threading.Tasks;

namespace ViewYourPayments.DataImport.Domain.Services
{
    public interface IHttpService
    {

        /// <summary>
        /// Add Header to Default header list.
        /// </summary>
        /// <param name="header">Header.</param>
        /// <param name="value">Value.</param>
        void AddDefaultRequestHeader(string header, string value);

        /// <summary>
        /// Make a Async call to a uri.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>Return Task containing response.</returns>
        Task<HttpResponseMessage> GetAsync(string uri);

        /// <summary>
        /// Dispose of the active HttpClient.
        /// </summary>
        void Dispose();
    }
}
