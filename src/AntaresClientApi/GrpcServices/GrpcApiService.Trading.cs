using System;
using System.Globalization;
using System.Threading.Tasks;
using AntaresClientApi.Domain.Models.Wallet;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MatchingEngine.Client.Contracts.Incoming;
using Swisschain.Lykke.AntaresWalletApi.ApiContract;
using Status = MatchingEngine.Client.Contracts.Incoming.Status;

namespace AntaresClientApi.GrpcServices
{
    public partial class GrpcApiService
    {
        public override async Task<PlaceOrderResponse> PlaceLimitOrder(LimitOrderRequest request, ServerCallContext context)
        {
            var session = SessionFromContext(context);

            var assetPair = await _marketDataService.GetAssetPairByTenantAndId(session.TenantId, request.AssetPairId);

            if (assetPair == null || assetPair.IsDisabled)
            {
                return PlaceOrderErrorResponse(ErrorMessages.AssetPairNotFound, nameof(request.AssetPairId));
            }

            var absVolume = Math.Abs(request.Volume);

            if (absVolume < (double) assetPair.MinVolume
                || absVolume > (double)assetPair.MaxVolume
                || absVolume * request.Price > (double)assetPair.MaxOppositeVolume)
            {
                return PlaceOrderErrorResponse(ErrorMessages.WrongVolume, nameof(request.Volume));
            }

            if (request.Price <= 0)
            {
                return PlaceOrderErrorResponse(ErrorMessages.WrongPrice, nameof(request.Price));
            }

            var walletId = await _clientWalletService.GetWalletIdAsync(session.TenantId, session.ClientId);

            var meRequest = new LimitOrder
            {
                Id = Guid.NewGuid().ToString(),
                AccountId = (ulong) session.ClientId,
                BrokerId = session.TenantId,
                WalletId = (ulong) walletId,
                AssetPairId = assetPair.Id.ToString(),
                Price = request.Price.ToString(CultureInfo.InvariantCulture),
                Volume = request.Volume.ToString(CultureInfo.InvariantCulture),
                Type = LimitOrder.Types.LimitOrderType.Limit,
                Timestamp = Timestamp.FromDateTime(DateTime.UtcNow)
            };

            var response = await _matchingEngineClient.Trading.CreateLimitOrderAsync(meRequest);

            if (response.Status != Status.Ok)
            {
                return PlaceOrderErrorResponse(ErrorMessages.MeError(response.Status.ToString(), response.StatusReason), null);
            }

            return new PlaceOrderResponse
            {
                Result = new PlaceOrderResponse.Types.OrderPayload
                {
                    Order = new OrderModel
                    {
                        Id = meRequest.Id,
                        AssetPair = meRequest.AssetPairId,
                        DateTime = meRequest.Timestamp,
                        Price = meRequest.Price,
                        Volume = absVolume.ToString(CultureInfo.InvariantCulture),
                        OrderType = request.Volume > 0 ? OrderSide.Buy.ToString() : OrderSide.Sell.ToString(),
                        TotalCost = (absVolume*request.Price).ToString(CultureInfo.InvariantCulture)
                    }
                }
            };
        }

