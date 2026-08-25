using Polly;
using Polly.Timeout;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using ViewYourPayments.Core.Enums.Logging;
using ViewYourPayments.Core.Interfaces;
using ViewYourPayments.DataImport.Domain.Domain;
using ViewYourPayments.DataImport.Domain.Services;

namespace ViewYourPayments.DataImport.Domain.Clients
{
    /// <summary>
    /// Makes call to Client Apim API.
    /// </summary>
    public class ApimApi : BaseDomainObject, IApiClient
    {
        /// <summary>
        /// Date-format string.
        /// </summary>
        private const string DateFormat = "yyyy-MM-dd";

        /// <summary>
        /// Base Api Url.
        /// </summary>
        private readonly string _apimUri;

        /// <summary>
        /// EndPoint Version number.
        /// </summary>
        private readonly string _apimEndPointVersion;

        /// <summary>
        /// Key in the base url to replace.
        /// </summary>
        private readonly string _replacementKey = "{Company}";

        /// <summary>
        /// Http client service to use.
        /// </summary>
        private readonly IHttpService _client;

        /// <summary>
        /// Application Logging instance.
        /// </summary>
        private readonly IApplicationLogger _logger;

        /// <summary>
        /// Uri used to call api.
        /// </summary>
        public string UriCallWithParameters { get; private set; }

        /// <summary>
        /// Argument Constructor.
        /// </summary>
        /// <param name="client">The Http Client to use for the external calls.</param>
        /// <param name="logger">Logger to use.</param>
        /// <param name="apimEndPointUri">APIM Api Url.</param>
        /// <param name="apimEndPointVersion">APIM endpoint version.</param>
        /// <param name="apimSubscriptionKey">Subscription key for APIM endpoint.</param>
        public ApimApi(IHttpService client, IApplicationLogger logger, string apimEndPointUri, string apimEndPointVersion, string apimSubscriptionKey) : base(logger)
        {
            _client = client;
            _logger = logger;
            _apimUri = apimEndPointUri;
            _apimEndPointVersion = apimEndPointVersion;

            // Request headers
            _client.AddDefaultRequestHeader("Ocp-Apim-Subscription-Key", apimSubscriptionKey);
            _client.AddDefaultRequestHeader("accept", "application/json");
            //client.DefaultRequestHeaders.Add("Host", "apim-pp-finance.azure-api.net");
        }


        /// <summary>
        /// Make call to Apim api.
        /// </summary>
        /// <param name="PaymentApiRequest">paymentApiRequest with all request params.</param>
        /// <param name="retryCount">Number of retry if there is any failure.</param>
        /// <param name="retryInterval">Time interval between each failed call.</param>
        /// <returns>Task containing JSON result.</returns>
        public async Task<string> MakeRequest(PaymentApiRequest paymentApiRequest, int retryCountc, int retryInterval)
        {
            var apiUrl = _apimUri.Replace(_replacementKey, paymentApiRequest.Company);

            LogTrace($"APIM endpoint: [{apiUrl}]");

            LogTrace($"APIM parameters supplied: [DateFrom: {paymentApiRequest.FromDate}], [DateTo: {paymentApiRequest.ToDate}], [DimensionCode:  {paymentApiRequest.DimensionCode}], " +
                            $"[DimensionValue: {paymentApiRequest.DimensionValue}] ,SkipToken : {paymentApiRequest.SkipToken}", LoggingSeverity.Information);

            var queryString = new List<string>
            {
                $"dateFrom={paymentApiRequest.FromDate.ToString(DateFormat)}",
                $"dateTo={paymentApiRequest.ToDate.ToString(DateFormat)}",
                $"api-version={_apimEndPointVersion}"
            };

            if (paymentApiRequest.DimensionCode != null)
            {
                queryString.Add($"dimensionCode={paymentApiRequest.DimensionCode}");
            }

            if (paymentApiRequest.DimensionValue != null)
            {
                queryString.Add($"dimensionValue={paymentApiRequest.DimensionValue}");
            }

            if (!string.IsNullOrWhiteSpace(paymentApiRequest.SkipToken))
            {
                queryString.Add($"skipToken={paymentApiRequest.SkipToken}");
            }

            var requestedUri = apiUrl + "?" + string.Join("&", queryString);

            UriCallWithParameters = Uri.EscapeUriString(requestedUri);

            LogTrace($"Calling APIM: {UriCallWithParameters}");
            var retryPolicy = Policy
                    .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                    .Or<TimeoutRejectedException>()
                     .WaitAndRetryAsync(retryCountc, i => TimeSpan.FromMilliseconds(retryInterval), (result, timeSpan, retryCount, context) =>
                     {
                         LogTraceAndAudit($"Request failed with {result.Result.StatusCode}. Waiting {timeSpan} before next retry. Retry attempt {retryCount}");
                     });
            try
            {
                var response = await retryPolicy.ExecuteAsync(() =>
                {
                    var timer = new Stopwatch();
                    timer.Start();
                    var retVal = _client.GetAsync(UriCallWithParameters);
                    timer.Stop();
                    if (retVal.Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        LogTraceAndAudit($"Call to APIM Api successful. request url is {requestedUri} " +
                            $" which took {timer.Elapsed.TotalMilliseconds} ms time");
                    }
                    return retVal;
                });
                string responseString = await response.Content?.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(responseString);
                }
                return responseString;
            }
            catch (Exception ex)
            {
                LogExceptionAndAudit(ex, $"Error calling APIM: {ex.Message} with uri {requestedUri}");
                throw;
            }
        }

        /// <summary>
        /// Dispose http client.
        /// </summary>
        public void DisposeApi()
        {
            _client.Dispose();
        }
    }
}
