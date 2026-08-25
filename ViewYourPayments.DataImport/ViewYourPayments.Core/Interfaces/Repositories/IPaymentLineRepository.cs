using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ViewYourPayments.Core.DTOs;
using ViewYourPayments.Core.Models.Payments;

namespace ViewYourPayments.Core.Interfaces.Repositories
{
    public interface IPaymentLineRepository
    {
        Task<IEnumerable<PaymentLine>> GetPaymentLines(string ukprn, int paymentIdentifier);
        Task<IEnumerable<BudgetGroupItem>> GetBudgetGroupSummaries(PaymentTransactionSearchCriteria searchCriteria);
        Task<IEnumerable<PaymentLine>> GetPaymentTransactions(PaymentTransactionSearchCriteria searchCriteria);
        Task<int> GetPaymentTransactionCount(PaymentTransactionSearchCriteria searchCriteria);
        Task<IEnumerable<string>> GetUniqueTransactionDescriptions(string ukprn, DateTime startDate, DateTime endDate);
    }
}