        public override async Task<PlaceOrderResponse> PlaceMarketOrder(MarketOrderRequest request, ServerCallContext context)
        {
            var session = SessionFromContext(context);

            var assetPair = await _marketDataService.GetAssetPairByTenantAndId(session.TenantId, request.AssetPairId);

            if (assetPair == null || assetPair.IsDisabled)
            {
                return PlaceOrderErrorResponse(ErrorMessages.AssetPairNotFound, nameof(request.AssetPairId));
            }

            var absVolume = Math.Abs(request.Volume);

            if (absVolume < (double)assetPair.MinVolume
                || absVolume > (double)assetPair.MaxVolume)
            {
                return PlaceOrderErrorResponse(ErrorMessages.WrongVolume, nameof(request.Volume));
            }

            var walletId = await _clientWalletService.GetWalletIdAsync(session.TenantId, session.ClientId);

            var meRequest = new MarketOrder
            {
                Id = Guid.NewGuid().ToString(),
                AccountId = (ulong)session.ClientId,
                BrokerId = session.TenantId,
                WalletId = (ulong)walletId,
                AssetPairId = assetPair.Id.ToString(),
                Volume = request.Volume.ToString(CultureInfo.InvariantCulture),
                Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
            };

            var response = await _matchingEngineClient.Trading.CreateMarketOrderAsync(meRequest);

            if (response.Status != Status.Ok)
            {
                return PlaceOrderErrorResponse(ErrorMessages.MeError(response.Status.ToString(), response.StatusReason), null);
            }

            return new PlaceOrderResponse
            {
                Result = new PlaceOrderResponse.Types.OrderPayload
                {
                    Order = new OrderModel
                    {
                        Id = meRequest.Id,
                        AssetPair = meRequest.AssetPairId,
                        DateTime = meRequest.Timestamp,
                        Price = response.Price,
                        Volume = absVolume.ToString(CultureInfo.InvariantCulture),
                        OrderType = request.Volume > 0 ? OrderSide.Buy.ToString() : OrderSide.Sell.ToString(),
                        TotalCost = (absVolume * double.Parse(response.Price)).ToString(CultureInfo.InvariantCulture)
                    }
                }
            };
        }

        public override async Task<CancelOrderResponse> CancelAllOrders(CancelOrdersRequest request, ServerCallContext context)
        {
            var session = SessionFromContext(context);

            var assetPair = await _marketDataService.GetAssetPairByTenantAndId(session.TenantId, request.AssetPairId);

            if (assetPair == null || assetPair.IsDisabled)
            {
                return new CancelOrderResponse
                {
                    Error = new Error
                    {
                        Message = ErrorMessages.AssetPairNotFound
                    }
                };
            }

            var walletId = await _clientWalletService.GetWalletIdAsync(session.TenantId, session.ClientId);

            var side = request.OptionalSideCase == CancelOrdersRequest.OptionalSideOneofCase.Side
                ? request.Side == Side.Buy
                : (bool?)null;

            var meRequest = new LimitOrderMassCancel
            {
                Id = Guid.NewGuid().ToString(),
                BrokerId = session.TenantId,
                WalletId = (ulong)walletId,
                AssetPairId = assetPair.Id.ToString(),
                IsBuy = side
            };

            var response = await _matchingEngineClient.Trading.MassCancelLimitOrderAsync(meRequest);

            if (response.Status != Status.Ok)
            {
                return new CancelOrderResponse
                {
                    Error = new Error
                    {
                        Message = ErrorMessages.MeError(response.Status.ToString(), response.StatusReason)
                    }
                };
            }

            return new CancelOrderResponse {Payload = true};
        }

        public override async Task<CancelOrderResponse> CancelOrder(CancelOrderRequest request, ServerCallContext context)
        {
            var session = SessionFromContext(context);

            //var assetPair = await _marketDataService.GetAssetPairByTenantAndId(session.TenantId, request.AssetPairId);

            //request.OrderId

            var meRequest = new LimitOrderCancel
            {
                Id = Guid.NewGuid().ToString(),
                BrokerId = session.TenantId,
                LimitOrderId =
                {
                    request.OrderId
                }
            };

            var response = await _matchingEngineClient.Trading.CancelLimitOrderAsync(meRequest);

            if (response.Status != Status.Ok && response.Status != Status.LimitOrderNotFound)
            {
                return new CancelOrderResponse
                {
                    Error = new Error
                    {
                        Message = ErrorMessages.MeError(response.Status.ToString(), response.StatusReason)
                    }
                };
            }

            return new CancelOrderResponse { Payload = true };
        }

        private static PlaceOrderResponse PlaceOrderErrorResponse(string message, string field)
        {
            var resp = new PlaceOrderResponse
            {
                Error = new ErrorV1
                {
                    Code = ErrorModelCode.BadRequest.ToString(),
                    Message = message
                }
            };

            if (!string.IsNullOrEmpty(field))
                resp.Error.Field = field;

            return resp;
        }
    }
}
