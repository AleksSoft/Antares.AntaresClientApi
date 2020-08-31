using System.Threading.Tasks;

namespace AntaresClientApi.Domain.Services
{
    public interface ISmsVerification
    {
        Task<string> SendVerificationSms(string clientId, string tenantId);
        Task<string> SendVerificationSms(string phone);
    }
}
