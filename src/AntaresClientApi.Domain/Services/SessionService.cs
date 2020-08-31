using System;
using System.Threading.Tasks;
using AntaresClientApi.Common.Configuration;
using AntaresClientApi.Domain.Models.MyNoSql;
using AntaresClientApi.Domain.Tools;
using Common;
using MyNoSqlServer.Abstractions;

namespace AntaresClientApi.Domain.Services
{
    public class SessionService: ISessionService, IRegistrationTokenService
    {
        private readonly IMyNoSqlServerDataWriter<RegistrationTokenEntity> _registrationTokenWriter;
        private readonly IMyNoSqlServerDataReader<SessionEntity> _sessionsReader;
        private readonly IMyNoSqlServerDataWriter<SessionEntity> _sessionsWriter;
        private readonly IMyNoSqlServerDataReader<RegistrationTokenEntity> _registrationTokenReader;
        private readonly SessionConfig _sessionConfig;

        public SessionService(
            IMyNoSqlServerDataReader<SessionEntity> sessionsReader,
            IMyNoSqlServerDataWriter<SessionEntity> sessionsWriter,
            IMyNoSqlServerDataReader<RegistrationTokenEntity> registrationTokenReader,
            IMyNoSqlServerDataWriter<RegistrationTokenEntity> registrationTokenWriter,
            SessionConfig sessionConfig
        )
        {
            _registrationTokenWriter = registrationTokenWriter;
            _sessionsReader = sessionsReader;
            _sessionsWriter = sessionsWriter;
            _registrationTokenReader = registrationTokenReader;
            _sessionConfig = sessionConfig;
        }

        public SessionEntity GetSession(string sessionId)
        {
            return _sessionsReader.Get(SessionEntity.GetPk(), sessionId);
        }

        public SessionEntity GetSessionByOriginToken(string sessionId)
        {
            return _sessionsReader.Get(SessionEntity.GetPk(), sessionId.ToSha256().ToBase64());
        }

        public async Task<(SessionEntity, string)> CreateVerifiedSessionAsync(string tenantId, string clientId, string publicKey = null)
        {
            var (session, token) = SessionEntity.Generate(_sessionConfig.ExpirationTimeInMins);
            session.Verified = true;
            session.Sms = true;
            session.Pin = true;
            session.PublicKey = publicKey;

            session.ClientId = clientId;
            session.TenantId = tenantId;

            await _sessionsWriter.InsertOrReplaceAsync(session);
            return (session, token);
        }

        public async Task<(SessionEntity, string)> CreateSessionAsync(string tenantId, string clientId, string publicKey = null)
        {
            var (session, token) = SessionEntity.Generate(_sessionConfig.ExpirationTimeInMins);
            session.PublicKey = publicKey;

            session.ClientId = clientId;
            session.TenantId = tenantId;

            await _sessionsWriter.InsertOrReplaceAsync(session);
            return (session, token);
        }

        public ValueTask SaveSessionAsync(SessionEntity session)
        {
            return _sessionsWriter.InsertOrReplaceAsync(session);
        }

        public ValueTask ProlongateAndSaveSessionAsync(SessionEntity session)
        {
            session.ExpirationDate = DateTime.UtcNow.AddMinutes(_sessionConfig.ExpirationTimeInMins);
            return _sessionsWriter.InsertOrReplaceAsync(session);
        }

        public async Task CloseSession(SessionEntity session, string reason)
        {
            await _sessionsWriter.DeleteAsync(session.PartitionKey, session.RowKey);
        }

        #region IRegistrationTokenService

        async Task<(RegistrationTokenEntity, string)> IRegistrationTokenService.CreateAsync()
        {
            var (session, token) = RegistrationTokenEntity.Generate(_sessionConfig.RegistrationExpirationTimeInMins);
            await _registrationTokenWriter.InsertOrReplaceAsync(session);
            return (session, token);
        }

        Task<RegistrationTokenEntity> IRegistrationTokenService.GetByOriginalTokenAsync(string tokenId)
        {
            var data = _registrationTokenReader.Get(RegistrationTokenEntity.GetPk(), tokenId.ToSha256().ToBase64());
            return Task.FromResult(data);
        }

        async Task IRegistrationTokenService.SaveAsync(RegistrationTokenEntity token)
        {
            await _registrationTokenWriter.InsertOrReplaceAsync(token);
        }

        async Task IRegistrationTokenService.DeleteAsync(RegistrationTokenEntity token)
        {
            await _registrationTokenWriter.DeleteAsync(token.PartitionKey, token.RowKey);
        }

        #endregion
    }
}
