using System.Collections.Generic;
using System.Threading.Tasks;
using ViewYourPayments.Core.DTOs;
using ViewYourPayments.Core.Models.Payments;

namespace ViewYourPayments.Core.Interfaces.Repositories
{
    public interface IPaymentSummaryRepository
    {
        Task<IEnumerable<PaymentSummary>> GetPaymentSummaries(PaymentSummarySearchCriteria searchCriteria);
        Task<int> GetPaymentSummariesCount(PaymentSummarySearchCriteria searchCriteria);
    }
}
