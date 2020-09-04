using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AntaresClientApi.Domain.Services;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Swisschain.Lykke.AntaresWalletApi.ApiContract;

namespace AntaresClientApi.GrpcServices
{
    public partial class GrpcApiService : ApiService.ApiServiceBase
    {
        public override async Task<BaseAssetResponse> GetBaseAsset(Empty request, ServerCallContext context)
        {
            var session = SessionFromContext(context);

            var profile = await _accountManager.GetClientProfile(session.TenantId, session.ClientId);

            var result = new BaseAssetResponse()
            {
                BaseAsset = new BaseAssetResponse.Types.BaseAsset() { AssetId = profile.BaseAssetId }
            };

            return result;
        }

        public override async Task<EmptyResponseV2> SetBaseAsset(BaseAssetUpdateRequest request, ServerCallContext context)
        {
            var session = SessionFromContext(context);

            var assets = await _marketDataService.GetAssetsByTenant(session.TenantId);

            if (assets.All(a => a.Symbol != request.BaseAssetId || a.IsDisabled))
            {
                return new EmptyResponseV2()
                {
                    Error = new ErrorV2()
                    {
                        Error = ErrorModelCode.AssetNotFound.ToString(),
                        Message = ErrorMessages.AssetNotFound
                    }
                };
            }

            await _accountManager.SetBaseAssetToClientProfile(session.TenantId, session.ClientId, request.BaseAssetId);

            return new EmptyResponseV2();
        }

        public override async Task<BalancesResponse> GetBalances(Empty request, ServerCallContext context)
        {
            var session = SessionFromContext(context);

            var balances = await _clientWalletService.GetClientBalances(session.TenantId, session.ClientId);

            var response = new BalancesResponse();
            foreach (var balance in balances)
            {
                response.Payload.Add(new Balance()
                {
                    AssetId = balance.AssetId,
                    Available = balance.Available.ToString(CultureInfo.InvariantCulture),
                    Reserved = balance.Reserve.ToString(CultureInfo.InvariantCulture),
                    Timestamp = Timestamp.FromDateTime(DateTime.SpecifyKind(balance.Timestamp, DateTimeKind.Utc))
                });
            }

            return response;
        }
    }
}
