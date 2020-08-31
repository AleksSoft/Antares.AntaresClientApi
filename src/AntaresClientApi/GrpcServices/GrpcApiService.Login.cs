using System;
using System.Threading.Tasks;
using AntaresClientApi.Domain.Tools;
using Common;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Swisschain.Lykke.AntaresWalletApi.ApiContract;

namespace AntaresClientApi.GrpcServices
{
    public partial class GrpcApiService 
    {
        [AllowAnonymous]
        public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
        {
            var validateResult = ValidateLoginRequest(request);

            if (validateResult != null)
                return validateResult;

            var result = new LoginResponse();

            var identity = await _authService.Login(request.Email, request.Password);

            if (identity == null)
            {
                result.Error = new ErrorV1
                {
                    Code = ErrorModelCode.InvalidUsernameOrPassword.ToString(),
                    Message = ErrorMessages.InvalidLoginOrPassword
                };

                return result;
            }

            var (_, token) = await _sessionService.CreateSessionAsync(identity.TenantId, identity.ClientId, request.PublicKey);


            result.Result = new LoginResponse.Types.LoginPayload
            {
                SessionId = token,
                NotificationId = string.Empty //todo: setup right notification ID
            };

            return result;
        }

        [AllowAnonymous]
        public override async Task<EmptyResponse> SendLoginSms(LoginSmsRequest request, ServerCallContext context)
        {
            var session = _sessionService.GetSession(request.SessionId.ToSha256().ToBase64());

            if (session == null)
            {
                var result = new EmptyResponse
                {
                    Error = new ErrorV1
                    {
                        Code = ErrorModelCode.InvalidInputField.ToString(),
                        Message = ErrorMessages.InvalidFieldValue(nameof(request.SessionId)),
                        Field = nameof(request.SessionId)
                    }
                };

                return result;
            }

            var codeHash = await _smsVerification.SendVerificationSms(session.Id, session.ClientId);
            session.LastCodeHash = codeHash;
            await _sessionService.SaveSessionAsync(session);

            return new EmptyResponse();
        }

        [AllowAnonymous]
        public override async Task<VerifyLoginSmsResponse> VerifyLoginSms(VerifyLoginSmsRequest request, ServerCallContext context)
        {
            var session = _sessionService.GetSession(request.SessionId.ToSha256().ToBase64());

            if (session == null)
            {
                context.Status = new Status(StatusCode.Unauthenticated, "Session not found");

                return new VerifyLoginSmsResponse
                {
                    Error = new ErrorV1
                    {
                        Code = ErrorModelCode.InvalidInputField.ToString(),
                        Message = ErrorMessages.InvalidFieldValue(nameof(request.SessionId)),
                        Field = nameof(request.SessionId)
                    }
                };
            }

            if (session.SmsVerificationLimit <= 0)
            {
                context.Status = new Status(StatusCode.Unauthenticated, "Session is closed");
                await _sessionService.CloseSession(session, "sms verification limit");

                return new VerifyLoginSmsResponse
                {
                    Error = new ErrorV1
                    {
                        Code = ErrorModelCode.InvalidInputField.ToString(),
                        Message = ErrorMessages.InvalidFieldValue(nameof(request.SessionId)),
                        Field = nameof(request.SessionId)
                    }
                };
            }

            var verificationResult = session.LastCodeHash == request.Code.ToSha256().ToBase64();

            var result = new VerifyLoginSmsResponse();

            if (!verificationResult)
            {
                session.SmsVerificationLimit--;
                await _sessionService.SaveSessionAsync(session);

                if (session.SmsVerificationLimit <= 0)
                {
                    await _sessionService.CloseSession(session, "sms verification limit");
                    context.Status = new Status(StatusCode.Unauthenticated, "Session is closed");
                }

                result.Result = new VerifyLoginSmsResponse.Types.VerifyLoginSmsPayload { Passed = false };
                return result;
            }

            result.Result = new VerifyLoginSmsResponse.Types.VerifyLoginSmsPayload { Passed = true };
            session.Sms = true;
            session.ResetSmsVerificationLimit();

            if (session.Pin)
                session.Verified = true;

            await _sessionService.SaveSessionAsync(session);

            return result;
        }

