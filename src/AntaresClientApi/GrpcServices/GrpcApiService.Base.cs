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
        private readonly IPersonalData _personalData;
        private readonly IClientWalletService _clientWalletService;
        private readonly IClientAccountManager _accountManager;

        public GrpcApiService(
            ISessionService sessionService, 
            ISmsVerification smsVerification,
            ILogger<GrpcApiService> logger,
            IAuthService authService,
            IRegistrationTokenService registrationTokenService,
            IEmailVerification emailVerification,
            IPersonalData personalData,
            IClientWalletService clientWalletService,
            IClientAccountManager accountManager)
        {
            _sessionService = sessionService;
            _smsVerification = smsVerification;
            _logger = logger;
            _authService = authService;
            _registrationTokenService = registrationTokenService;
            _emailVerification = emailVerification;
            _personalData = personalData;
            _clientWalletService = clientWalletService;
            _accountManager = accountManager;
        }
    }
}
