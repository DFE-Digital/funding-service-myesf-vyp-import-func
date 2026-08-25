using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ViewYourPayments.Core.DTOs;

namespace ViewYourPayments.Core.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<PaymentSummaryResult> GetPaymentSummaries(PaymentSummarySearchCriteria searchCriteria);
        Task<PaymentDetailsResult> GetPaymentDetails(string ukprn, int paymentIdentifier);
        Task<PaymentTransactionsResult> GetPaymentTransactions(PaymentTransactionSearchCriteria searchCriteria);
        Task<IEnumerable<string>> GetUniqueTransactionDescriptions(string ukprn, DateTime startDate, DateTime endDate);
        Task<int> GetPaymentSummariesCount(PaymentSummarySearchCriteria searchCriteria);
    }
}
