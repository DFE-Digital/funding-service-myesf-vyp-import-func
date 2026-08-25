using ViewYourPayments.Core.Enums;

namespace ViewYourPayments.Core.DTOs
{
    public class PaymentTransactionSortItem
    {

        public PaymentTransactionSortItem(PaymentTransactionSortFields sortField, SortDirection sortDirection)
        {
            SortField = sortField;
            SortDirection = sortDirection;
        }

        public PaymentTransactionSortFields SortField { get; set; }
        public SortDirection SortDirection { get; set; }
    }
}
