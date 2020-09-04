using System.Threading.Tasks;

namespace AntaresClientApi.Domain.Services
{
    public interface ISmsVerification
    {
        Task<string> SendVerificationSms(long clientId, string tenantId);
        Task<string> SendVerificationSms(string phone);
    }
}
