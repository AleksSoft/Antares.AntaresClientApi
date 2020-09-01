using System.Threading.Tasks;
using AntaresClientApi.Domain.Models;

namespace AntaresClientApi.Domain.Services
{
    public interface IClientAccountManager
    {
        Task<RegistrationResult> RegisterAccountAsync(
            string email,
            string phone,
            string fullName,
            string countryIso3Code,
            string affiliateCode,
            string password,
            string hint,
            string pin);
    }
}
