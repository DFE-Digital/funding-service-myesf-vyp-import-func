using System.Collections.Generic;

namespace ViewYourPayments.Core.Models.Payments
{
    public class PaymentSummaryStaging : BasePaymentSummary
    {
        /// <summary>
        /// List of payment lines associated with payment summary.
        /// </summary>
        public IList<PaymentLineStaging> PaymentLine { get; set; } = new List<PaymentLineStaging>();

    }
}
