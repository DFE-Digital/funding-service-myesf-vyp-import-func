using System;

namespace ViewYourPayments.Core.DTOs
{
    public class PaymentSummarySearchCriteria
    {
        public string Ukprn { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
