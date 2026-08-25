using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ViewYourPayments.Core.Enums.Logging;
using ViewYourPayments.Core.Interfaces;
using ViewYourPayments.Core.Models.Payments;
using ViewYourPayments.DataImport.Domain.DomainClients;
using ViewYourPayments.DataImport.Domain.Services;

namespace ViewYourPayments.DataImportFunction.Tests.Helpers
{
    /// <summary>
    /// Extended RemittanceClient to allow for test methods.
    /// </summary>
    public class MockViewYourPaymentsDbClient : ViewYourPaymentsDbClient
    {
        private new readonly string _connectionString;

        public MockViewYourPaymentsDbClient(IDataService dataService, IApplicationLogger logger, string connectionString, IMapper mapper) : base(dataService, logger, connectionString, mapper)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Delete all records from remittance Audits, Details and Summary tables.
        /// </summary>
        public void CleanDB()
        {
            var deleteQuery = "Delete from [dbo].[DataImportAudits];" +
                "Delete from [dbo].[PaymentLines];" +
                "Delete from [dbo].[PaymentSummaries];" +
                "Delete from [dbo].[PaymentLinesStaging];" +
                "Delete from [dbo].[PaymentSummariesStaging];" +
                "Delete from [dbo].[DataImportHistories]";

            using (SqlConnection sqlConnection =
                            _dataService.GetSqlConnection(_connectionString))
            {
                sqlConnection.Open();

                SqlCommand command = new SqlCommand(deleteQuery, sqlConnection);

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Retrieve Count of Records in Remittance Summary Table.
        /// </summary>
        /// <returns>Record Count.</returns>
        public int GetRemittanceSummaryCount()
        {
            var auditCountQuery = "SELECT Count(*) FROM [dbo].[PaymentSummaries];";

            var count = CountQuery(auditCountQuery);

            return count;
        }

        /// <summary>
        /// Retrieve Count of Records in Remittance Summary Staging Table.
        /// </summary>
        /// <returns>Record Count.</returns>
        public int GetRemittanceSummaryStagingCount()
        {
            var auditCountQuery = "SELECT Count(*) FROM [dbo].[PaymentSummariesStaging];";

            var count = CountQuery(auditCountQuery);

            return count;
        }

        /// <summary>
        /// Retrieve Count of Records in Remittance Details Table.
        /// </summary>
        /// <returns>Record Count.</returns>
        public int GetRemittanceDetailCount()
        {
            var detailCountQuery = "SELECT Count(*) FROM [dbo].[PaymentLines];";

            var count = CountQuery(detailCountQuery);

            return count;
        }

        /// <summary>
        /// Retrieve Count of Records in Remittance Details Table.
        /// </summary>
        /// <returns>Record Count.</returns>
        public int GetRemittanceDetailStagingCount()
        {
            var detailCountQuery = "SELECT Count(*) FROM [dbo].[PaymentLinesStaging];";

            var count = CountQuery(detailCountQuery);

            return count;
        }

        /// <summary>
        /// Retrieve Count of Records in Remittance Audit Table.
        /// </summary>
        /// <returns>Record Count.</returns>
        public int GetDataImportAuditCount()
        {
            var summaryCountQuery = "SELECT Count(*) FROM [dbo].[DataImportAudits];";

            var count = CountQuery(summaryCountQuery);

            return count;
        }

        private int CountQuery(string query)
        {
            using (SqlConnection sqlConnection =
                            _dataService.GetSqlConnection(_connectionString))
            {
                sqlConnection.Open();

                SqlCommand command = new SqlCommand(query, sqlConnection);

                SqlDataReader reader = command.ExecuteReader();
                reader.Read();

                var count = (int)reader[0];

                return count;
            }
        }

        /// <summary>
        /// Retrieves All Audits in the Remittance Audit Table. 
        /// </summary>
        /// <returns>Audit List.</returns>
        private List<DataImportAudit> GetAudits()
        {

            using (SqlConnection sqlConnection =
                            _dataService.GetSqlConnection(_connectionString))
            {
                sqlConnection.Open();

                var auditBase = "SELECT [Id], [Severity], [User], [Message], [Action], [CreatedOn]" +
            " FROM [dbo].[DataImportAudits]";

                SqlCommand command = new SqlCommand(auditBase, sqlConnection);


                SqlDataReader reader = command.ExecuteReader();
                var audits = new List<DataImportAudit>();
                while (reader.Read())
                {
                    var audit = ConvertReaderToAudit(reader);
                    audits.Add(audit);
                }

                return audits;
            }
        }

        public string GetBudgetGroupDescription(string ContractCode)
        {
            using (SqlConnection sqlConnection =
                            _dataService.GetSqlConnection(_connectionString))
            {
                sqlConnection.Open();

                var auditBase = "SELECT BudgetGroupDescription from dbo.BudgetGroups Where ContractCodePrefix ='" + ContractCode.Trim()
                                                                    + "' and Len(" + ContractCode.Trim() + ") = Len(ContractCodePrefix)";

                SqlCommand command = new SqlCommand(auditBase, sqlConnection);

                var description = command.ExecuteScalar()?.ToString();

                return description;
            }
        }

        /// <summary>
        /// Retrieve Summary and associated details records for provided PayementId.
        /// </summary>
        /// <param name="paymentId">Id of the Payment to retrieve.</param>
        /// <returns></returns>
        public List<PaymentSummary> GetRemittanceViaPaymentId(string paymentId)
        {
            using (SqlConnection sqlConnection =
                _dataService.GetSqlConnection(_connectionString))
            {

                var query = "SELECT [Summary].[Id] as Summary_Id, [Summary].[Ukprn], [Summary].[PaymentTotal] as Summary_PaymentTotal, [Summary].[PaymentDate], [Summary].[PaymentIdentifier],"
                            + "[Detail].[Id] as Detail_Id, [Detail].[Contract], [Detail].[PaymentLineDescription], [Detail].[PaymentLineGroupDescription], [Detail].[LineAmount] as Detail_PaymentTotal, [Detail].[PaymentSummaryId], [Detail].[PaymentLineIdentifier], [Detail].[BudgetGroup] "
                            + "FROM [dbo].[PaymentSummaries] Summary "
                            + "Left Join [dbo].[PaymentLines] Detail on[Detail].[PaymentSummaryId] = [Summary].[Id] "
                            + "Where [Summary].[PaymentIdentifier] = @PaymentId;";

                sqlConnection.Open();

                SqlCommand command = new SqlCommand(query, sqlConnection);

                command.Parameters.AddWithValue("@paymentId", paymentId);

                SqlDataReader reader = command.ExecuteReader();

                List<PaymentSummary> paymentSummaries = new List<PaymentSummary>();

                PaymentSummary currentSummary = null;

                while (reader.Read())
                {
                    if (currentSummary == null)
                    {
                        currentSummary = ConvertReaderToSummary(reader);
                    }
                    else if (!((string)reader[8]).Equals(currentSummary.PaymentIdentifier))
                    {
                        paymentSummaries.Add(currentSummary);
                        currentSummary = ConvertReaderToSummary(reader);
                    }

                    currentSummary.PaymentLine.Add(ConvertReaderToDetail(reader));
                }
                if (currentSummary != null)
                {
                    paymentSummaries.Add(currentSummary);
                }
                reader.Close();
                command.Dispose();

                return paymentSummaries;
            }

        }
        /// <summary>
        /// Retrieves all payement Summaries and related payments.
        /// </summary>
        /// <returns>List of Payments</returns>
        public List<PaymentSummary> GetAllPayments()
        {
            using (SqlConnection sqlConnection =
                _dataService.GetSqlConnection(_connectionString))
            {

                var query = "SELECT [Summary].[Id] as Summary_Id, [Summary].[Ukprn], [Summary].[PaymentTotal] as Summary_PaymentTotal, [Summary].[PaymentMethodId], [Summary].[PaymentDate], [Summary].[PaymentIdentifier],"
                            + "[Detail].[Id] as Detail_Id, [Detail].[Contract], [Detail].[DocumentNumber], [Detail].[LineDescription], [Detail].[PaymentTotal] as Detail_PaymentTotal, [Detail].[PaymentSummaryId], [Detail].[PaymentLineIdentifier] "
                            + "FROM [dbo].[PaymentSummaries] Summary "
                            + "Left Join [dbo].[PaymentLines] Detail on[Detail].[PaymentSummaryId] = [Summary].[Id] ";

                sqlConnection.Open();

                SqlCommand command = new SqlCommand(query, sqlConnection);

                SqlDataReader reader = command.ExecuteReader();

                List<PaymentSummary> paymentSummaries = new List<PaymentSummary>();

                PaymentSummary currentSummary = null;

                while (reader.Read())
                {
                    if (currentSummary == null)
                    {
                        currentSummary = ConvertReaderToSummary(reader);
                    }
                    else if (!((string)reader[8]).Equals(currentSummary.PaymentIdentifier))
                    {
                        paymentSummaries.Add(currentSummary);
                        currentSummary = ConvertReaderToSummary(reader);
                    }

                    currentSummary.PaymentLine.Add(ConvertReaderToDetail(reader));
                }
                paymentSummaries.Add(currentSummary);

                reader.Close();
                command.Dispose();

                return paymentSummaries;
            }
        }

        private PaymentSummary ConvertReaderToSummary(SqlDataReader reader)
        {
            var summary = new PaymentSummary
            {
                PaymentIdentifier = (int)reader["PaymentIdentifier"],
                Ukprn = reader["Ukprn"].ToString(),
                PaymentTotal = (decimal)reader["Summary_PaymentTotal"],
                PaymentDate = (DateTime)reader["PaymentDate"]
            };

            return summary;
        }

        private PaymentLine ConvertReaderToDetail(SqlDataReader reader)
        {
            var details = new PaymentLine
            {
                Contract = (string)reader["Contract"],
                PaymentLineDescription = (string)reader["PaymentLineDescription"],
                LineAmount = (decimal)reader["Detail_PaymentTotal"],
                PaymentLineIdentifier = (string)reader["PaymentLineIdentifier"],
                BudgetGroup = (string)reader["BudgetGroup"],
                PaymentLineGroupDescription = (string)reader["PaymentLineGroupDescription"],
            };

            return details;
        }

        private DataImportAudit ConvertReaderToAudit(SqlDataReader reader)
        {
            var severity = (LoggingSeverity)reader[0];
            var user = (string)reader[1];
            var message = (string)reader[2];

            return new DataImportAudit(severity, user, message);
        }

    }
}