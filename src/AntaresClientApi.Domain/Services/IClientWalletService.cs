using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AntaresClientApi.Domain.Models;
using AntaresClientApi.Domain.Models.MyNoSql;
using AntaresClientApi.Domain.Models.Wallet;

namespace AntaresClientApi.Domain.Services
{
    public interface IClientWalletService
    {
        Task<ClientWalletEntity> RegisterOrGetDefaultWallets(ClientIdentity client);

        Task<IReadOnlyList<IAssetBalance>> GetClientBalances(string tenantId, long clientId);
        Task<IReadOnlyList<IClientOrder>> GetClientOrdersAsync(string sessionTenantId, long sessionClientId, string assetId);

        Task<IReadOnlyList<IClientTrade>> GetClientTradesAsync(
            string tenantId,
            long clientId,
            string assetPairId,
            DateTime? fromTime,
            DateTime? toTime,
            string side,
            int skip,
            int take);

        Task<long> GetWalletIdAsync(string tenantId, long clientId);
    }
}
