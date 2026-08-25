using System;
using System.Data.SqlClient;

namespace ViewYourPayments.DataImport.Domain.Services
{
    /// <summary>
    /// Interface for a Data Service.
    /// </summary>
    public interface IDataService
    {

        /// <summary>
        /// Retrieve Sql Connection.
        /// </summary>
        /// <param name="uri">Sql connection string.</param>
        /// <returns>an sql connection.</returns>
        SqlConnection GetSqlConnection(String uri);
    }
}
