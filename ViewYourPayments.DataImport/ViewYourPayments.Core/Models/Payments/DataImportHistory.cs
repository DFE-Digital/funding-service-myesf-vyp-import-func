using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ViewYourPayments.Core.Models.Payments
{
    public class DataImportHistory
    {
        [Key]
        public int Id { get; set; }

        public int BatchId { get; set; }

        [MaxLength(100)]
        public string SkipTokenNumber { get; set; }

        public DateTime RunFromDate { get; set; }

        public DateTime RunToDate { get; set; }

        [MaxLength(1000)]
        public string RequestedUrl { get; set; }

        public int TotalRecords { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CompanyName { get; set; }
        /// <summary>
        /// List of payment lines associated with payment summary.
        /// </summary>
        public IList<PaymentSummary> PaymentSummaryList { get; set; }
    }
}
