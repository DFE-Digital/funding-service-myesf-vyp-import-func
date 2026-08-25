using ViewYourPayments.DataImport.Domain.Domain;

namespace ViewYourPayments.DataImport.Domain
{
    public interface IDeSerializeApimJson
    {
        /// <summary>
        /// Convert JSON to list of PaymentSummaries.
        /// </summary>
        /// <param name="rawJson">JSON to parse.</param>
        /// <returns>List of PaymentSummaries.</returns>
        PaymentDeSerializeResponse GetPaymentSummaries(string rawJson);
    }
}