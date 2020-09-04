using System.Globalization;
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
        public override Task<BaseAssetResponse> GetBaseAsset(Empty request, ServerCallContext context)
        {
            var session = SessionFromContext(context);

            var profile = _accountManager.GetClientProfile(session.TenantId, session.ClientId);

            var result = new BaseAssetResponse()
            {
                BaseAsset = new BaseAssetResponse.Types.BaseAsset() { AssetId = profile.BaseAssetId }
            };

            return Task.FromResult(result);
        }

        public override async Task<EmptyResponseV2> SetBaseAsset(BaseAssetUpdateRequest request, ServerCallContext context)
        {
            var session = SessionFromContext(context);

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
                    Available = balance.Available().ToString(CultureInfo.InvariantCulture),
                    Reserved = balance.Reserve.ToString(CultureInfo.InvariantCulture),
                    Timestamp = balance.Timestamp
                });
            }

            return base.GetBalances(request, context);
        }
    }
}
