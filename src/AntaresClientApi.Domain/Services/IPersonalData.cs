using System.Threading.Tasks;
using AntaresClientApi.Domain.Models;

namespace AntaresClientApi.Domain.Services
{
    public interface IPersonalData
    {
        Task<ClientIdentity> RegisterClientAsync(
            string tenantId,
            string requestEmail,
            string requestPhone,
            string requestFullName,
            string requestCountryIso3Code,
            string requestAffiliateCode);
    }
}
