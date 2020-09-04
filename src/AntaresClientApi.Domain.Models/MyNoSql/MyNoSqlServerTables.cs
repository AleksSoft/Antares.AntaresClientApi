namespace AntaresClientApi.Domain.Models.MyNoSql
{
    public static class MyNoSqlServerTables
    {
        public const string SessionsTableName = "antaresapisessions";
        public const string RegistrationTokenTableName = "registrationtoken";
        public const string ClientWalletTableName = "clientwallet";
        public const string ClientWalletIndexedByIdTableName = "clientwallet-indexed-by-id";

        public const string PersonalDataTableName = "personaldata-mock";
        public const string AuthDataTableName = "authdata-mock";
        public const string AuthDataIndexByIdTableName = "authdata-mock-index-by-id";
        
        public const string ClientProfileTableName = "client-profile";

    }
}
