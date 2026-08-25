
using Microsoft.EntityFrameworkCore;
using System;
using ViewYourPayments.Core.Models.Payments;

namespace ViewYourPayments.EntityFramework
{
    /// <summary>
    /// Represents DB access wrapper for the Entity Framework.
    /// </summary>
   // [DbConfigurationType(typeof(SqlAzureDbConfiguration))]
    public class ViewYourPaymentsDbContext : DbContext
    {
        private readonly string _connectionString;


        public ViewYourPaymentsDbContext() : base()
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public ViewYourPaymentsDbContext(DbContextOptions options) : base(options)
        {

        }

        public ViewYourPaymentsDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }
        }

        public virtual DbSet<PaymentLine> PaymentLines { get; set; }
        public virtual DbSet<PaymentLineStaging> PaymentLinesStaging { get; set; }
        public virtual DbSet<PaymentSummary> PaymentSummaries { get; set; }
        public virtual DbSet<PaymentSummaryStaging> PaymentSummariesStaging { get; set; }
        public virtual DbSet<DataImportAudit> DataImportAudits { get; set; }
        public virtual DbSet<DataImportHistory> DataImportHistories { get; set; }
        public virtual DbSet<BudgetGroup> BudgetGroups { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DataImportAudit>().Property(x => x.CreatedOn).HasDefaultValueSql("GetDate()");

            modelBuilder.Entity<BudgetGroup>()
           .HasIndex(p => new { p.ContractCodePrefix })
           .IsUnique(true);

            modelBuilder.Entity<BudgetGroup>().Property(x => x.CreatedOn).HasDefaultValueSql("GetDate()");

            modelBuilder.Entity<PaymentLine>()
                .HasIndex(p => new { p.Contract, p.PaymentLineDescription, p.PaymentLineGroupDescription, p.LineAmount, p.BudgetGroup })
                .IsUnique(false);

            modelBuilder.Entity<PaymentLine>().Property(x => x.CreatedOn).HasDefaultValueSql("GetDate()");
            modelBuilder.Entity<PaymentLine>().Property(x => x.PaymentLineIdentifier).IsRequired(true);
            modelBuilder.Entity<PaymentLine>().HasIndex(p => p.PaymentLineIdentifier).IsUnique(false);

            modelBuilder.Entity<PaymentSummary>()
                .HasIndex(p => new { p.PaymentDate, p.Ukprn, p.PaymentTotal })
                 .IsUnique(false);

            modelBuilder.Entity<PaymentSummary>()
                .HasIndex(p => new { p.PaymentIdentifier }).IsUnique(false);
            modelBuilder.Entity<PaymentSummary>().Property(x => x.CreatedOn).HasDefaultValueSql("GetDate()");



            modelBuilder.Entity<DataImportHistory>()
              .HasIndex(p => new { p.BatchId })
               .IsUnique(false);
            modelBuilder.Entity<DataImportHistory>().Property(x => x.CreatedOn).HasDefaultValueSql("GetDate()");
            modelBuilder.Entity<DataImportHistory>().Property(x => x.CompanyName).HasDefaultValue("ESFA");
            modelBuilder.Entity<DataImportHistory>().Property(x => x.CompanyName).IsRequired(true);


            modelBuilder.Entity<BudgetGroup>().HasData(
               new { Id = 1, BudgetGroupDescription = "16-18 Traineeships", ContractCodePrefix = "16TR", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 2, BudgetGroupDescription = "16-19 Funding", ContractCodePrefix = "16ED", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 3, BudgetGroupDescription = "16-19 Funding", ContractCodePrefix = "16LR", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 5, BudgetGroupDescription = "16-19 Funding", ContractCodePrefix = "NLG", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 6, BudgetGroupDescription = "16-19 Funding", ContractCodePrefix = "NMF", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 7, BudgetGroupDescription = "16-19 Funding", ContractCodePrefix = "SSF", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 8, BudgetGroupDescription = "Advanced Learner Loans", ContractCodePrefix = "ALLB", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 9, BudgetGroupDescription = "Advanced Learner Loans", ContractCodePrefix = "ALLC", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 10, BudgetGroupDescription = "Advanced Learner Loans", ContractCodePrefix = "ALLF", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 11, BudgetGroupDescription = "Advanced Learner Loans", ContractCodePrefix = "CLP", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 12, BudgetGroupDescription = "Apprenticeship Grant for Employers", ContractCodePrefix = "AGE", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 13, BudgetGroupDescription = "Apprenticeship (Employer on App service) Levy", ContractCodePrefix = "LEVY", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 14, BudgetGroupDescription = "Apprenticeship (Employer on App service) Non-Levy", ContractCodePrefix = "NLA", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 15, BudgetGroupDescription = "Apprenticeships Carry-in", ContractCodePrefix = "16AP", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 16, BudgetGroupDescription = "Apprenticeships Carry-in", ContractCodePrefix = "16NL", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 17, BudgetGroupDescription = "Apprenticeships Carry-in", ContractCodePrefix = "AAPP", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 18, BudgetGroupDescription = "Apprenticeships Carry-in", ContractCodePrefix = "ANL", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 19, BudgetGroupDescription = "Apprenticeships Carry-in", ContractCodePrefix = "APPS", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 20, BudgetGroupDescription = "European Social Fund", ContractCodePrefix = "ESF", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 21, BudgetGroupDescription = "National Careers Service", ContractCodePrefix = "NCSA", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 22, BudgetGroupDescription = "National Careers Service", ContractCodePrefix = "NCSC", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 23, BudgetGroupDescription = "National Careers Service", ContractCodePrefix = "NCSD", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 24, BudgetGroupDescription = "National Careers Service", ContractCodePrefix = "NCSE", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 25, BudgetGroupDescription = "National Careers Service", ContractCodePrefix = "NCSF", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 26, BudgetGroupDescription = "National Careers Service", ContractCodePrefix = "NCSI", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 27, BudgetGroupDescription = "National Careers Service", ContractCodePrefix = "NCSP", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 28, BudgetGroupDescription = "National Careers Service", ContractCodePrefix = "NCSS", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 29, BudgetGroupDescription = "National Careers Service", ContractCodePrefix = "NCSV", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 30, BudgetGroupDescription = "Non-Procured Adult Education Budget", ContractCodePrefix = "AEC", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 31, BudgetGroupDescription = "Non-Procured Adult Education Budget", ContractCodePrefix = "AECA", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 32, BudgetGroupDescription = "Non-Procured Adult Education Budget", ContractCodePrefix = "AECL", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 33, BudgetGroupDescription = "Non-Procured Adult Education Budget", ContractCodePrefix = "AECT", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 34, BudgetGroupDescription = "Non-Procured Adult Education Budget", ContractCodePrefix = "AELS", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 35, BudgetGroupDescription = "Non-Procured Adult Education Budget", ContractCodePrefix = "AEO", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 36, BudgetGroupDescription = "Procured Adult Education Budget", ContractCodePrefix = "AEBL", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 37, BudgetGroupDescription = "Procured Adult Education Budget", ContractCodePrefix = "AEBT", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 38, BudgetGroupDescription = "Procured Adult Education Budget", ContractCodePrefix = "AEAL", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 39, BudgetGroupDescription = "Procured Adult Education Budget", ContractCodePrefix = "AEBA", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 40, BudgetGroupDescription = "Procured Adult Education Budget", ContractCodePrefix = "AEBC", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 41, BudgetGroupDescription = "Procured Adult Education Budget", ContractCodePrefix = "AEBR", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 42, BudgetGroupDescription = "Procured Adult Education Budget", ContractCodePrefix = "AETL", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 43, BudgetGroupDescription = "Procured Non-Levy Apprenticeships", ContractCodePrefix = "ANAS", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 44, BudgetGroupDescription = "Procured Non-Levy Apprenticeships", ContractCodePrefix = "ANLP", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 45, BudgetGroupDescription = "Procured Non-Levy Apprenticeships", ContractCodePrefix = "YNAS", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 46, BudgetGroupDescription = "Procured Non-Levy Apprenticeships", ContractCodePrefix = "YNLP", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 47, BudgetGroupDescription = "Apprenticeships Carry-in", ContractCodePrefix = "AATO", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 48, BudgetGroupDescription = "Apprenticeships Carry-in", ContractCodePrefix = "AAC", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 49, BudgetGroupDescription = "National Careers Service", ContractCodePrefix = "NCS", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 50, BudgetGroupDescription = "Non-Procured Adult Education Budget", ContractCodePrefix = "ASC", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 51, BudgetGroupDescription = "Non-Procured Adult Education Budget", ContractCodePrefix = "ASTO", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 52, BudgetGroupDescription = "Non-Procured Adult Education Budget", ContractCodePrefix = "DL", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 53, BudgetGroupDescription = "Non-Procured Adult Education Budget", ContractCodePrefix = "DLS", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 54, BudgetGroupDescription = "Employer Ownership Pilot", ContractCodePrefix = "EOP1", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 55, BudgetGroupDescription = "Offender Learning and Skills Strategy", ContractCodePrefix = "OLAS", CreatedOn = new DateTime(2019, 12, 1) },
                    new { Id = 56, BudgetGroupDescription = "Non-Learning Grants", ContractCodePrefix = "CCF", CreatedOn = new DateTime(2020, 08, 20) }
                    );
        }
    }
}
