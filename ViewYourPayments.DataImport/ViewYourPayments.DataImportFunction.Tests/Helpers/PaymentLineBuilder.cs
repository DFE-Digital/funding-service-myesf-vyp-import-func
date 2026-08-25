using System;
using ViewYourPayments.Core.Models.Payments;

namespace ViewYourPayments.DataImportFunction.Tests.Helpers
{
    class PaymentLineBuilder
    {
        /// <summary>
        /// The Payment Identifier of the payment detail.
        /// </summary>
        private string _paymentLineIdentifier;

        /// <summary>
        /// The contract number.
        /// </summary>
        private string _contract;

        /// <summary>
        /// The document number.
        /// </summary>
        private string _documentNumber;

        /// <summary>
        /// The payment line description (long description).
        /// </summary>
        private string _paymentLineDescription;

        /// <summary>
        /// The payment line group description (short description).
        /// </summary>
        private string _paymentLineGroupDescription;

        /// <summary>
        /// The line description (short description).
        /// </summary>
        private string _lineDescription;

        /// <summary>
        /// The payment amount.
        /// </summary>
        private decimal _lineAmount;

        private static readonly PaymentLine Default = new PaymentLine
        {
            PaymentLineIdentifier = "123456",
            Contract = "Contract",
            PaymentLineDescription = "Payment Line Description",
            PaymentLineGroupDescription = "PaymentLineDescription A",
            LineAmount = 10.5m
        };

        public PaymentLineBuilder() : this(Default) { }

        public PaymentLineBuilder(PaymentLine line)
        {
            _paymentLineIdentifier = line.PaymentLineIdentifier;
            _paymentLineDescription = line.PaymentLineDescription;
            _contract = line.Contract;
            _lineAmount = line.LineAmount;
            _paymentLineGroupDescription = line.PaymentLineGroupDescription;
        }

        /// <summary>
        /// Set the Payment Identifier of the payment detail.
        /// </summary>
        public PaymentLineBuilder SetPaymentLineId(string paymentLineId)
        {
            _paymentLineIdentifier = paymentLineId;
            return this;
        }

        /// <summary>
        /// Set the contract number.
        /// </summary>
        public PaymentLineBuilder SetContract(string contract)
        {
            _contract = contract;
            return this;
        }

        /// <summary>
        /// Set the document number.
        /// </summary>
        public PaymentLineBuilder SetDocumentNumber(string documentNumber)
        {
            _documentNumber = documentNumber;
            return this;
        }

        /// <summary>
        /// Set the payment line description (long description).
        /// </summary>
        public PaymentLineBuilder SetPaymentLineDescription(string paymentLineDescription)
        {
            _paymentLineDescription = paymentLineDescription;
            return this;
        }

        /// <summary>
        /// Set the line description (short description).
        /// </summary>
        public PaymentLineBuilder SetLineDescription(string lineDescription)
        {
            _lineDescription = lineDescription;
            return this;
        }

        /// <summary>
        /// Set the payment amount.
        /// </summary>
        public PaymentLineBuilder SetLineAmount(decimal lineAmount)
        {
            _lineAmount = lineAmount;
            return this;
        }

        public PaymentLine Build()
        {
            return new PaymentLine()
            {
                PaymentLineIdentifier = _paymentLineIdentifier,
                Contract = _contract,
                PaymentLineDescription = _paymentLineDescription,
                PaymentLineGroupDescription = _paymentLineGroupDescription,
                LineAmount = _lineAmount,
                PostingDate = new DateTime(2019, 11, 11),
                FundingType = "AEB"
            };

        }
    }
}
