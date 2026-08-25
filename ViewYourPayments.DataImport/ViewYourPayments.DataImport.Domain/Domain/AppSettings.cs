namespace ViewYourPayments.DataImport.Domain
{
    /// <summary>
    /// Retrieve settings from configuration file.
    /// </summary>
    public class AppSettings
    {

        public string ConnectionString { get; set; }

        public DataImportSetting DataImportSetting { get; set; }

    }
}