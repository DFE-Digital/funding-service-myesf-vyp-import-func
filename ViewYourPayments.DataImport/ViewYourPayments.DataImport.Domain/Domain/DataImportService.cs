
using System;
using System.Collections.Generic;
using System.Linq;
using ViewYourPayments.Core.Interfaces;
using ViewYourPayments.Core.Models.Payments;
using ViewYourPayments.DataImport.Domain.Clients;
using ViewYourPayments.DataImport.Domain.Domain;
using ViewYourPayments.DataImport.Domain.DomainClients;

namespace ViewYourPayments.DataImport.Domain
{
    /// <summary>
    /// This is the orchestration class that call related domain object.
    /// </summary>
    public class DataImportService : BaseDomainObject
    {
        /// <summary>
        /// Application Logger instance.
        /// </summary>
        private readonly IApplicationLogger _logger;

        /// <summary>
        /// Remittance database client.
        /// </summary>
        private readonly IViewYourPaymentsDbClient _remittanceDatabaseClient;

        /// <summary>
        /// Apim Api Client.
        /// </summary>
        private readonly IApiClient _apiClient;

        /// <summary>
        /// Deserialises JSON into domain objects.
        /// </summary>
        private readonly IDeSerializeApimJson _deserializer;

        /// <summary>
        /// DataImport settings to read from appsetting.json
        /// </summary>
        private readonly DataImportSetting _appSettings;

        /// <summary>
        /// Argument Constructor.
        /// </summary>
        /// <param name="apiClient">Api Client.</ param >
        /// <param name="deserializer">deSerializes apim JSON.</param>
        /// <param name="viewYourPaymentsDbClient">View your payments Database Client.</param>
        /// <param name="logger">Application Logger.</param>
        public DataImportService(IApiClient apiClient, IDeSerializeApimJson deserializer,
            IViewYourPaymentsDbClient viewYourPaymentsDbClient,
            IApplicationLogger logger,
            DataImportSetting appSettings) : base(logger)
        {
            _remittanceDatabaseClient = viewYourPaymentsDbClient;
            _apiClient = apiClient;
            _deserializer = deserializer;
            _logger = logger;
            _appSettings = appSettings;
        }

        /// <summary>
        /// Call Apim end-point and add new remittances to database.
        /// </summary>
        public void CallApimAndInsertNewRemittances()
        {
            try
            {
                LogTraceAndAudit($"Remittance run started");

                var useFinanceApi = _appSettings.UseFinanceAPI;

                if (!useFinanceApi)
                {
                    LogTraceAndAudit($"Finance API is turned OFF. Exiting");
                }
                else
                {
                    var batchNumber = (_remittanceDatabaseClient.GetLatestBatchNumber() ?? 0) + 1;
                    LogTraceAndAudit($"Current Batch Number is: {batchNumber}");
                    var companiesList = _appSettings.Company.Split(',');
                    for (int i = 0; i < companiesList.Length; i++)
                    {
                        var companySettings = _appSettings.NavApiCompaniesSettings.FirstOrDefault(x => x.CompanyName == companiesList[i].Trim());
                        var latestDataImportHistory = _remittanceDatabaseClient.GetLatestDataImportHistoryRecordByCompanyName(companiesList[i].Trim());
                        RetrieveAndSavePaymentsInformation(companySettings, _appSettings.RetryCounter, _appSettings.RetryGapDurationMilliseconds, batchNumber, latestDataImportHistory);
                        LogTraceAndAudit($"Data import finish for {companiesList[i]}");
                    }

                    _apiClient.DisposeApi();
                }
            }
            catch (Exception ex)
            {
                LogExceptionAndAudit(ex);
                throw;
            }
            finally
            {
                LogTraceAndAudit($"Remittance run finished");
                base.PersistAuditRecords(_remittanceDatabaseClient);
            }
        }

