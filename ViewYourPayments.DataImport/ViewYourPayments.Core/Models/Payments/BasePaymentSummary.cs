using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViewYourPayments.Core.Models.Payments
{
    public class BasePaymentSummary
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The Payment Identifier of the Payment.
        /// </summary>
        public int PaymentIdentifier { get; set; }

        /// <summary>
        /// Who the remittance is for.
        /// </summary>
        [MaxLength(50)]
        public string Ukprn { get; set; }

        /// <summary>
        /// The payment amount for the summary.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal PaymentTotal { get; set; }

        /// <summary>
        /// The payment date.
        /// </summary>
        public DateTime PaymentDate { get; set; }

        /// <summary>
        /// Vendor reference
        /// </summary>
        [MaxLength(1000)]
        public string VendorIdentifier { get; set; }

        /// <summary>
        /// List of payment lines associated with payment summary.
        /// </summary>
        public DataImportHistory DataImportHistory { get; set; }
        /// <summary>
        /// Data Import History Id under payment has been imported
        /// </summary>
        public int DataImportHistoryId { get; set; }

        public DateTime CreatedOn { get; set; }

    }
}
