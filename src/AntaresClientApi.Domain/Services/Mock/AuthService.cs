using System.Collections.Generic;
using System.Threading.Tasks;
using AntaresClientApi.Domain.Models;
using AntaresClientApi.Domain.Tools;

namespace AntaresClientApi.Domain.Services.Mock
{
    public class AuthServiceMock: IAuthService
    {
        public Dictionary<string, string> Users { get; set; } = new Dictionary<string, string> { {"test@swisschain.com", "123456"} };

        public async Task<ClientIdentity> Login(string username, string password)
        {
            if (!Users.TryGetValue(username, out var passw) || password != passw)
            {
                return null;
            }

            return new ClientIdentity()
            {
                ClientId = $"user-test-{username}",
                TenantId = "demo"
            };
        }

        public async Task<bool> CheckPin(string tenantId, string clientId, string pinHash)
        {
            return pinHash == "1111".ToSha256();
        }

        public async Task<RegistrationResult> RegisterClientAsync(string requestEmail,
            string requestPhone,
            string requestFullName,
            string requestCountryIso3Code,
            string requestAffiliateCode,
            string requestPassword,
            string requestHint,
            string requestPin)
        {
            if (Users.ContainsKey(requestEmail))
            {
                return new RegistrationResult()
                {
                    IsEmailAlreadyExist = true,
                    IsSuccess = false
                };
            }

            Users.Add(requestEmail, requestPassword);

            return new RegistrationResult()
            {
                IsEmailAlreadyExist = false,
                ClientIdentity = new ClientIdentity()
                {
                    ClientId = $"user-test-{requestEmail}",
                    TenantId = "demo"
                },
                IsSuccess = true
            };
        }
    }
}
