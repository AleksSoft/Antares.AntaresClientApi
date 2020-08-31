using System.Threading.Tasks;
using AntaresClientApi.Domain.Models;

namespace AntaresClientApi.Domain.Services
{
    public interface IAuthService
    {
        Task<ClientIdentity> Login(string username, string password);

        Task<bool> CheckPin(string tenantId, string clientId, string pinHash);

        Task<RegistrationResult> RegisterClientAsync(
            string tenantId,
            string clientId,
            string requestEmail,
            string requestPassword,
            string requestHint,
            string requestPin);
    }
}
