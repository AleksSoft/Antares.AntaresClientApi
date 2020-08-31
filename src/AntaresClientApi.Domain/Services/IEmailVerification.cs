using System.Threading.Tasks;

namespace AntaresClientApi.Domain.Services
{
    public interface IEmailVerification
    {
        Task<string> SendVerificationEmail(string clientId, string tenantId);
        Task<string> SendVerificationEmail(string email);
    }
}
