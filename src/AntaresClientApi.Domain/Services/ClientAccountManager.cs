using System.Threading.Tasks;
using AntaresClientApi.Domain.Models;
using AntaresClientApi.Domain.Models.MyNoSql;
using AntaresClientApi.Domain.Services.Extention;
using MyNoSqlServer.Abstractions;

namespace AntaresClientApi.Domain.Services
{
    public class ClientAccountManager : IClientAccountManager
    {
        private readonly IPersonalData _personalData;

        private readonly IAuthService _authService;

        private readonly IClientWalletService _clientWalletService;

        private readonly IMyNoSqlServerDataWriter<ClientProfileEntity> _clientProfileDataWriter;
        private readonly IMyNoSqlServerDataReader<ClientProfileEntity> _clientProfileDataReader;
        private readonly IMarketDataService _marketDataService;

        public ClientAccountManager(IPersonalData personalData, IAuthService authService, IClientWalletService clientWalletService,
            IMyNoSqlServerDataWriter<ClientProfileEntity> clientProfileDataWriter, IMarketDataService marketDataService,
            IMyNoSqlServerDataReader<ClientProfileEntity> clientProfileDataReader)
        {
            _personalData = personalData;
            _authService = authService;
            _clientWalletService = clientWalletService;
            _clientProfileDataWriter = clientProfileDataWriter;
            _marketDataService = marketDataService;
            _clientProfileDataReader = clientProfileDataReader;
        }

        public async Task<RegistrationResult> RegisterAccountAsync(
            string tenantId,
            string email,
            string phone,
            string fullName,
            string countryIso3Code,
            string affiliateCode,
            string password,
            string hint,
            string pin)
        {
            var identity = await _personalData.RegisterClientAsync(
                tenantId,
                email,
                phone,
                fullName,
                countryIso3Code,
                affiliateCode);

            if (identity == null)
            {
                return null;
            }

            await _clientWalletService.RegisterOrGetDefaultWallets(identity);

            await CreateClientProfile(tenantId, identity.ClientId);

            var result = await _authService.RegisterClientAsync(
                identity.TenantId,
                identity.ClientId,
                email,
                password,
                hint,
                pin);

            return result;
        }

        public async Task<ClientProfileEntity> GetClientProfile(string tenantId, long clientId)
        {
            var profile = _clientProfileDataReader.Get(ClientProfileEntity.GeneratePartitionKey(tenantId),
                ClientProfileEntity.GenerateRowKey(clientId));

            if (profile == null)
            {
                profile = await CreateClientProfile(tenantId, clientId);
            }

            return profile;
        }

        public async Task SetBaseAssetToClientProfile(string tenantId, long clientId, string baseAssetId)
        {
            var profile = await GetClientProfile(tenantId, clientId);

            profile.BaseAssetId = baseAssetId;

            await _clientProfileDataWriter.InsertOrReplaceAsync(profile);
        }

        private async Task<ClientProfileEntity> CreateClientProfile(string tenantId, long clientId)
        {
            var exist = await _clientProfileDataWriter.TryGetAsync(ClientProfileEntity.GeneratePartitionKey(tenantId), ClientProfileEntity.GenerateRowKey(clientId));
            if (exist != null)
            {
                return exist;
            }

            var baseAsset = await _marketDataService.GetDefaultBaseAsset(tenantId);

            var profile = new ClientProfileEntity()
            {
                PartitionKey = ClientProfileEntity.GeneratePartitionKey(tenantId),
                RowKey = ClientProfileEntity.GenerateRowKey(clientId),
                TenantId = tenantId,
                ClientId = clientId,
                BaseAssetId = baseAsset.Symbol
            };

            await _clientProfileDataWriter.InsertOrReplaceAsync(profile);

            return profile;
        }
    }
}
