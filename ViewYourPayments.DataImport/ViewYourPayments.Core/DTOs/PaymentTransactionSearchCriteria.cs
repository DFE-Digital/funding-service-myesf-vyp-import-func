using System;
using ViewYourPayments.Core.Enums;

namespace ViewYourPayments.Core.DTOs
{
    /// <summary>
    /// This is PaymentTransactionSearchCriteria for test swagger.
    /// </summary>
    public class PaymentTransactionSearchCriteria
    {
        /// <summary>
        /// Provider Identifier-UKPRN.
        /// </summary>
        public string Ukprn { get; set; }

        /// <summary>
        /// Date from which transactions are required, must be used together with end date.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Date until which transactions are required,, must be used together with start date.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Page number (optional). If not specified, first page will be returned.
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Page size (optional). If not specified, 20 records will be returned.
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// Primary sort field(optional). If not specified default to PaymentDate.
        /// </summary>
        public PaymentTransactionSortFields PrimarySortField { get; set; } = PaymentTransactionSortFields.PaymentDate;

        /// <summary>
        /// Sort direction field(optional). If not specified default to Descending.
        /// </summary>
        public SortDirection SortDirection { get; set; } = SortDirection.Descending;

        /// <summary>
        /// Selected search terms by user.
        /// </summary>
        public string[] SearchTerms { get; set; } = Array.Empty<string>();
    }
}
