using System.Collections.Generic;

namespace ViewYourPayments.Core.Models.Payments
{
    public class PaymentSummary : BasePaymentSummary
    {
        /// <summary>
        /// List of payment lines associated with payment summary.
        /// </summary>
        public IList<PaymentLine> PaymentLine { get; set; } = new List<PaymentLine>();

    }
}
