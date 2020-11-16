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
using MoreLinq;
using OrderBooks.MyNoSql.PriceData;
using Swisschain.Lykke.AntaresWalletApi.ApiContract;
using CandleType = AntaresClientApi.Database.CandleData.Models.CandleType;

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

            var fiatAssets = new List<string>{"CHF", "EUR", "USA"};
            var cryptoAssets = new List<string>{"BTC", "ETH", "ETHtest", "BTCtest"};
            //todo: add all parameters in AssetService
            foreach (var asset in assets.Where(a => !a.IsDisabled))
            {
                response.Assets.Add(
                    new Asset(
                        id: asset.Id.ToString(),
                        name: asset.Symbol,
                        symbol: asset.Symbol,
                        displayId:asset.Symbol,
                        accuracy: asset.Accuracy,
                        kycNeeded: true,
                        categoryId: DefaultAssetCategoryId,
                        cardDeposit: fiatAssets.Contains(asset.Symbol),
                        swiftDeposit: fiatAssets.Contains(asset.Symbol),
                        blockchainDeposit: cryptoAssets.Contains(asset.Symbol),
                        swiftWithdrawal: fiatAssets.Contains(asset.Symbol),
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

        public override async Task<CandlesResponse> GetCandles(CandlesRequest request, ServerCallContext context)
        {
            var symbol = request.AssetPairId;

            TimeSpan delta;

            const int defaultCountOfCandle = 1000;

            AntaresClientApi.Database.CandleData.Models.CandleType interval;

            switch (request.Interval)
            {
                //todo: implement other candle intervals;

                //case CandleInterval.Min5:   delta = TimeSpan.FromMinutes(5); break;
                //case CandleInterval.Min15:  delta = TimeSpan.FromMinutes(15); break;
                //case CandleInterval.Min30:  delta = TimeSpan.FromMinutes(30); break;
                case CandleInterval.Hour:   delta = TimeSpan.FromHours(1); interval = CandleType.Hour;  break;
                //case CandleInterval.Hour4:  delta = TimeSpan.FromHours(4); break;
                //case CandleInterval.Hour6:  delta = TimeSpan.FromHours(6); break;
                //case CandleInterval.Hour12: delta = TimeSpan.FromHours(12); break;
                case CandleInterval.Day:    delta = TimeSpan.FromDays(1); interval = CandleType.Day; break;
                //case CandleInterval.Week:   delta = TimeSpan.FromDays(7); break;
                case CandleInterval.Month:  delta = TimeSpan.FromDays(30); interval = CandleType.Month; break;
                default:
                {
                    var response = new CandlesResponse();
                    return response;
                }
            }

            var toDate = DateTime.UtcNow;
            var fromDate = toDate.AddSeconds(-delta.TotalSeconds * defaultCountOfCandle);

            if (request.From != null && request.To != null)
            {
                fromDate = request.From.ToDateTime();
                toDate = request.To.ToDateTime();
            }
            else if (request.From != null)
            {
                fromDate = request.From.ToDateTime();
                toDate = fromDate.AddSeconds(delta.Seconds * defaultCountOfCandle);
            }
            else if (request.To != null)
            {
                toDate = request.To.ToDateTime();
                fromDate = toDate.AddSeconds(-delta.Seconds * defaultCountOfCandle);
            }

            //todo: implement different charts for ask\bid\mig\trade
            //request.Type


            try
            {
                var candles = await _marketDataService.GetCandles(request.AssetPairId,
                    fromDate,
                    toDate,
                    interval);

                var result = new CandlesResponse()
                {
                    Candles =
                    {
                        candles.OrderBy(c => c.Time)
                            .Select(c => new Candle()
                            {
                                Timestamp =
                                    Timestamp.FromDateTime(DateTime.SpecifyKind(c.Time, DateTimeKind.Utc)),
                                Volume = "0",
                                OppositeVolume = "0",
                                Open = c.Open.ToString(CultureInfo.InvariantCulture),
                                Close = c.Close.ToString(CultureInfo.InvariantCulture),
                                High = c.High.ToString(CultureInfo.InvariantCulture),
                                Low = c.Close.ToString(CultureInfo.InvariantCulture),
                                LastPrice = c.Close.ToString(CultureInfo.InvariantCulture)
                            })
                    }
                };

                return result;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }


    }
}
