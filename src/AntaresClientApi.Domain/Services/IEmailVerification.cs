using System.Threading.Tasks;

namespace AntaresClientApi.Domain.Services
{
    public interface IEmailVerification
    {
        Task<string> SendVerificationEmail(long clientId, string tenantId);
        Task<string> SendVerificationEmail(string email);
    }
}
