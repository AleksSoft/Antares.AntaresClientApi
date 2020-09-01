using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntaresClientApi.Common.Configuration;
using AntaresClientApi.Domain.Models.MyNoSql;
using MyNoSqlServer.Abstractions;
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
            await GenerateWallet("demo", "user-test-test@swisschain.com", 100000000, TradingWalletType.Trading);

            await GenerateWallet("demo", "user-test-test-1@swisschain.com", 100000001, TradingWalletType.Trading);
            await GenerateWallet("demo", "user-test-test-2@swisschain.com", 100000002, TradingWalletType.Trading);
            await GenerateWallet("demo", "user-test-test-3@swisschain.com", 100000003, TradingWalletType.Trading);
            await GenerateWallet("demo", "user-test-test-4@swisschain.com", 100000004, TradingWalletType.Trading);


        }

        private async Task GenerateWallet(string tenantId,
            string clientId,
            long walletId,
            TradingWalletType type)
        {
            var wallets = await _walletWriter.GetOrDefaultAsync(ClientWalletEntity.GetPk(),
                    ClientWalletEntity.GetRk(tenantId, clientId));

            if (wallets == null)
            {
                wallets = ClientWalletEntity.Generate(tenantId, clientId);
            }

            var wallet = wallets.WalletList.FirstOrDefault(w => w.WalletId == walletId);

            if (wallet != null && wallet.Type != type)
            {
                throw new Exception($"Data not consistent. WalleId: {walletId}, ClientId: {clientId}, tenantId: {tenantId}, ExistingType: {wallet.Type}, newType: {type}");
            }

            if (wallet == null)
            {
                wallet = new TradingWallet()
                {
                    Client = wallets.Client,
                    Type = type,
                    WalletId = walletId
                };

                wallets.WalletList.Add(wallet);

                await _walletWriter.InsertOrReplaceAsync(wallets);
            }

            _testOutputHelper.WriteLine($"Wallet added. Tenant: {tenantId}, Client: {clientId}, Wallet: {walletId}, Type: {type}");
        }
    }
}
