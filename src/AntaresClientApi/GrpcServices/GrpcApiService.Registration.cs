using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Swisschain.Lykke.AntaresWalletApi.ApiContract;

namespace AntaresClientApi.GrpcServices
{
    public partial class GrpcApiService
    {
        private static ConcurrentDictionary<string, string> _tokens = new ConcurrentDictionary<string, string>();

        private static ConcurrentDictionary<string, string> _sessions = new ConcurrentDictionary<string, string>();

        public override async Task<VerificationEmailResponse> SendVerificationEmail(VerificationEmailRequest request, ServerCallContext context)
        {
            var token = Guid.NewGuid().ToString("N");

            _tokens.TryAdd(token, request.Email);

            return new VerificationEmailResponse()
            {
                Result = new VerificationEmailResponse.Types.VerificationEmailPayload()
                {
                    Token = token
                }
            };
        }

        public override async Task<VerifyResponse> VerifyEmail(VerifyEmailRequest request, ServerCallContext context)
        {
            if (_tokens.TryGetValue(request.Token, out var email) &&
                email == request.Email &&
                request.Code == "0000")
            {
                return new VerifyResponse()
                {
                    Result = new VerifyResponse.Types.VerifyPayload()
                    {
                        Passed = true
                    }
                };
            }

            return new VerifyResponse()
            {
                Result = new VerifyResponse.Types.VerifyPayload()
                {
                    Passed = false
                }
            };


        }

        public override async Task<EmptyResponse> SendVerificationSms(VerificationSmsRequest request, ServerCallContext context)
        {
            if (_tokens.TryGetValue(request.Token, out _))
            {
                _tokens[request.Token] = request.Phone;
                return new EmptyResponse();
            }

            return new EmptyResponse()
            {
                Error = new ErrorV1()
                {
                    Code = "401",
                    Details = "401 Unauthorized",
                    Message = "401 Unauthorized"
                }
            };
        }

        public override async Task<VerifyResponse> VerifyPhone(VerifyPhoneRequest request, ServerCallContext context)
        {
            if (_tokens.TryGetValue(request.Token, out var phone) &&
                phone == request.Phone &&
                request.Code == "0000")
            {
                return new VerifyResponse()
                {
                    Result = new VerifyResponse.Types.VerifyPayload()
                    {
                        Passed = true
                    }
                };
            }

            return new VerifyResponse()
            {
                Result = new VerifyResponse.Types.VerifyPayload()
                {
                    Passed = false
                }
            };


        }

        public override async Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
        {
            if (!_tokens.TryGetValue(request.Token, out _))
            {
                return new RegisterResponse()
                {
                    Error = new ErrorV1()
                    {
                        Code = "401",
                        Details = "401 Unauthorized",
                        Message = "401 Unauthorized"
                    }
                };
            }

            var session = Guid.NewGuid().ToString("N");

            return new RegisterResponse()
            {
                Result = new RegisterResponse.Types.RegisterPayload()
                {
                    SessionId = session,
                    CanCashInViaBankCard = false,
                    NotificationsId = "111",
                    SwiftDepositEnabled = false,
                    State = "OK",
                    PersonalData = new PersonalData()
                    {
                        Phone = request.Phone,
                        Email = request.Email
                    }
                }
            };
        }
    }
}
