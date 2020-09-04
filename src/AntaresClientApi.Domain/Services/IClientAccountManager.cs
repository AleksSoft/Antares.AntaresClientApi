using System.Threading.Tasks;
using AntaresClientApi.Domain.Models;
using AntaresClientApi.Domain.Models.MyNoSql;

namespace AntaresClientApi.Domain.Services
{
    public interface IClientAccountManager
    {
        Task<RegistrationResult> RegisterAccountAsync(
            string tenantId,
            string email,
            string phone,
            string fullName,
            string countryIso3Code,
            string affiliateCode,
            string password,
            string hint,
            string pin);

        ClientProfileEntity GetClientProfile(string tenantId, long clientId);
        Task SetBaseAssetToClientProfile(string tenantId, long clientId, string baseAssetId);
    }
}
