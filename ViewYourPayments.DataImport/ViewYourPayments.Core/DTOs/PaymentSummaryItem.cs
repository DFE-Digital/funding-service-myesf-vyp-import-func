using System;

namespace ViewYourPayments.Core.DTOs
{
    public class PaymentSummaryItem
    {
        /// <summary>
        /// The unique identifer of the payment made to provider.
        /// </summary>
        public int PaymentIdentifier { get; set; }

        /// <summary>
        /// Payment date when the remittance is transferred to provider's bank account.
        /// </summary>
        public DateTime PaymentDate { get; set; }

        /// <summary>
        /// The payment amount transferred to provider's bank account.
        /// </summary>
        public decimal PaymentAmount { get; set; }
    }
}