        [AllowAnonymous]
        public override async Task<CheckPinResponse> CheckPin(CheckPinRequest request, ServerCallContext context)
        {
            var session = _sessionService.GetSession(request.SessionId.ToSha256().ToBase64());

            if (session == null)
            {
                context.Status = new Status(StatusCode.Unauthenticated, "Session not found");

                return new CheckPinResponse()
                {
                    Error = new ErrorV1
                    {
                        Code = ErrorModelCode.InvalidInputField.ToString(),
                        Message = ErrorMessages.InvalidFieldValue(nameof(request.SessionId)),
                        Field = nameof(request.SessionId)
                    }
                };
            }

            if (session.PinVerificationLimit <= 0)
            {
                context.Status = new Status(StatusCode.Unauthenticated, "Session is closed");
                await _sessionService.CloseSession(session, "sms verification limit");

                return new CheckPinResponse
                {
                    Error = new ErrorV1
                    {
                        Code = ErrorModelCode.InvalidInputField.ToString(),
                        Message = ErrorMessages.InvalidFieldValue(nameof(request.SessionId)),
                        Field = nameof(request.SessionId)
                    }
                };
            }

            var verificationResult = await _authService.CheckPin(session.TenantId, session.ClientId, request.Pin.ToSha256());

            var result = new CheckPinResponse();

            if (!verificationResult)
            {
                session.PinVerificationLimit--;
                await _sessionService.SaveSessionAsync(session);

                if (session.PinVerificationLimit <= 0)
                {
                    await _sessionService.CloseSession(session, "pin verification limit");
                    context.Status = new Status(StatusCode.Unauthenticated, "Session is closed");
                }

                result.Result = new CheckPinResponse.Types.CheckPinPayload() { Passed = false };
                return result;
            }

            result.Result = new CheckPinResponse.Types.CheckPinPayload() { Passed = true };
            session.Pin = true;
            session.ResetPinVerificationLimit();

            if (session.Sms)
                session.Verified = true;

            await _sessionService.ProlongateAndSaveSessionAsync(session);

            return result;
        }

        private LoginResponse ValidateLoginRequest(LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
                return new LoginResponse
                {
                    Error = new ErrorV1
                    {
                        Code = ErrorModelCode.InvalidInputField.ToString(),
                        Message = ErrorMessages.CantBeEmpty(nameof(request.Email)),
                        Field = nameof(request.Email)
                    }
                };

            if (!request.Email.IsValidEmailAndRowKey())
                return new LoginResponse
                {
                    Error = new ErrorV1
                    {
                        Code = ErrorModelCode.InvalidInputField.ToString(),
                        Message = ErrorMessages.InvalidFieldValue(nameof(request.Email)),
                        Field = nameof(request.Email)
                    }
                };

            if (string.IsNullOrEmpty(request.Password))
                return new LoginResponse
                {
                    Error = new ErrorV1
                    {
                        Code = ErrorModelCode.InvalidInputField.ToString(),
                        Message = ErrorMessages.CantBeEmpty(nameof(request.Password)),
                        Field = nameof(request.Password)
                    }
                };

            if (!ValidatePublicKey(request.PublicKey))
                return new LoginResponse
                {
                    Error = new ErrorV1
                    {
                        Code = ErrorModelCode.InvalidInputField.ToString(),
                        Message = ErrorMessages.InvalidFieldValue(nameof(request.PublicKey)),
                        Field = nameof(request.PublicKey)
                    }
                };
            
            return null;
        }

        private bool ValidatePublicKey(string publicKey)
        {
            //todo: add validation for Key and enable empty string
            return true; 
        }
    }
}
