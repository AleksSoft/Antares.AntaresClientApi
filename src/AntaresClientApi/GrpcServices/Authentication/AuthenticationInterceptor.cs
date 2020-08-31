using System;
using System.Threading.Tasks;
using AntaresClientApi.Domain.Models.MyNoSql;
using AntaresClientApi.Domain.Services;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AntaresClientApi.GrpcServices.Authentication
{
    public class AuthenticationInterceptor : Interceptor
    {
        private readonly ISessionService _sessionService;
        private readonly ILogger<AuthenticationInterceptor> _logger;

        public AuthenticationInterceptor(ISessionService sessionService, ILogger<AuthenticationInterceptor> logger)
        {
            _sessionService = sessionService;
            _logger = logger;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            if (IsAuthRequired(context))
            {

                var session = SessionFromContext(context);

                if (session == null)
                {
                    context.Status = new Status(StatusCode.Unauthenticated, "SessionNotFound");
                    return Activator.CreateInstance<TResponse>();
                }

                if (!session.Verified)
                {
                    context.Status = new Status(StatusCode.Unauthenticated, "SessionNotVerified");
                    return Activator.CreateInstance<TResponse>();
                }

                if (DateTime.UtcNow > session.ExpirationDate)
                {
                    context.Status = new Status(StatusCode.Unauthenticated, "SessionExpired");
                    return Activator.CreateInstance<TResponse>();
                }

                context.UserState.Add(UserStateProperties.ClientId, session.ClientId);
                context.UserState.Add(UserStateProperties.TenantId, session.TenantId);
                context.UserState.Add(UserStateProperties.Session, session);
                context.UserState.Add(UserStateProperties.SessionTokenHash, session.Id);
            }

            try
            {
                return await base.UnaryServerHandler(request, context, continuation);
            }
            catch (UnAuthorizedException ex)
            {
                _logger.LogInformation("Detect UnAuthorizedException from Method: {Method}, TokenHash: {TokenHash}, Message: {Message}", context.Method, context.GetSessionTokenHash(), ex.ToString());
                context.Status = new Status(StatusCode.Unauthenticated, "SessionNotFound");
                return Activator.CreateInstance<TResponse>();
            }
        }

        private bool IsAuthRequired(ServerCallContext context)
        {
            var endpoint = context.GetHttpContext().GetEndpoint();
            var anonymousAttribute = endpoint.Metadata.GetMetadata<AllowAnonymousAttribute>();

            return anonymousAttribute == null;
        }

        protected SessionEntity SessionFromContext(ServerCallContext context)
        {
            var sessionId = context.GetToken();
            if (string.IsNullOrEmpty(sessionId))
            {
                return null;
            }

            var session = _sessionService.GetSessionByOriginToken(sessionId);

            return session;
        }
    }
}
