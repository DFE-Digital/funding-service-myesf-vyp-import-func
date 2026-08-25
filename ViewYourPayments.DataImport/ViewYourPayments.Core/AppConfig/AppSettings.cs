namespace ViewYourPayments.Core.AppConfig
{
    public class AppSettings
    {
        public string Environment { get; set; }
        public string AllowedHosts { get; set; }
        public string ConnectionString { get; set; }
        public string APPINSIGHTS_INSTRUMENTATIONKEY { get; set; }
        public AzureAdOptions AzureAdOptions { get; set; }
    }
}
