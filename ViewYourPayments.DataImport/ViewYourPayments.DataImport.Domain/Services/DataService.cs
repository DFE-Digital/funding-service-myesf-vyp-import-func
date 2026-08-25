using System.Data.SqlClient;

namespace ViewYourPayments.DataImport.Domain.Services
{
    /// <summary>
    /// Proxy for SqlConnection
    /// </summary>
    public class DataService : IDataService
    {
        /// <summary>
        /// Retrieve Sql Connection.
        /// </summary>
        /// <param name="uri">Sql connection string.</param>
        /// <returns>an sql connection</returns>
        public SqlConnection GetSqlConnection(string uri)
        {
            return new SqlConnection(uri);
        }

    }
}
