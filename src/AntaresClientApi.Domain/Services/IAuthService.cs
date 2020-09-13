using System.Threading.Tasks;
using AntaresClientApi.Domain.Models;

namespace AntaresClientApi.Domain.Services
{
    public interface IAuthService
    {
        Task<ClientIdentity> Login(string tenantId, string username, string password);

        Task<bool> CheckPin(string tenantId, long clientId, string pinHash);

        Task<RegistrationResult> RegisterClientAsync(
            string tenantId,
            long clientId,
            string requestEmail,
            string requestPassword,
            string requestHint,
            string requestPin);

    }
}
