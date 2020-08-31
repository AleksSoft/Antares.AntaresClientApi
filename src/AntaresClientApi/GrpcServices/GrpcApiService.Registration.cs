using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using AntaresClientApi.Domain.Tools;
using Common;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Swisschain.Lykke.AntaresWalletApi.ApiContract;

namespace AntaresClientApi.GrpcServices
{
    public partial class GrpcApiService
    {
        [AllowAnonymous]
        public override async Task<VerificationEmailResponse> SendVerificationEmail(VerificationEmailRequest request, ServerCallContext context)
        {
            if (!request.Email.IsValidEmail())
            {
                return new VerificationEmailResponse()
                {
                    Error = new ErrorV1()
                    {
                        Code = ErrorModelCode.InvalidInputField.ToString(),
                        Message = ErrorMessages.InvalidFieldValue("EMail")
                    }
                };
            }
            
            var (registration, token) = await _registrationTokenService.CreateAsync();

            var codeHash = await _emailVerification.SendVerificationEmail(request.Email);

            if (string.IsNullOrEmpty(codeHash))
            {
                return new VerificationEmailResponse()
                {
                    Error = new ErrorV1()
                    {
                        Code = ErrorModelCode.InvalidInputField.ToString(),
                        Message = ErrorMessages.CannotDeliveryEmail
                    }
                };
            }

            registration.EmailHash = request.Email.ToSha256().ToBase64();
            registration.LastCodeHash = codeHash;

            await _registrationTokenService.SaveAsync(registration);

            return new VerificationEmailResponse()
            {
                Result = new VerificationEmailResponse.Types.VerificationEmailPayload()
                {
                    Token = token
                }
            };
        }

        [AllowAnonymous]
        public override async Task<VerifyResponse> VerifyEmail(VerifyEmailRequest request, ServerCallContext context)
        {
            var token = await _registrationTokenService.GetByOriginalTokenAsync(request.Token);

            if (token == null || string.IsNullOrEmpty(request.Code)
                              || token.ExpirationDate >= DateTime.UtcNow
                              || token.EmailHash != request.Email.ToSha256().ToBase64()
                              || token.LastCodeHash != request.Code.ToSha256().ToBase64())
            {
                return new VerifyResponse()
                {
                    Result = new VerifyResponse.Types.VerifyPayload()
                    {
                        Passed = false
                    }
                };
            }

            token.EmailVerified = true;
            await _registrationTokenService.SaveAsync(token);

            return new VerifyResponse()
            {
                Result = new VerifyResponse.Types.VerifyPayload()
                {
                    Passed = true
                }
            };
        }

        [AllowAnonymous]
        public override async Task<EmptyResponse> SendVerificationSms(VerificationSmsRequest request, ServerCallContext context)
        {
            var token = await _registrationTokenService.GetByOriginalTokenAsync(request.Token);

            if (token == null || token.ExpirationDate >= DateTime.UtcNow
                              || !token.EmailVerified)
            {
                context.Status = new Status(StatusCode.Unauthenticated, "Unauthorized");
                return new EmptyResponse()
                {
                    Error = new ErrorV1()
                    {
                        Code = ErrorModelCode.NotAuthenticated.ToString(),
                        Message = ErrorMessages.Unauthorized
                    }
                };
            }

            if (string.IsNullOrEmpty(request.Phone))
            {
                return new EmptyResponse()
                {
                    Error = new ErrorV1()
                    {
                        Code = ErrorModelCode.InvalidInputField.ToString(),
                        Message = ErrorMessages.InvalidFieldValue("Phone")
                    }
                };
            }

            var codeHash = await _smsVerification.SendVerificationSms(request.Phone);
            
            if (string.IsNullOrEmpty(codeHash))
            {
                return new EmptyResponse()
                {
                    Error = new ErrorV1()
                    {
                        Code = ErrorModelCode.InvalidInputField.ToString(),
                        Message = ErrorMessages.CannotDeliverySms
                    }
                };
            }

            token.LastCodeHash = codeHash;
            token.PhoneHash = request.Phone.ToSha256().ToBase64();
            await _registrationTokenService.SaveAsync(token);

            return new EmptyResponse();
        }

        [AllowAnonymous]
        public override async Task<VerifyResponse> VerifyPhone(VerifyPhoneRequest request, ServerCallContext context)
        {
            var token = await _registrationTokenService.GetByOriginalTokenAsync(request.Token);

            if (token == null || string.IsNullOrEmpty(request.Code)
                              || token.ExpirationDate >= DateTime.UtcNow
                              || !token.EmailVerified
                              || token.PhoneHash != request.Phone.ToSha256().ToBase64()
                              || token.LastCodeHash != request.Code.ToSha256().ToBase64())
            {
                return new VerifyResponse()
                {
                    Result = new VerifyResponse.Types.VerifyPayload() { Passed = false }
                };
            }

            token.PhoneVerified = true;
            await _registrationTokenService.SaveAsync(token);

            return new VerifyResponse()
            {
                Result = new VerifyResponse.Types.VerifyPayload()
                {
                    Passed = true
                }
            };
        }

        [AllowAnonymous]
        public override async Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
        {
            var token = await _registrationTokenService.GetByOriginalTokenAsync(request.Token);

            if (token == null || token.ExpirationDate >= DateTime.UtcNow
                              || !token.EmailVerified
                              || !token.PhoneVerified
                              || token.EmailHash != request.Email.ToSha256().ToBase64()
                              || token.PhoneHash != request.Phone.ToSha256().ToBase64())
            {
                context.Status = new Status(StatusCode.Unauthenticated, "Unauthorized");
                return new RegisterResponse()
                {
                    Error = new ErrorV1()
                    {
                        Code = ErrorModelCode.NotAuthenticated.ToString(),
                        Message = ErrorMessages.Unauthorized
                    }
                };
            }

            if (!ValidatePublicKey(request.PublicKey))
            {
                return new RegisterResponse()
                {
                    Error = new ErrorV1()
                    {
                        Code = ErrorModelCode.InvalidInputField.ToString(),
                        Message = ErrorMessages.InvalidFieldValue(nameof(request.PublicKey))
                    }
                };
            }

            if (!token.RegistrationDone)
            {
                var registrationResult = await _authService.RegisterClientAsync(
                    request.Email,
                    request.Phone,
                    request.FullName,
                    request.CountryIso3Code,
                    request.AffiliateCode,
                    request.Password,
                    request.Hint,
                    request.Pin);

                if (registrationResult.IsEmailAlreadyExist)
                {
                    return new RegisterResponse()
                    {
                        Error = new ErrorV1()
                        {
                            Code = ErrorModelCode.ClientAlreadyExist.ToString(),
                            Message = ErrorMessages.ClientAlreadyExist
                        }
                    };
                }

                token.LastCodeHash = string.Empty;
                token.ClientId = registrationResult.ClientIdentity.ClientId;
                token.TenantId = registrationResult.ClientIdentity.TenantId;
                token.RegistrationDone = true;
                await _registrationTokenService.SaveAsync(token);
            }

            var (_, sessionToken) = await _sessionService.CreateVerifiedSessionAsync(token.TenantId,
                token.ClientId,
                request.PublicKey);
            

            return new RegisterResponse()
            {
                Result = new RegisterResponse.Types.RegisterPayload()
                {
                    SessionId = sessionToken,
                    CanCashInViaBankCard = false,
                    NotificationsId = string.Empty, //todo: set notification id
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
