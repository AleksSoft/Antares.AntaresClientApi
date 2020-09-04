using Assets.Client;

namespace AntaresClientApi.Common.Configuration
{
    public class AppConfig
    {
        public DbConfig Db { get; set; }

        public MyNoSqlConfig MyNoSqlServer { get; set; }

        public SessionConfig SessionConfig { get; set; }
    }

    public class MyNoSqlConfig
    {
        public string ReaderServiceUrl { get; set; }
        public string WriterServiceUrl { get; set; }
    }

    public class SessionConfig
    {
        public int ExpirationTimeInMins { get; set; }

        public int RegistrationExpirationTimeInMins { get; set; }
    }
}
