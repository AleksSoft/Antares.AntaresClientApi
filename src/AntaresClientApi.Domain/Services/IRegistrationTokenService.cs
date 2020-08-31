using System.Threading.Tasks;
using AntaresClientApi.Domain.Models.MyNoSql;

namespace AntaresClientApi.Domain.Services
{
    public interface IRegistrationTokenService
    {
        Task<(RegistrationTokenEntity, string)> CreateAsync();

        Task<RegistrationTokenEntity> GetByOriginalTokenAsync(string tokenId);

        Task SaveAsync(RegistrationTokenEntity token);

        Task DeleteAsync(RegistrationTokenEntity token);
    }
}
