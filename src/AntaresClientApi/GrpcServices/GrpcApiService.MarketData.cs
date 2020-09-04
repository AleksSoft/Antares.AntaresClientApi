using System.Linq;
using System.Threading.Tasks;
using AntaresClientApi.Domain.Services;
using Assets.Domain.MyNoSql;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Swisschain.Lykke.AntaresWalletApi.ApiContract;

namespace AntaresClientApi.GrpcServices
{
    public partial class GrpcApiService : ApiService.ApiServiceBase
    {
        public const string DefaultAssetCategoryId = "assets";
        public const string AssetIconUrl = "https://swcimages.blob.core.windows.net/images/Capture.PNG";

        public override async Task<AssetsDictionaryResponse> AssetsDictionary(Empty request, ServerCallContext context)
        {
            var session = SessionFromContext(context);

            var assets = await _marketDataService.GetAssetsByTenant(session.TenantId);
                
            
            var response = new AssetsDictionaryResponse();

            //todo: add all parameters in AssetService
            foreach (var asset in assets.Where(a => !a.IsDisabled))
            {
                response.Assets.Add(
                    new Asset(
                        id: asset.Symbol,
                        name: asset.Symbol,
                        symbol: asset.Symbol,
                        displayId:asset.Symbol,
                        accuracy: asset.Accuracy,
                        kycNeeded: true,
                        categoryId: DefaultAssetCategoryId,
                        cardDeposit: false,
                        swiftDeposit: false,
                        blockchainDeposit: false, 
                        swiftWithdrawal: false,
                        forwardWithdrawa: false,
                        crosschainWithdrawal: false,
                        isTrusted: true,
                        canBeBase: true,
                        iconUrl: AssetIconUrl
                    ));
            }

            //todo: Add category
            response.Categories.Add(
                new AssetCategory()
                {
                    Id = DefaultAssetCategoryId,
                    IconUrl = AssetIconUrl,
                    Name = "Trading Assets"
                });

            return response;
        }

        public override async Task<AssetPairsResponse> GetAssetPairs(Empty request, ServerCallContext context)
        {
            var session = SessionFromContext(context);

            var pairs = await _marketDataService.GetAssetPairsByTenant(session.TenantId);

            var response = new AssetPairsResponse();

            foreach (var pair in pairs)
            {
                response.AssetPairs.Add(new AssetPair()
                {
                    Id = pair.Symbol,
                    Accuracy = pair.Accuracy,
                    BaseAssetId = pair.BaseAssetId.ToString(),
                    Name = pair.Symbol,
                    InvertedAccuracy = pair.Accuracy, //todo: add invert accuracy
                    QuotingAssetId = pair.QuotingAssetId.ToString()
                });
            }

            return response;
        }

        public override Task<PricesResponse> GetPrices(PricesRequest request, ServerCallContext context)
        {
            return base.GetPrices(request, context);
        }
    }
}
