using System;

namespace ViewYourPayments.Core.DTOs
{
    public class PaymentLineItem
    {
        /// <summary>
        /// The unique identifer of a payment line.
        /// </summary>
        public string PaymentLineIdentifier { get; set; }

        /// <summary>
        /// payment date when the remittance is generated.
        /// </summary>
        public DateTime PaymentDate { get; set; }

        /// <summary>
        /// Corresponding contract number of the payment line.
        /// </summary>
        public string ContractNumber { get; set; }

        /// <summary>
        /// Corresponding budget description of the payment line.
        /// </summary>
        public string BudgetDescription { get; set; }

        /// <summary>
        /// Description of the Payment line.
        /// </summary>
        public string PaymentLineDescription { get; set; }

        /// <summary>
        /// The payment line amount.
        /// </summary>
        public decimal PaymentLineAmount { get; set; }
    }
}
