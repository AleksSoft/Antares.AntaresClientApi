using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntaresClientApi.Common.Configuration;
using AntaresClientApi.Domain.Models.MyNoSql;
using AntaresClientApi.Domain.Services.Extention;
using MyNoSqlServer.Abstractions;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace AntaresClientApiTests
{
    public class GenerateMockData
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private IMyNoSqlServerDataWriter<ClientWalletEntity> _walletWriter;
        private AppConfig _config;

        public GenerateMockData(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _config = TestConfig.Load();

            if (string.IsNullOrEmpty(_config?.MyNoSqlServer?.WriterServiceUrl))
                throw new Exception("MyNoSqlServer.WriterServiceUrl cannot be empty");

            _walletWriter = new MyNoSqlServer.DataWriter.MyNoSqlServerDataWriter<ClientWalletEntity>(
                () => _config.MyNoSqlServer.WriterServiceUrl,
                MyNoSqlServerTables.ClientWalletTableName);

        }

        [Fact, Trait("Category", "Integration")]
        public async Task GeneateWallets()
        {


        }

        [Fact, Trait("Category", "Integration")]
        public async Task TestMethodsOfWriter()
        {
        }
    }
}
