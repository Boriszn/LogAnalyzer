using System.Configuration;

namespace LogAnalyzer.Dal
{
    public class ConfigurationProvider
    {
        public string ConnectionString => ConfigurationManager.AppSettings["DomainsLogging_MongoDB"];
    }
}
