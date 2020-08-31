using System;
using System.Threading.Tasks;
using AntaresClientApi.Domain.Models.MyNoSql;
using AntaresClientApi.Domain.Services;
using AntaresClientApi.GrpcServices.Authentication;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Swisschain.Lykke.AntaresWalletApi.ApiContract;

namespace AntaresClientApi.GrpcServices
{
    public partial class GrpcApiService 
    {
        [AllowAnonymous]
        public override Task<CheckSessionResponse> IsSessionExpired(CheckSessionRequest request, ServerCallContext context)
        {
            var session = _sessionService.GetSessionByOriginToken(request.SessionId);

            if (session == null)
            {
                context.Status = new Status(StatusCode.Unauthenticated, "Session not found");
                return Task.FromResult(new CheckSessionResponse()
                {
                    Expired = true
                });
            }

            return Task.FromResult(new CheckSessionResponse()
            {
                Expired = session.ExpirationDate <= DateTime.UtcNow
            });
        }

        public override async Task<EmptyResponse> ProlongateSession(Empty request, ServerCallContext context)
        {
            var session = SessionFromContext(context);

            await _sessionService.ProlongateAndSaveSessionAsync(session);

            return new EmptyResponse();
        }

        protected SessionEntity SessionFromContext(ServerCallContext context)
        {
            var session = context.GetSession();

            if (session == null)
            {
                throw new UnAuthorizedException($"Cannot find session by token. Method: {context.Method}");
            }

            return session;
        }
    }
}
