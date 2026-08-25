using System;

namespace ViewYourPayments.Core.DTOs
{
    public class PaymentTransactionsResult
    {
        /// <summary>
        /// The unique identifer of the provider.
        /// </summary>
        public string ProviderUkprn { get; set; }

        /// <summary>
        /// Finance internal identifer of the provider.
        /// </summary>
        public string ProviderFinanceVendorCode { get; set; }

        /// <summary>
        /// The date from which the provider transactions are retrieved.
        /// </summary>
        public DateTime DateFrom { get; set; }

        /// <summary>
        /// The date until which the provider transactions are retrieved.
        /// </summary>
        public DateTime DateTo { get; set; }

        /// <summary>
        /// Budget group summary result set.
        /// </summary>
        public BudgetGroupSummary BudgetGroupSummary { get; set; }

        /// <summary>
        /// Payment transaction result set.
        /// </summary>
        public PaymentLineItem[] PaymentLines { get; set; } = Array.Empty<PaymentLineItem>();

        /// <summary>
        /// Current page number.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Number of records in a page-defaulted to 20.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total number of records.
        /// </summary>
        public int TotalRecords { get; set; }
    }
}
