using System;

namespace ViewYourPayments.Core.DTOs
{
    public class PaymentDetailsResult
    {
        /// <summary>
        /// The payment amount transferred to the provider's bank account.
        /// </summary>
        public decimal? PaymentAmount { get; set; }

        /// <summary>
        /// Date on which payment was made to provider.
        /// </summary>
        public DateTime? PaymentDate { get; set; }
        /// <summary>
        /// The unique identifer of the provider.
        /// </summary>
        public string ProviderUkprn { get; set; }

        /// <summary>
        /// Finance internal identifer of the provider.
        /// </summary>
        public string ProviderFinanceVendorIdentifier { get; set; }

        /// <summary>
        /// Payment line rows.
        /// </summary>
        public PaymentLineItem[] PaymentLines { get; set; } = Array.Empty<PaymentLineItem>();
    }
}
