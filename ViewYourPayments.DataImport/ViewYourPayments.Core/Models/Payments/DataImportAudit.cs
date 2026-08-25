using System;
using System.ComponentModel.DataAnnotations;
using ViewYourPayments.Core.Enums.Logging;

namespace ViewYourPayments.Core.Models.Payments
{
    public class DataImportAudit
    {
        /// <summary>
        /// Remittance Audit Record
        /// </summary>
        /// <param name="severity">Severity level for the audit log.</param>
        /// <param name="user">The User.</param>
        /// <param name="message">What took place.</param>
        public DataImportAudit(LoggingSeverity severity, string user, string message)
        {
            Severity = severity;
            User = user;
            Message = message;
        }

        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Severity Level of the Audit.
        /// </summary>
        public LoggingSeverity Severity { get; set; }

        /// <summary>
        /// The user.
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// What took place.
        /// </summary>
        public string Message { get; set; }

        public DateTime CreatedOn { get; set; }

        public override string ToString()
        {
            return $"DataImport Audit:[Severity: {Severity}, User: {User}, Message: '{Message}']";
        }
    }
}
