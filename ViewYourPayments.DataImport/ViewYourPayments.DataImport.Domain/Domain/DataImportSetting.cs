using ViewYourPayments.DataImport.Domain.Domain;

namespace ViewYourPayments.DataImport.Domain
{
    /// <summary>
    /// Represent data import settings from appsetting.json
    /// </summary>
    public class DataImportSetting
    {
        public bool UseFinanceAPI { get; set; }

        public string DataImportFromDate { get; set; }

        public string DataImportToDate { get; set; }

        public bool RunAsOneTimeJobWithConfigDates { get; set; }

        public string NavApiBaseUrl { get; set; }

        public string NavApiSubscriptionKey { get; set; }

        public string NavApiVersionNumber { get; set; }

        public string Company { get; set; }

        public string DimensionCode { get; set; }

        public string DimensionValues { get; set; }

        public int RetryCounter { get; set; }

        public int RetryGapDurationMilliseconds { get; set; }

        public NavApiCompanySettings[] NavApiCompaniesSettings { get; set; }
    }
}