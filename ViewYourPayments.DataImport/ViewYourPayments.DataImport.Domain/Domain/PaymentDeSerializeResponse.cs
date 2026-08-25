using System.Collections.Generic;
using ViewYourPayments.Core.Models.Payments;

namespace ViewYourPayments.DataImport.Domain.Domain
{
    public class PaymentDeSerializeResponse
    {
        public IList<PaymentSummary> Payments { get; set; }

        public string SkipToken { get; set; }

        public int FailedCount { get; set; }
    }
}
