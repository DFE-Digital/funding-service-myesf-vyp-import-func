using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ViewYourPayments.Core.Models.Payments
{
    public class BasePaymentLine
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The Payment Identifier of the payment line.
        /// </summary>
        [MaxLength(100)]
        public string PaymentLineIdentifier { get; set; }

        /// <summary>
        /// The contract number.
        /// </summary>
        [MaxLength(200)]
        [JsonProperty("contractCode")]
        public string Contract { get; set; }

        /// <summary>
        /// Establisment Code
        /// </summary>
        [MaxLength(200)]
        public string Establishment { get; set; }

        /// <summary>
        /// Establisment Description
        /// </summary>
        [MaxLength(1000)]
        public string EstablishmentDescription { get; set; }

        /// <summary>
        /// The payment line description (long description).
        /// </summary>
        [MaxLength(1000)]
        public string PaymentLineDescription { get; set; }

        /// <summary>
        /// The payment line description (long description).
        /// </summary>
        [MaxLength(1000)]
        public string PaymentLineGroupDescription { get; set; }


        /// <summary>
        /// Linked payment summary row id
        /// </summary>
        public int PaymentSummaryId { get; set; }

        /// <summary>
        /// The payment amount.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal LineAmount { get; set; }

        public DateTime CreatedOn { get; set; }

        [JsonProperty("fundingTypeCode")]
        public string FundingType { get; set; }
        public DateTime PostingDate { get; set; }
        [MaxLength(1000)]
        public string BudgetGroup { get; set; }

        /// <summary>
        /// Gets or sets the business central company name this payment line is associated to.
        /// </summary>
        /// <value>
        /// The name of the company for ex 104 - ESFA.
        /// </value>
        [MaxLength(100)]
        public string CompanyName { get; set; }
    }
}
