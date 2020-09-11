using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AntaresClientApi.Domain.Services;
using Flurl.Util;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
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

        public override Task<PendingActionsResponse> GetPendingActions(Empty request, ServerCallContext context)
        {
            var result = new PendingActionsResponse()
            {
                Result = new PendingActionsResponse.Types.PendingActionsPayload()
                {
                    DialogPending = false,
                    EthereumPendingActions = false,
                    NeedReinit = false,
                    OffchainRequests = false,
                    PendingOperations = false,
                    SessionConfirmation = false,
                    UnsignedTxs = false
                }
            };

            return Task.FromResult(result);
        }

        public override async Task<LimitOrdersResponse> GetOrders(LimitOrdersRequest request, ServerCallContext context)
        {
            var session = SessionFromContext(context);

            var assetId = request.OptionalAssetPairIdCase == LimitOrdersRequest.OptionalAssetPairIdOneofCase.AssetPairId
                ? request.AssetPairId
                : string.Empty;

            var orders = await _clientWalletService.GetClientOrdersAsync(session.TenantId, session.ClientId, assetId);
            
            var response = new LimitOrdersResponse()
            {
                Result = new LimitOrdersResponse.Types.OrdersPayload()
                {
                    Orders =
                    {
                        orders.Select(o => new LimitOrderModel()
                        {
                            Id = o.ExternalId,
                            DateTime = o.CreatedAt.ToString("O"),
                            Asset = string.Empty,
                            Price = o.Price,
                            Volume = o.Volume.Replace("-",""),
                            RemainingVolume = o.RemainingVolume,
                            AssetPair = o.AssetPairId,
                            RemainingOtherVolume = o.RemainingVolume,
                            TotalCost = (decimal.Parse(o.Price)*decimal.Parse(o.Volume)).ToString(CultureInfo.InvariantCulture),
                            OrderType = o.Side.ToString()
                        })
                    }
                }
            };

            return response;
        }

        public override async Task<TradesResponse> GetTrades(TradesRequest request, ServerCallContext context)
        {
            var session = SessionFromContext(context);

            var assetPairId = request.OptionalAssetPairIdCase == TradesRequest.OptionalAssetPairIdOneofCase.AssetPairId
                ? request.AssetPairId
                : string.Empty;

            DateTime? fromTime = request.OptionalFromDateCase == TradesRequest.OptionalFromDateOneofCase.From
                ? request.From.ToDateTime()
                : (DateTime?)null;

            DateTime? toTime = request.OptionalToDateCase == TradesRequest.OptionalToDateOneofCase.To
                ? request.To.ToDateTime()
                : (DateTime?)null;

            var side = request.OptionalTradeTypeCase == TradesRequest.OptionalTradeTypeOneofCase.TradeType
                ? request.TradeType
                : string.Empty;

            var trades = await _clientWalletService.GetClientTradesAsync(session.TenantId, session.ClientId, 
                assetPairId, fromTime, toTime, side, request.Skip, request.Take);

            var resp = new TradesResponse();
            foreach (var trade in trades)
            {
                resp.Trades.Add(new TradesResponse.Types.TradeModel()
                {
                    Id = trade.TradeId,
                    AssetPairId = trade.BaseAssetId+trade.QuotingAssetId, //todo: take Id from AssetPair list
                    Timestamp = Timestamp.FromDateTime(DateTime.SpecifyKind(trade.Timestamp, DateTimeKind.Utc)),
                    Price = trade.Price,
                    BaseVolume = trade.BaseVolume.Replace("-", ""),
                    BaseAssetName = trade.BaseAssetId,
                    QuoteVolume = trade.QuotingVolume,
                    QuoteAssetName = trade.QuotingAssetId,
                    Direction = trade.BaseVolume.StartsWith("-") ? "Sell" : "Buy",
                    OrderId = trade.ExternalOrderId
                });
            }

            return resp;
        }
    }
}
