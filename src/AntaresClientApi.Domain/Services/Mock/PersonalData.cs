using System.Threading.Tasks;
using AntaresClientApi.Domain.Models;

namespace AntaresClientApi.Domain.Services.Mock
{
    public class PersonalDataMock: IPersonalData
    {
        public async Task<ClientIdentity> RegisterClientAsync(string requestEmail,
            string requestPhone,
            string requestFullName,
            string requestCountryIso3Code,
            string requestAffiliateCode)
        {
            return new ClientIdentity()
                {
                    ClientId = $"user-test-{requestEmail}",
                    TenantId = "demo"
                };
        }
    }
}
