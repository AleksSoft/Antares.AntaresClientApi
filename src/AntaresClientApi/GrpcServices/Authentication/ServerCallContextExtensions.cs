using System.Linq;
using System.Security.Cryptography;
using AntaresClientApi.Domain.Models.MyNoSql;
using AntaresClientApi.Domain.Tools;
using Common;
using Grpc.Core;

namespace AntaresClientApi.GrpcServices.Authentication
{
    public static class ServerCallContextExtensions
    {
        public static string GetToken(this ServerCallContext context)
        {
            var header = GetAuthorizationHeader(context);

            if (string.IsNullOrEmpty(header))
                return null;

            var values = header.Split(' ');

            if (values.Length != 2)
                return null;

            if (values[0] != "Bearer")
                return null;

            return values[1];
        }

        public static string GetAuthorizationHeader(this ServerCallContext context)
        {
            var header = context.RequestHeaders.FirstOrDefault(x => x.Key.ToLowerInvariant() == "authorization")?.Value;
            return header;
        }

        public static SessionEntity GetSession(this ServerCallContext context)
        {
            if (context.UserState.TryGetValue(UserStateProperties.Session, out var session))
            {
                return context.UserState[UserStateProperties.Session] as SessionEntity;
            }
            return null;
        }

        public static string GetClientId(this ServerCallContext context)
        {
            return context.UserState[UserStateProperties.ClientId]?.ToString();
        }

        public static string GetTenantId(this ServerCallContext context)
        {
            return context.UserState[UserStateProperties.TenantId]?.ToString();
        }

        public static string GetSessionTokenHash(this ServerCallContext context)
        {
            return context.UserState[UserStateProperties.SessionTokenHash]?.ToString();
        }
    }
}
