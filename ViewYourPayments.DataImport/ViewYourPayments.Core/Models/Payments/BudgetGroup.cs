using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel.DataAnnotations;

namespace ViewYourPayments.Core.Models.Payments
{
    public class BudgetGroup
    {
        [Key]
        public int Id { get; private set; }

        [MaxLength(10)]
        public string ContractCodePrefix { get; private set; }

        [MaxLength(500)]
        public string BudgetGroupDescription { get; private set; }

        public DateTime CreatedOn { get; private set; }
    }
}
