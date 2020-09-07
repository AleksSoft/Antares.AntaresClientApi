using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AntaresClientApi.Domain.Services;
using Assets.Domain.MyNoSql;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using OrderBooks.MyNoSql.PriceData;
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
            var symbols = request.AssetPairIds.ToList();

            var prices = _marketDataService.GetPrices(DefaultTenantId);

            IEnumerable<PriceEntity> data = prices;

            if (symbols.Any())
            {
                data = data.Where(p => symbols.Contains(p.Symbol));
            }

            var response = new PricesResponse()
            {
                Prices =
                {
                    data.Select(p =>

                            new PriceUpdate()
                            {
                                AssetPairId = p.Symbol,
                                Timestamp = Timestamp.FromDateTime(DateTime.SpecifyKind(p.LastUpdate, DateTimeKind.Utc)),
                                Ask = p.Ask.HasValue ? p.Ask.ToString() : string.Empty,
                                Bid = p.Ask.HasValue ? p.Bid.ToString() : string.Empty,
                                PriceChange24H = "0", //todo: calculate 24h statistic and write to mynisql to use here
                                VolumeBase24H = "0",
                                VolumeQuote24H = "0"
                            })
                        .ToList()
                }
            };

            return Task.FromResult(response);
        }

        public override async Task<Orderbook> GetOrderbook(OrderbookRequest request, ServerCallContext context)
        {
            var book = _marketDataService.OrderBook(DefaultTenantId, request.AssetPairId);

            if (book == null || book.OrderBook == null)
            {
                return new Orderbook()
                {
                    AssetPairId = request.AssetPairId,
                    Timestamp = Timestamp.FromDateTime(DateTime.UtcNow)
                };
            }

            var response = new Orderbook()
            {
                AssetPairId = book.OrderBook.Symbol,
                Timestamp = Timestamp.FromDateTime(DateTime.SpecifyKind(book.OrderBook.Timestamp, DateTimeKind.Utc)),
                Asks =
                {
                    book.OrderBook.LimitOrders.Where(o => o.Volume < 0).OrderBy(o => o.Price).Select(o => new Orderbook.Types.PriceVolume()
                    {
                        P = o.Price.ToString(CultureInfo.InvariantCulture),
                        V = Math.Abs(o.Volume).ToString(CultureInfo.InvariantCulture)
                    })
                },
                Bids =
                {
                    book.OrderBook.LimitOrders.Where(o => o.Volume > 0).OrderByDescending(o => o.Price).Select(o => new Orderbook.Types.PriceVolume()
                    {
                        P = o.Price.ToString(CultureInfo.InvariantCulture),
                        V = Math.Abs(o.Volume).ToString(CultureInfo.InvariantCulture)
                    })
                }
            };

            return response;
        }
    }
}
