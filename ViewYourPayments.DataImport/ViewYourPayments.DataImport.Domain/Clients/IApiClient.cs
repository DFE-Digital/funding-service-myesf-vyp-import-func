using System.Threading.Tasks;
using ViewYourPayments.DataImport.Domain.Domain;

namespace ViewYourPayments.DataImport.Domain.Clients
{
    /// <summary>
    /// Api Client Interface.
    /// </summary>
    public interface IApiClient
    {
        /// <summary>
        /// Client API and parameters used.
        /// </summary>
        string UriCallWithParameters { get; }

        /// <summary>
        /// Make call to Apim api.
        /// </summary>
        /// <param name="paymentApiRequest">PaymentApi </param>
        /// <param name="retryCount">No of retries.</param>
        /// <param name="retryInterval">Time interval between each retry.</param>
        /// <returns>Task containing Json result.</returns>
        Task<string> MakeRequest(PaymentApiRequest paymentApiRequest, int retryCount, int retryInterval);

        /// <summary>
        /// Dispose http client.
        /// </summary>
        void DisposeApi();
    }
}
