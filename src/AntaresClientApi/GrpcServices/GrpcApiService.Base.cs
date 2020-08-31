using AntaresClientApi.Domain.Services;
using Microsoft.Extensions.Logging;
using Swisschain.Lykke.AntaresWalletApi.ApiContract;

namespace AntaresClientApi.GrpcServices
{
    public partial class GrpcApiService : ApiService.ApiServiceBase
    {
        private readonly ISessionService _sessionService;
        private readonly ISmsVerification _smsVerification;
        private readonly ILogger<GrpcApiService> _logger;
        private readonly IAuthService _authService;
        private readonly IRegistrationTokenService _registrationTokenService;
        private readonly IEmailVerification _emailVerification;

        public GrpcApiService(
            ISessionService sessionService, 
            ISmsVerification smsVerification,
            ILogger<GrpcApiService> logger,
            IAuthService authService,
            IRegistrationTokenService registrationTokenService,
            IEmailVerification emailVerification)
        {
            _sessionService = sessionService;
            _smsVerification = smsVerification;
            _logger = logger;
            _authService = authService;
            _registrationTokenService = registrationTokenService;
            _emailVerification = emailVerification;
        }
    }
}
