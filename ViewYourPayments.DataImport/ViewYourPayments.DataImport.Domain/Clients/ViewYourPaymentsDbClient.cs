using System;
using System.Collections.Generic;
using System.Linq;
using ViewYourPayments.Core.Interfaces;
using ViewYourPayments.Core.Models.Payments;
using ViewYourPayments.DataImport.Domain.Domain;
using ViewYourPayments.DataImport.Domain.Services;
using ViewYourPayments.EntityFramework;

namespace ViewYourPayments.DataImport.Domain.DomainClients
{
    /// <summary>
    /// Interaction with vyp database.
    /// </summary>
    public class ViewYourPaymentsDbClient : BaseDomainObject, IViewYourPaymentsDbClient
    {

        /// <summary>
        /// Default Constructor.
        /// </summary>
        /// <param name="dataService">Data Service to obtain db connection from.</param>
        /// <param name="logger">Logger that will write to Application Insights.</param>
        /// <param name="connectionString">Database connection string.</param>
        public ViewYourPaymentsDbClient(IDataService dataService, IApplicationLogger logger, string connectionString) : base(logger)
        {
            _dataService = dataService;
            _logger = logger;
            _connectionString = connectionString;
            _dbContext = new ViewYourPaymentsDbContext(_connectionString);
        }

        /// <summary>
        /// Data Service to obtain db connection from.
        /// </summary>
        protected readonly IDataService _dataService;

        /// <summary>
        /// Logger.
        /// </summary>
        protected readonly IApplicationLogger _logger;

        /// <summary>
        /// Database connection string.
        /// </summary>
        protected readonly string _connectionString;

        private ViewYourPaymentsDbContext _dbContext;


        /// <summary>
        /// Persist remittance advices to database.
        /// </summary>
        /// <param name="paymentSummaryList">List of Payment Summaries.</param>
        /// <param name="currentBatchNo">Current batch number.</param>
        /// <param name="dataImportHistory">Data import history object.</param>
        /// <param name="companySettings">NavApiCompanySettings object.</param>
        public int InsertRemittances(IEnumerable<PaymentSummary> paymentSummaryList,
            DataImportHistory dataImportHistory,
            NavApiCompanySettings companySettings,
            DateTime? BatchExecutionDate = null)
        {
            LogTrace($"Writing {paymentSummaryList.Count()} remittances to database.");
            var savedDetails = 0;
            var invalidUkprns = new List<string>();
            var duplicatePaymentIdentifiers = new List<int>();
            var budgetGroups = _dbContext.BudgetGroups.ToList();

            try
            {
                dataImportHistory.TotalRecords = paymentSummaryList.Sum(x => x.PaymentLine.Count);
                dataImportHistory.PaymentSummaryList = new List<PaymentSummary>();

                foreach (var paymentSummary in paymentSummaryList)
                {
                    if (paymentSummary.Ukprn?.Length < 8)
                    {
                        invalidUkprns.Add(paymentSummary.Ukprn);
                        continue;
                    }
                    var matchedPaymentSummaries = _dbContext.PaymentSummaries.Where(x => x.PaymentIdentifier == paymentSummary.PaymentIdentifier).Select(x => x.Id).ToArray();

                    if (matchedPaymentSummaries.Any())
                    {
                        var matchedPaymentLines = _dbContext.PaymentLines.Where(x => matchedPaymentSummaries.Contains(x.PaymentSummaryId))?
                                 .Where(x => x.CompanyName == companySettings.CompanyName)?.ToArray();

                        if (matchedPaymentLines.Any())
                        {
                            duplicatePaymentIdentifiers.Add(paymentSummary.PaymentIdentifier);
                            continue;
                        }
                    }

                    foreach (var paymentLine in paymentSummary.PaymentLine)
                    {
                        var contractCode = paymentLine.Contract ?? string.Empty;
                        paymentLine.BudgetGroup = budgetGroups.FirstOrDefault(x => contractCode.Contains($"{x.ContractCodePrefix}-"))?.BudgetGroupDescription;
                        paymentLine.BudgetGroup = paymentLine.BudgetGroup ?? "Other";
                        paymentLine.CompanyName = companySettings.CompanyName;
                    }
                    dataImportHistory.PaymentSummaryList.Add(paymentSummary);
                }

                _dbContext.DataImportHistories.Add(dataImportHistory);
                _dbContext.SaveChanges();
                savedDetails = dataImportHistory.PaymentSummaryList.Count;

                var message = string.Empty;
                message += $"Processed {paymentSummaryList.Count()} remittances. {savedDetails} " +
                           $"remittances committed to database.  {invalidUkprns.Count} invalid Ukpns " +
                            $"{duplicatePaymentIdentifiers.Count}  duplicate payment Identifiers or summaries found in data import history- {dataImportHistory.Id}.";

                LogTraceAndAudit(message);

                if (duplicatePaymentIdentifiers.Count > 0)
                    LogTrace($"Duplicate UKPRN's : {string.Join<int>(",", duplicatePaymentIdentifiers)}");

                if (invalidUkprns.Count > 0)
                    LogTrace($"invalid UKPRN's : {string.Join<string>(",", invalidUkprns)}");
            }
            catch (Exception ex)
            {
                var messasge = $"Error: could not process skip token: {dataImportHistory.SkipTokenNumber} and url {dataImportHistory.RequestedUrl}";
                LogExceptionAndAudit(ex, messasge);
                throw;
            }
            return savedDetails;
        }

        public int? GetLatestBatchNumber()
        {
            return _dbContext.DataImportHistories.OrderByDescending(x => x.BatchId).FirstOrDefault()?.BatchId;
        }

        public DataImportHistory GetLatestDataImportHistoryRecordByCompanyName(string companyName)
        {
            return _dbContext.DataImportHistories.Where(x => x.CompanyName == companyName).OrderByDescending(x => x.Id).FirstOrDefault();
        }

        /// <summary>
        /// Add Auditing records to database.
        /// </summary>
        /// <param name="audits">List of all the audits that need to be added to audit table.</param>
        public void InsertAudit(IEnumerable<DataImportAudit> audits)
        {
            try
            {
                _dbContext.DataImportAudits.AddRange(audits);
                _dbContext.SaveChanges();

            }
            catch (Exception ex)
            {
                LogException(ex, "Unable to create audit entry.");
                throw;
            }
        }

        public void InsertAudit(params DataImportAudit[] audits)
        {
            InsertAudit(audits.ToList());
        }
    }
}