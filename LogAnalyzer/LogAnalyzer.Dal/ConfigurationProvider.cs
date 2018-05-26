using System.Configuration;

namespace LogAnalyzer.Dal
{
    public class ConfigurationProvider
    {
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        public string ConnectionString => ConfigurationManager.AppSettings["DomainsLogging_MongoDB"];

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <value>
        /// The database.
        /// </value>
        public string Database => ConfigurationManager.AppSettings["Database"];
    }
}
