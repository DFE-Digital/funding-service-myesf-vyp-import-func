using System;
using System.Collections.Generic;
using ViewYourPayments.Core.Models.Payments;

namespace ViewYourPayments.DataImportFunction.Tests.Helpers
{
    class PaymentSummaryBuilder
    {
        private static readonly PaymentSummary Default = new PaymentSummary
        {
            PaymentDate = new DateTime(2019, 01, 02),
            PaymentIdentifier = 123456,
            PaymentLine = new List<PaymentLine>(),
            PaymentTotal = 100.00m,
            Ukprn = "12345678"
        };

        public PaymentSummaryBuilder() : this(Default) { }

        public PaymentSummaryBuilder(PaymentSummary summary)
        {
            _paymentDate = summary.PaymentDate;
            _paymentIdentifier = summary.PaymentIdentifier;
            _paymentLine = summary.PaymentLine;
            _paymentTotal = summary.PaymentTotal;
            _ukprn = summary.Ukprn;
        }

        /// <summary>
        /// The Payment Identifier of the Payment.
        /// </summary>
        public int _paymentIdentifier;

        /// <summary>
        /// Who the remittance is for.
        /// </summary>
        public string _ukprn;

        /// <summary>
        /// The payment method used.
        /// </summary>
        public string _paymentMethod;

        /// <summary>
        /// The payment amount for the summary.
        /// </summary>
        public decimal _paymentTotal;

        /// <summary>
        /// The payment date.
        /// </summary>
        public DateTime _paymentDate;

        /// <summary>
        /// List of payment lines associated with payment summary.
        /// </summary>
        public IList<PaymentLine> _paymentLine;

        /// <summary>
        /// Set the Payment Identifier of the Payment.
        /// </summary>
        public PaymentSummaryBuilder SetPaymentIdentifier(int paymentId)
        {
            _paymentIdentifier = paymentId;
            return this;
        }

        /// <summary>
        /// Set who the remittance is for.
        /// </summary>
        public PaymentSummaryBuilder SetUkprn(string ukprn)
        {
            _ukprn = ukprn;
            return this;
        }

        /// <summary>
        /// Set the payment method used.
        /// </summary>
        public PaymentSummaryBuilder SetPaymentMethod(string method)
        {
            _paymentMethod = method;
            return this;
        }

        /// <summary>
        /// Set the payment amount for the summary.
        /// </summary>
        public PaymentSummaryBuilder SetPaymentTotal(decimal total)
        {
            _paymentTotal = total;
            return this;
        }

        /// <summary>
        /// Set the payment date.
        /// </summary>
        public PaymentSummaryBuilder SetPaymentDate(DateTime date)
        {
            _paymentDate = date;
            return this;
        }

        /// <summary>
        /// Set List of payment lines associated with payment summary.
        /// </summary>
        public PaymentSummaryBuilder SetPaymentLine(IList<PaymentLine> paymentLines)
        {
            _paymentLine = paymentLines;
            return this;
        }

        public PaymentSummary Build()
        {
            return new PaymentSummary
            {
                PaymentDate = _paymentDate,
                PaymentIdentifier = _paymentIdentifier,
                PaymentLine = _paymentLine,
                PaymentTotal = _paymentTotal,
                Ukprn = _ukprn
            };
        }
    }
}
