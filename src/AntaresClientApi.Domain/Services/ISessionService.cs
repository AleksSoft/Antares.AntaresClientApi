using System.Threading.Tasks;
using AntaresClientApi.Domain.Models.MyNoSql;

namespace AntaresClientApi.Domain.Services
{
    public interface ISessionService
    {
        SessionEntity GetSession(string sessionId);
        SessionEntity GetSessionByOriginToken(string sessionId);
        Task<(SessionEntity, string)> CreateVerifiedSessionAsync(string tenantId, long clientId, string publicKey = null);
        Task<(SessionEntity, string)> CreateSessionAsync(string tenantId, long clientId, string publicKey = null);
        ValueTask SaveSessionAsync(SessionEntity session);
        ValueTask ProlongateAndSaveSessionAsync(SessionEntity session);
        Task CloseSession(SessionEntity sessionId, string reason);
    }
}
