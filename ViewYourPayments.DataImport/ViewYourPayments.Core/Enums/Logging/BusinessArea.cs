namespace ViewYourPayments.Core.Enums.Logging
{
    /// <summary>
    /// Enumeration representing possible logging categories.
    /// </summary>
    public enum BusinessArea
    {
        /// <summary>
        /// Log information releted to provider search.
        /// </summary>
        ProviderSearch = 0,
        /// <summary>
        /// Log information releted to data search.
        /// </summary>
        DataSearchService = 1,
        /// <summary>
        /// Log information releted to user authentication.
        /// </summary>
        AuthenticationService = 2,
        VypWebApplication = 3,
        DataImport = 4
    }
}
