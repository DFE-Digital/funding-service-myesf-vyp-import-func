using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using ViewYourPayments.Core.AppConfig;
using ViewYourPayments.Core.Enums.Logging;
using ViewYourPayments.Core.Interfaces;

namespace ViewYourPayments.Core.Services
{
    public class ApplicationInsightsLogger : IApplicationLogger
    {
        /// <summary>
        /// The Application Insights Instrumentation Key.
        /// </summary>
        public static string InstrumentationKey { get; set; }

        /// <summary>
        /// The telemetry client to use for logging to Application Insights.
        /// </summary>
        private readonly TelemetryClient _telemetryClient;

        /// <summary>
        /// Properties that will be marked against the log entries.
        /// </summary>
        protected IDictionary<string, string> Properties { get; set; }

        /// <summary>
        /// Create a new instance of an Application Insights Logger with the given key and properties.
        /// </summary>
        /// <param name="instrumentationKey">The instrumentation key.</param>
        /// <param name="properties">A dictionary containing the logging properties.</param>
        public ApplicationInsightsLogger(TelemetryClient telemetryClient, IOptions<AppSettings> appSettings)
        {
            _telemetryClient = telemetryClient;
            this.Properties = new Dictionary<string, string>
                {
                    {"user", "testUser"},
                    {"environment", appSettings.Value.Environment},
                    {"component", "ViewYourPayment.Web"},
                };
        }

        /// <summary>
        /// Create a new instance of an Application Insights Logger with the given key and properties.
        /// </summary>
        /// <param name="instrumentationKey">The instrumentation key.</param>
        /// <param name="properties">A dictionary containing the logging properties.</param>
        public ApplicationInsightsLogger(TelemetryClient telemetryClient, string environment, string applicationName)
        {
            _telemetryClient = telemetryClient;
            this.Properties = new Dictionary<string, string>
                {
                    {"user", "testUser"},
                    {"environment", environment},
                    {"component", applicationName},
                };
        }

        /// <summary>
        /// Write a log message.
        /// </summary>
        /// <param name="message">The message string to log.</param>
        /// <param name="category">The log category.</param>
        /// <param name="severityLevel">The log severity.</param>
        public void LogTrace(string message, BusinessArea category, LoggingSeverity severityLevel = LoggingSeverity.Information)
        {
            IDictionary<string, string> properties = new Dictionary<string, string>(Properties);
            try
            {
                properties.Add(new KeyValuePair<string, string>("DomainArea", category.ToString()));
                _telemetryClient.TrackTrace(message, (SeverityLevel)severityLevel, properties);
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Log an exception.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        public void LogException(Exception exception)
        {
            try
            {
                try
                {
                    _telemetryClient.TrackException(exception, Properties);
                }
                catch (Exception ex)
                {
                    _telemetryClient.TrackException(ex, Properties);
                }
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Log an exception and then throw it.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        public void LogAndThrowException(Exception exception)
        {
            try
            {
                try
                {
                    _telemetryClient.TrackException(exception, Properties);
                }
                catch (Exception ex)
                {
                    _telemetryClient.TrackException(ex, Properties);
                }
            }
            catch
            {
                // ignored
            }
            finally
            {
                throw exception;
            }
        }

        /// <summary>
        /// log an application exception to Application Insights.
        /// </summary>
        /// <param name="exception">Exception thrown by application.</param>
        /// <param name="additionalMessage">Additional information associated with exception.</param>
        public void LogException(Exception exception, string additionalMessage)
        {
            const string messageKey = "Message";
            string newMessage = $"{additionalMessage}; ";

            var dictionary = Properties;

            if (dictionary.ContainsKey(messageKey))
            {
                dictionary[messageKey] += newMessage;
            }
            else
            {
                dictionary.Add(messageKey, newMessage);
            }

            _telemetryClient.TrackException(exception, dictionary);
            _telemetryClient.Flush();
        }

        public void LogWarn(string message)
        {
            throw new NotImplementedException();
        }

        public void LogInfo(string message)
        {
            _telemetryClient.TrackTrace(message, SeverityLevel.Information);
        }

    }
}
