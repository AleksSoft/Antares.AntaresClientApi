using System.Threading.Tasks;
using AntaresClientApi.Domain.Tools;
using Common;

namespace AntaresClientApi.Domain.Services.Mock
{
    public class SmsVerificationMock: ISmsVerification
    {
        public Task<string> SendVerificationSms(long clientId, string tenantId)
        {
            return Task.FromResult("0000".ToSha256().ToBase64());
        }

        public Task<string> SendVerificationSms(string phone)
        {
            return Task.FromResult("0000".ToSha256().ToBase64());
        }
    }

    public class EmailVerificationMock: IEmailVerification
    {
        public Task<string> SendVerificationEmail(long clientId, string tenantId)
        {
            return Task.FromResult("0000".ToSha256().ToBase64());
        }

        public Task<string> SendVerificationEmail(string email)
        {
            return Task.FromResult("0000".ToSha256().ToBase64());
        }
    }
}
