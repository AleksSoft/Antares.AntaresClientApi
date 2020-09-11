using System.Threading.Tasks;
using AntaresClientApi.Domain.Services;
using Assets.Client;
using Assets.Domain.MyNoSql;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using Swisschain.Lykke.AntaresWalletApi.ApiContract;

namespace AntaresClientApi.GrpcServices
{
    public partial class GrpcApiService 
    {
        #region MarketData

        public override Task<MarketsResponse> GetMarkets(MarketsRequest request, ServerCallContext context)
        {
            var result = new MarketsResponse();
            return Task.FromResult(result);
        }

        public override Task<PublicTradesResponse> GetPublicTrades(PublicTradesRequest request, ServerCallContext context)
        {
            var resp = new PublicTradesResponse();
            return Task.FromResult(resp);
        }

        public override Task<ExplorerLinksResponse> GetExplorerLinks(ExplorerLinksRequest request, ServerCallContext context)
        {
            var resp = new ExplorerLinksResponse();
            return Task.FromResult(resp);
        }



        #endregion


        #region ClientData

        public override Task<AmountInBaseAssetResponse> GetAmountInBaseAsset(AmountInBaseRequest request, ServerCallContext context)
        {
            var result = new AmountInBaseAssetResponse()
            {
                Values =
                {
                    new AmountInBaseAssetResponse.Types.AmountInBasePayload()
                    {
                        AssetId = "USD",
                        AmountInBase = "0"
                    }
                }
            };

            return Task.FromResult(result);
        }

        public override Task<PushSettingsResponse> GetPushSettings(Empty request, ServerCallContext context)
        {
            var response = new PushSettingsResponse()
            {
                Result = new PushSettingsResponse.Types.PushSettingsPayload() {Enabled = false}
            };

            return Task.FromResult(response);
        }

        public override Task<EmptyResponse> SetPushSettings(PushSettingsRequest request, ServerCallContext context)
        {
            return Task.FromResult(new EmptyResponse());
        }

        public override async Task<AssetTradesResponse> GetAssetTrades(AssetTradesRequest request, ServerCallContext context)
        {
            var response = new AssetTradesResponse() { };
            return response;
        }

        public override async Task<FundsResponse> GetFunds(FundsRequest request, ServerCallContext context)
        {
            var resp = new FundsResponse();
            return resp;
        }



        #endregion

        #region ClientState

        public override Task<WatchlistsResponse> GetWatchlists(Empty request, ServerCallContext context)
        {
            return base.GetWatchlists(request, context);
        }

        public override Task<WatchlistResponse> GetWatchlist(WatchlistRequest request, ServerCallContext context)
        {
            return base.GetWatchlist(request, context);
        }

        public override Task<WatchlistResponse> AddWatchlist(AddWatchlistRequest request, ServerCallContext context)
        {
            return base.AddWatchlist(request, context);
        }

        public override Task<WatchlistResponse> UpdateWatchlist(UpdateWatchlistRequest request, ServerCallContext context)
        {
            return base.UpdateWatchlist(request, context);
        }

        public override Task<DeleteWatchlistResponse> DeleteWatchlist(DeleteWatchlistRequest request, ServerCallContext context)
        {
            return base.DeleteWatchlist(request, context);
        }

        #endregion


        #region KYC

        public override Task<TierInfoRespone> GetTierInfo(Empty request, ServerCallContext context)
        {
            var resp = new TierInfoRespone()
            {
                Result = new TierInfoPayload()
                {
                    CurrentTier = new CurrentTier()
                    {
                        Asset = "USD",
                        Current = "0",
                        MaxLimit = "100000",
                        Tier = "Advanced"
                    },
                    NextTier = null,
                    QuestionnaireAnswered = true
                }
            };

            return Task.FromResult(resp);
        }

        public override Task<PersonalDataResponse> GetPersonalData(Empty request, ServerCallContext context)
        {
            var resp = new PersonalDataResponse()
            {
                Result = new PersonalData()
                {
                    Email = "mail@test.com",
                    Phone = "123123123",
                    FullName = "Full-Name",
                    FirstName = "First-Name",
                    LastName = "Last-Name",
                    Address = "Street 54, ap 41",
                    City = "Nice",
                    Country = "Switzerland"
                }
            };

            return Task.FromResult(resp);
        }

        public override Task<KycDocumentsResponse> GetKycDocuments(Empty request, ServerCallContext context)
        {
            var resp = new KycDocumentsResponse()
            {
                Result = { }
            };

            return Task.FromResult(resp);
        }

        public override Task<EmptyResponseV2> SetAddress(SetAddressRequest request, ServerCallContext context)
        {
            return Task.FromResult(new EmptyResponseV2());
        }

        public override Task<EmptyResponseV2> SetZip(SetZipRequest request, ServerCallContext context)
        {
            return Task.FromResult(new EmptyResponseV2());
        }

        public override Task<EmptyResponse> UploadKycFile(KycFileRequest request, ServerCallContext context)
        {
            return Task.FromResult(new EmptyResponse());
        }

        public override Task<QuestionnaireResponse> GetQuestionnaire(Empty request, ServerCallContext context)
        {
            var resp = new QuestionnaireResponse() {Result = new QuestionnaireResponse.Types.QuestionnairePayload()};
            return Task.FromResult(resp);
        }

        public override Task<EmptyResponse> SaveQuestionnaire(AnswersRequest request, ServerCallContext context)
        {
            return Task.FromResult(new EmptyResponse());
        }

        public override Task<EmptyResponse> SubmitProfile(SubmitProfileRequest request, ServerCallContext context)
        {
            return Task.FromResult(new EmptyResponse());
        }

        #endregion


        #region CashInOut
        


        #endregion

    }
}
