using System;

namespace ViewYourPayments.Core.DTOs
{
    /// <summary>
    /// Bank payment search result.
    /// </summary>
    public class PaymentSummaryResult
    {
        public PaymentSummaryResult()
        {
            PaymentSummaries = Array.Empty<PaymentSummaryItem>();
        }

        /// <summary>
        /// The unique identifer of the provider.
        /// </summary>
        public string ProviderUKPRN { get; set; }

        /// <summary>
        /// The date from which the provider payments are retrieved.
        /// </summary>
        public DateTime DateFrom { get; set; }

        /// <summary>
        /// The date until which the provider payments are retrieved.
        /// </summary>
        public DateTime DateTo { get; set; }

        /// <summary>
        /// List of Provider Bank Payments.
        /// </summary>
        public PaymentSummaryItem[] PaymentSummaries { get; set; }

        /// <summary>
        /// Total number of page based on selected page size and total records.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// current page number.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// total number of records for selected search criteria.
        /// </summary>
        public int TotalRecords { get; set; }

        /// <summary>
        /// number of records in a page-defaulted to 20.
        /// </summary>
        public int PageSize { get; set; }
    }
}
