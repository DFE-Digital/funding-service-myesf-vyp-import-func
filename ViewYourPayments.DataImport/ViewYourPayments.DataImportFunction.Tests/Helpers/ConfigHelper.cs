using Microsoft.Extensions.Configuration;
using System.IO;

namespace ViewYourPayments.DataImportFunction.Tests.Helpers
{
    public class ConfigHelper
    {
        private static IConfiguration _configuration;

        public static string GetConnectionString(string connectStringName)
        {
            if (_configuration == null)
            {
                var builder = new ConfigurationBuilder()
                           .SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                           .AddEnvironmentVariables();
                _configuration = builder.Build();
                return _configuration.GetConnectionString(connectStringName);
            }
            return _configuration.GetConnectionString(connectStringName);
        }
    }
}
