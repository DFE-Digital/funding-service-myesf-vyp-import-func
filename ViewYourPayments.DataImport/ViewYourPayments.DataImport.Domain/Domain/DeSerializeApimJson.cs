using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using ViewYourPayments.Core.Interfaces;
using ViewYourPayments.Core.Models.Payments;
using ViewYourPayments.DataImport.Domain.Domain;

namespace ViewYourPayments.DataImport.Domain
{
    /// <summary>
    /// DeSerialize JSON to List of PaymentSummaries.
    /// </summary>
    public class DeSerializeApimJson : BaseDomainObject, IDeSerializeApimJson
    {
        private readonly IApplicationLogger _logger;
        const string Payments = "payments";
        const string SkipToken = "skipToken";

        /// <summary>
        /// Argument Constructor.
        /// </summary>
        /// <param name="logger">Logger</param>
        public DeSerializeApimJson(IApplicationLogger logger) : base(logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Convert JSON to list of PaymentSummaries.
        /// </summary>
        /// <param name="rawJson">JSON to parse.</param>
        /// <returns>List of PaymentSummaries.</returns>
        public PaymentDeSerializeResponse GetPaymentSummaries(string rawJson)
        {
            LogTrace($"DeSerializing JSON payment summaries. Length of raw json: {rawJson.Length}");

            JObject apimResults = JObject.Parse(rawJson);

            // get JSON result objects into a list
            IList<JToken> results = apimResults.GetValue(Payments, StringComparison.OrdinalIgnoreCase)?
                .Value<IList<JToken>>();
            int failedCount = 0;
            string skipToken;
            try
            {
                skipToken = apimResults.GetValue(SkipToken, StringComparison.OrdinalIgnoreCase)?
                    .Value<string>();
            }
            catch (Exception)
            {
                skipToken = null;
            }

            // serialize JSON results into .NET objects
            List<PaymentSummary> paymentSummaries = new List<PaymentSummary>();

            foreach (JToken result in results ?? Enumerable.Empty<JToken>())
            {
                // JToken.ToObject is a helper method that uses JsonSerializer internally
                try
                {
                    PaymentSummary searchResult = result.ToObject<PaymentSummary>();
                    paymentSummaries.Add(searchResult);
                }
                catch (Exception ex)
                {
                    _logger.LogException(ex, "Incorrect data");
                    failedCount++;
                }
            }

            LogTrace($"DeSerialized JSON payment summaries into {paymentSummaries.Count} remittance summaries. SkipCodeMessage: {skipToken}.");

            return (new PaymentDeSerializeResponse
            {
                Payments = paymentSummaries,
                SkipToken = skipToken,
                FailedCount = failedCount
            });
        }

    }
}