        /// <summary>
        /// Retrieve and save Remittance payments.
        /// </summary>
        /// <param name="company">company.</param>
        /// <param name="retryCount">retryCount.</param>
        /// <param name="retryInterval">retryInterval.</param>
        /// <param name="batchNumber">batchNumber.</param>
        /// <param name="dataImportHistory">dataImportHistory.</param>
        /// <returns>List of all new payments.</returns>
        private void RetrieveAndSavePaymentsInformation(NavApiCompanySettings companySettings, int retryCount, int retryInterval, int batchNumber, DataImportHistory dataImportHistory)
        {
            var completePaymentsList = new List<PaymentSummary>();
            var totalRemittancesForCompany = 0;
            string skipToken = null;
            DateTime BatchExecutionDate = DateTime.UtcNow;
            LogTraceAndAudit($"Get Remittances for company {companySettings.CompanyName}");

            int incorrectRecords;
            do
            {
                var paymentApiRequest = GetApiRequest(_appSettings, dataImportHistory, companySettings, skipToken);

                var result = _apiClient.MakeRequest(paymentApiRequest, retryCount, retryInterval);
                var responseBody = result.Result;

                LogTrace($"UriCallWithParameters: {_apiClient.UriCallWithParameters}");
                LogTrace($"Length of response: {responseBody.Length}.");

                var deSerializeResponse = _deserializer.GetPaymentSummaries(responseBody);

                LogTrace($"Payment summary Count: {deSerializeResponse.Payments?.Count}.");

                if (!deSerializeResponse.Payments.Any()) return;

                completePaymentsList.AddRange(deSerializeResponse.Payments);
                skipToken = deSerializeResponse.SkipToken;
                totalRemittancesForCompany += deSerializeResponse.Payments.Count;
                incorrectRecords = deSerializeResponse.FailedCount;
                var newDataImportHistory = new DataImportHistory
                {
                    BatchId = batchNumber,
                    RunFromDate = paymentApiRequest.FromDate,
                    RunToDate = paymentApiRequest.ToDate,
                    SkipTokenNumber = skipToken,
                    RequestedUrl = _apiClient.UriCallWithParameters,
                    CompanyName = companySettings.CompanyName
                };
                _remittanceDatabaseClient.InsertRemittances(deSerializeResponse.Payments, newDataImportHistory, companySettings, BatchExecutionDate);
                PersistAuditRecords(_remittanceDatabaseClient);
            }
            while (skipToken != null);

            LogTraceAndAudit($"Company {companySettings.CompanyName} has retrieved {totalRemittancesForCompany} remittances. There was {incorrectRecords} failed records. ");
        }

        /// <summary>
        /// Get api request parameter for selected secenario
        /// </summary>
        /// <param name="dataImportSetting"></param>
        /// <param name="latestDataImportHistory"></param>
        /// <param name="company"></param>
        /// <param name="newSkipToken"></param>
        /// <returns>PaymentApiRequest</returns>
        public PaymentApiRequest GetApiRequest(DataImportSetting dataImportSetting, DataImportHistory latestDataImportHistory, NavApiCompanySettings companySetting, string newSkipToken)
        {
            var dimensionCode = companySetting.IncludeDimensionQuery ? dataImportSetting.DimensionCode : null;
            var dimensionValue = companySetting.IncludeDimensionQuery ? dataImportSetting.DimensionValues : null;
            var skipToken = GetSkipToken(dataImportSetting, latestDataImportHistory);
            DateTime? endDate = GetEndDate(dataImportSetting, latestDataImportHistory);
            DateTime? startDate = GetStartDate(dataImportSetting, latestDataImportHistory);

            if (!startDate.HasValue || !endDate.HasValue)
            {
                var ex = new ArgumentException("Start date and end date not provided");
                LogExceptionAndAudit(ex);
                throw ex;
            }

            if (startDate > endDate)
            {
                var errorMessage = $"Error: Start-date is greater than end-date. [DateFrom: {startDate}], [DateTo: {endDate}]";
                var ex = new ArgumentException(errorMessage);
                LogExceptionAndAudit(ex, errorMessage);
                throw ex;
            }
            return new PaymentApiRequest
            {
                FromDate = startDate.Value,
                ToDate = endDate.Value,
                SkipToken = newSkipToken ?? skipToken,
                DimensionCode = dimensionCode,
                DimensionValue = dimensionValue,
                Company = companySetting.CompanyName,
                VersionNumber = dataImportSetting.NavApiVersionNumber
            };
        }
        public DateTime? GetStartDate(DataImportSetting dataImportSetting, DataImportHistory latestDataImportHistory)
        {
            if (dataImportSetting.RunAsOneTimeJobWithConfigDates) return DateTime.Parse(_appSettings.DataImportFromDate);
            if (latestDataImportHistory == null) return DateTime.Parse(_appSettings.DataImportFromDate);
            if (string.IsNullOrWhiteSpace(latestDataImportHistory.SkipTokenNumber)) return latestDataImportHistory.RunToDate;
            return latestDataImportHistory.RunFromDate;
        }

        public DateTime? GetEndDate(DataImportSetting dataImportSetting, DataImportHistory latestDataImportHistory)
        {
            if (dataImportSetting.RunAsOneTimeJobWithConfigDates) return DateTime.Parse(_appSettings.DataImportToDate);
            if (latestDataImportHistory == null) return DateTime.UtcNow;
            if (string.IsNullOrWhiteSpace(latestDataImportHistory.SkipTokenNumber)) return DateTime.UtcNow;
            return latestDataImportHistory.RunToDate;
        }

        public string GetSkipToken(DataImportSetting dataImportSetting, DataImportHistory latestDataImportHistory)
        {
            if (dataImportSetting.RunAsOneTimeJobWithConfigDates) return null;
            if (latestDataImportHistory?.SkipTokenNumber != null) return latestDataImportHistory?.SkipTokenNumber;
            return null;
        }
    }
}