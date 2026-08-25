using AutoMapper;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using ViewYourPayments.Core.Enums.Logging;
using ViewYourPayments.Core.Interfaces;
using ViewYourPayments.Core.Services;
using ViewYourPayments.DataImport.Domain;
using ViewYourPayments.DataImport.Domain.Clients;
using ViewYourPayments.DataImport.Domain.DomainClients;
using ViewYourPayments.DataImport.Domain.Services;

namespace ViewYourPayments.DataImportFunction
{
    public class DataImportFunction
    {

        public readonly IConfiguration _configuration;
        public readonly IMapper _mapper;

        public DataImportFunction(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }

        [Function("RetrievePaymentsFromNavApi")]
        public void Run([TimerTrigger("%TimerInterval%")] TimerInfo timer)
        {
            var insightInstrumentKey = _configuration.GetValue<string>("APPINSIGHTS_INSTRUMENTATIONKEY");
            var environment = _configuration.GetValue<string>("Environment");

            if (String.IsNullOrWhiteSpace(insightInstrumentKey))
            {
                throw new Exception($"Error: Configuration setting: [APPINSIGHTS_INSTRUMENTATIONKEY] missing or empty");
            }
            if (String.IsNullOrWhiteSpace(environment))
            {
                throw new Exception($"Error: Configuration setting: [Environment] missing or empty");
            }

            var dataImportSetting = _configuration.GetSection("DataImportSetting").Get<DataImportSetting>();
            var connectionString = _configuration.GetConnectionString("ViewYourPaymentsDbContext");

            var logger = GetLogger(environment, insightInstrumentKey);

            logger.LogInfo($"Azure Function run Started at V3.0: {DateTime.Now}");

            if (!EnsureParametersExist(dataImportSetting, connectionString, logger))
            {
                throw new ArgumentException("Error: Missing configuration setting(s)");
            }

            logger.LogInfo($"Calling Remittance APIM. {environment}");

            CallRemittanceEndPointAndPersistNewRemittances(dataImportSetting, connectionString, logger);

            logger.LogInfo($"Azure Function run Completed at: {DateTime.Now}");
        }


        #region Helper Methods

        /// <summary>
        /// Ensure all environment variables exist.
        /// </summary>
        /// <param name="dataImportSetting">application settings.</param>
        /// <param name="log">Instance of Logger.</param>
        /// <returns>return true if all parameters exist and have values.</returns>
        private bool EnsureParametersExist(DataImportSetting dataImportSetting, string connectionString,
                    IApplicationLogger log)
        {
            var validSettings = true;
            if (String.IsNullOrWhiteSpace(connectionString))
            {
                log.LogInfo($"Error: Configuration setting: [connectionString] missing or empty");
                validSettings = false;
            }

            if (String.IsNullOrWhiteSpace(dataImportSetting.NavApiBaseUrl))
            {
                log.LogInfo($"Error: Configuration setting: [apim:NavApiBaseUrl] missing or empty");
                validSettings = false;
            }

            if (String.IsNullOrWhiteSpace(dataImportSetting.NavApiVersionNumber))
            {
                log.LogInfo($"Error: Configuration setting: [apim:NavApiVersionNumber] missing or empty");
                validSettings = false;
            }

            if (String.IsNullOrWhiteSpace(dataImportSetting.NavApiSubscriptionKey))
            {
                log.LogInfo($"Error: Configuration setting: [apim:NavApiubscriptionKey] missing or empty");
                validSettings = false;
            }
            return validSettings;
        }

        /// <summary>
        /// Call remittance end-point and persist all new remittances.
        /// </summary>
        /// <param name="dataImportSetting">application settings.</param>
        private void CallRemittanceEndPointAndPersistNewRemittances(DataImportSetting dataImportSetting, string connectionString, IApplicationLogger logger)
        {
            var message = "Start application to pull data from the APIM remittance Api.";

            logger.LogTrace(message, BusinessArea.DataImport, LoggingSeverity.Information);

            var api = new ApimApi(new HttpService(), logger, dataImportSetting.NavApiBaseUrl, dataImportSetting.NavApiVersionNumber, dataImportSetting.NavApiSubscriptionKey);
            var deserializer = new DeSerializeApimJson(logger);
            var viewYourPaymentsDbClient = new ViewYourPaymentsDbClient(new DataService(), logger, connectionString, _mapper);

            DataImportService dataImportService = new DataImportService(api, deserializer, viewYourPaymentsDbClient, logger, dataImportSetting);

            dataImportService.CallApimAndInsertNewRemittances();

            message = "Application to pull data from the APIM remittance Api finished.";
            logger.LogTrace(message, BusinessArea.DataImport, LoggingSeverity.Information);
        }

        /// <summary>
        /// Get instance of Application Insight logger.
        /// </summary>
        /// <returns>Instance of ILogger.</returns>
        /// <param name="environment">current environment.</param>
        /// <param name="appInsightsKey">appInsightsKey for logging.</param>
        private IApplicationLogger GetLogger(string environment, string appInsightsKey)
        {
            var telemetryConfig = new TelemetryConfiguration { ConnectionString = $"InstrumentationKey={appInsightsKey};" };
            var telemetryClient = new TelemetryClient(telemetryConfig);
            return new ApplicationInsightsLogger(telemetryClient, environment, "ViewYourPayments.DataImport");
        }
        #endregion

    }
}
