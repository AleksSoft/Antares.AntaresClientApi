using System.Threading.Tasks;
using AntaresClientApi.Domain.Models;

namespace AntaresClientApi.Domain.Services
{
    public class ClientAccountManager : IClientAccountManager
    {
        private readonly IPersonalData _personalData;

        private readonly IAuthService _authService;

        private readonly IClientWalletService _clientWalletService;

        public ClientAccountManager(IPersonalData personalData, IAuthService authService, IClientWalletService clientWalletService)
        {
            _personalData = personalData;
            _authService = authService;
            _clientWalletService = clientWalletService;
        }

        public async Task<RegistrationResult> RegisterAccountAsync(string email,
            string phone,
            string fullName,
            string countryIso3Code,
            string affiliateCode,
            string password,
            string hint,
            string pin)
        {
            var identity = await _personalData.RegisterClientAsync(
                email,
                phone,
                fullName,
                countryIso3Code,
                affiliateCode);

            if (identity == null)
            {
                return null;
            }

            await _clientWalletService.RegisterDefaultWallets(identity);

            var result = await _authService.RegisterClientAsync(
                identity.TenantId,
                identity.ClientId,
                email,
                password,
                hint,
                pin);

            return result;
        }
    }
}
