using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AntaresClientApi.Domain.Models;
using AntaresClientApi.Domain.Models.Extensions;
using AntaresClientApi.Domain.Models.MyNoSql;
using AntaresClientApi.Domain.Services.Extention;
using Common;
using MyNoSqlServer.Abstractions;

namespace AntaresClientApi.Domain.Services.Mock
{
    public class AuthServiceMock: IAuthService
    {
        private readonly IMyNoSqlServerDataWriter<AuthDataEntity> _dataWriter;
        private readonly IMyNoSqlServerDataWriter<AuthDataIndexByIdEntity> _indexDataWriter;

        public AuthServiceMock(IMyNoSqlServerDataWriter<AuthDataEntity> dataWriter, 
            IMyNoSqlServerDataWriter<AuthDataIndexByIdEntity> indexDataWriter)
        {
            _dataWriter = dataWriter;
            _indexDataWriter = indexDataWriter;
        }

        public async Task<ClientIdentity> Login(string tenantId, string username, string password)
        {
            var data = await _dataWriter.TryGetAsync(AuthDataEntity.GeneratePartitionKey(), AuthDataEntity.GenerateRowKey(tenantId, username));

            if (data.PasswordHash == password.ToSha256().ToBase64())
            {
                var indexEntity = AuthDataIndexByIdEntity.Generate(data.TenantId, data.ClientId, data.Email);
                await _indexDataWriter.InsertOrReplaceAsync(indexEntity);

                return new ClientIdentity()
                {
                    ClientId = data.ClientId,
                    TenantId = data.TenantId
                };
            }

            return null;
        }

        public async Task<bool> CheckPin(string tenantId, long clientId, string pinHash)
        {
            var indexEntity = await _indexDataWriter.TryGetAsync(AuthDataIndexByIdEntity.GeneratePartitionKey(tenantId), AuthDataIndexByIdEntity.GenerateRowKey(clientId));
            if (indexEntity == null)
                return false;

            var entity = await _dataWriter.TryGetAsync(AuthDataEntity.GeneratePartitionKey(), AuthDataEntity.GenerateRowKey(indexEntity.TenantId, indexEntity.Email));
            if (entity == null || entity.PinHash != pinHash)
            {
                return false;
            }

            return true;
        }

        public async Task<RegistrationResult> RegisterClientAsync(
            string tenantId,
            long clientId,
            string requestEmail,
            string requestPassword,
            string requestHint,
            string requestPin)
        {
            var entity = AuthDataEntity.Generate(tenantId, requestEmail);
            entity.ClientId = clientId;
            entity.Hint = requestHint;
            entity.Email = requestEmail;
            entity.PasswordHash = requestPassword.ToSha256().ToBase64();
            entity.PinHash = requestPin.ToSha256().ToBase64();
            entity.TenantId = tenantId;

            var insertSuccess = await _dataWriter.TryInsertAsync(entity);
            if (!insertSuccess)
            {
                var exist = await TryGetAuthData(entity.TenantId, entity.Email);
                if (exist != null)
                {
                    return new RegistrationResult()
                    {
                        IsEmailAlreadyExist = true,
                        IsSuccess = false
                    };
                }
            }

            var indexEntity = AuthDataIndexByIdEntity.Generate(tenantId, clientId, requestEmail);
            await _indexDataWriter.InsertOrReplaceAsync(indexEntity);

            return new RegistrationResult()
            {
                IsEmailAlreadyExist = false,
                ClientIdentity = new ClientIdentity()
                {
                    ClientId = entity.ClientId,
                    TenantId = entity.TenantId
                },
                IsSuccess = true
            };
        }

        private async Task<AuthDataEntity> TryGetAuthData(string tenantId, string email)
        {
            return await _dataWriter.TryGetAsync(AuthDataEntity.GeneratePartitionKey(), AuthDataEntity.GenerateRowKey(tenantId, email));
        }
    }
}
