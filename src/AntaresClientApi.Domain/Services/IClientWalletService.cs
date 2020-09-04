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
    }
}
