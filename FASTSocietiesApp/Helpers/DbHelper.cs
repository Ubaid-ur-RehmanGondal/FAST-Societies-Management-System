using System.Configuration;

namespace FASTSocietiesApp.Helpers
{
    /// <summary>
    /// Centralised access to the SQL Server connection string defined in
    /// App.config under the "FASTSocietiesDB" key. The connection string
    /// must NEVER be hard-coded anywhere else in the project.
    /// </summary>
    public static class DbHelper
    {
        private const string ConnectionStringName = "FASTSocietiesDB";

        /// <summary>
        /// Returns the connection string from App.config. Throws AppException
        /// if it is missing or empty.
        /// </summary>
        public static string GetConnectionString()
        {
            ConnectionStringSettings? settings =
                ConfigurationManager.ConnectionStrings[ConnectionStringName];

            if (settings == null || string.IsNullOrWhiteSpace(settings.ConnectionString))
            {
                throw new AppException(
                    $"Connection string '{ConnectionStringName}' is not configured in App.config.");
            }

            return settings.ConnectionString;
        }
    }
}
