using System;
using System.Collections.Generic;
using ViewYourPayments.Core.Models.Payments;
using ViewYourPayments.DataImport.Domain.Domain;

namespace ViewYourPayments.DataImport.Domain.DomainClients
{
    public interface IViewYourPaymentsDbClient
    {
        /// <summary>
        /// Insert remittance advices.
        /// </summary>
        /// <param name="paymentSummaryList">Remittance list.</param>
        /// <param name="batchNumber">Batch number.</param>
        /// <param name="companySettings">Company Settings.</param>
        int InsertRemittances(IEnumerable<PaymentSummary> paymentSummaryList,
            DataImportHistory dataImportHistory,
            NavApiCompanySettings companySettings,
            DateTime? BatchExecutionDate = null);

        /// <summary>
        /// Add Auditing records to database when outside of existing connection.
        /// </summary>
        /// <param name = "audits" > List of all the audits that need to be added to audit table.</param>
        void InsertAudit(IEnumerable<DataImportAudit> audits);

        /// <summary>
        /// Proxy for Insert Audit. Converts array to list.
        /// </summary>
        /// <param name="audits">List of all the audits that need to be added to audit table.</param>
        void InsertAudit(params DataImportAudit[] audits);


        /// <summary>
        /// Get most recent entered record
        /// </summary>
        /// <returns></returns>
        /// <param name="companyName">CompanyName.</param>
        DataImportHistory GetLatestDataImportHistoryRecordByCompanyName(string companyName);

        int? GetLatestBatchNumber();

    }
}