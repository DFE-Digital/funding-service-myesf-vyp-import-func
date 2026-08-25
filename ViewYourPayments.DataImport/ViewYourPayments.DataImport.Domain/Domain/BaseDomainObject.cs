using System;
using System.Collections.Generic;
using ViewYourPayments.Core.Enums.Logging;
using ViewYourPayments.Core.Interfaces;
using ViewYourPayments.Core.Models.Payments;
using ViewYourPayments.DataImport.Domain.DomainClients;

namespace ViewYourPayments.DataImport.Domain
{
    /// Base class for all objects within the current domain.
    /// </summary>
    [Serializable]
    public class BaseDomainObject
    {
        /// <summary>
        /// Application Logging instance.
        /// </summary>
        private readonly IApplicationLogger _logger;

        /// <summary>
        /// List of audits.
        /// </summary>
        public static List<string> AuditList { get; protected set; }

        /// <summary>
        /// Parameter constructor.
        /// </summary>
        /// <param name="logger">logger instance.</param>
        public BaseDomainObject(IApplicationLogger logger)
        {
            _logger = logger;
            AuditList = new List<string>();
        }

        /// <summary>
        /// Log trace to logger.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="severity">severity of message..</param>
        public void LogTrace(string message, LoggingSeverity severity = LoggingSeverity.Information)
        {
            _logger.LogTrace(message, BusinessArea.DataImport, severity);
        }

        /// <summary>
        /// Log exception to logger.
        /// </summary>
        /// <param name="ex">Exception.</param>
        /// <param name="message">Message related to exception.</param>
        public void LogException(Exception ex, string message = "")
        {
            _logger.LogException(ex, message);
        }

        /// <summary>
        /// Log exception to logger and audit exception.
        /// </summary>
        /// <param name="ex">Exception.</param>
        /// <param name="message">Error message.</param>
        public void LogExceptionAndAudit(Exception ex, string message = "")
        {
            LogException(ex, message);

            if (String.IsNullOrWhiteSpace(message))
            {
                AuditList.Add($"Error: {ex.Message}");
            }
            else
            {
                AuditList.Add($"{message}");
            }
        }

        /// <summary>
        /// Log trace to logger and audit message.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="severity">severity of message.</param>
        public void LogTraceAndAudit(string message, LoggingSeverity severity = LoggingSeverity.Information)
        {
            LogTrace(message, severity);
            AuditList.Add(message);
        }

        /// <summary>
        /// Persist all collated audits to database client.
        /// </summary>
        /// <param name="remittanceDatabaseClient">database client instance.</param>
        public void PersistAuditRecords(IViewYourPaymentsDbClient remittanceDatabaseClient)
        {
            var audits = new List<DataImportAudit>();

            foreach (var message in AuditList)
            {
                audits.Add(new DataImportAudit(LoggingSeverity.Information, "System", message));
            }

            remittanceDatabaseClient.InsertAudit(audits);

            AuditList.Clear();
        }
    }
}
