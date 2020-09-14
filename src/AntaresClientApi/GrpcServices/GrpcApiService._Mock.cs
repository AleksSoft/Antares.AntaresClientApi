using System.Collections.Generic;
using System.Threading.Tasks;
using AntaresClientApi.Domain.Services;
using Assets.Client;
using Assets.Domain.MyNoSql;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
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

        public override Task<SwiftCredentialsResponse> GetSwiftCredentials(SwiftCredentialsRequest request, ServerCallContext context)
        {
            var resp = new SwiftCredentialsResponse();

            resp.Result = new SwiftCredentialsResponse.Types.SwiftCredentials()
            {
                AccountName = "account-name",
                AccountNumber = "1234567890",
                BankAddress = "bank-address",
                Bic = "BIC123",
                CompanyAddress = "Company-name",
                CorrespondentAccount = "123111123123",
                PurposeOfPayment = "Purpose-1"
            };

            return Task.FromResult(resp);
        }

        public override Task<EmptyResponse> SendBankTransferRequest(BankTransferRequest request, ServerCallContext context)
        {
            return Task.FromResult(new EmptyResponse());
        }

        public override async Task<BankCardPaymentDetailsResponse> GetBankCardPaymentDetails(Empty request, ServerCallContext context)
        {
            var session = SessionFromContext(context);
            var walletId = await _clientWalletService.GetWalletIdAsync(session.TenantId, session.ClientId);

            var resp = new BankCardPaymentDetailsResponse();

            resp.Result = new BankCardPaymentDetailsResponse.Types.BankCardPaymentDetails()
            {

                Email = "email@server.com",
                WalletId = walletId.ToString(),
                Phone = "123",
                FirstName = "name",
                AssetId = "USD",
                Address = "asdadad",
                LastName = "name",
                City = "Town",
                Country = "Any",
                Amount = 1000.0,
                Zip = "qwe",
                FailUrl = "",
                OkUrl = ""
            };

            return resp;
        }

        public override Task<BankCardPaymentUrlResponse> GetBankCardPaymentUrl(BankCardPaymentUrlRequest request, ServerCallContext context)
        {
            var resp = new BankCardPaymentUrlResponse();
            resp.Result = new BankCardPaymentUrlResponse.Types.BankCardPaymentUrl()
            {
                OkUrl = "",
                FailUrl = "",
                ReloadRegex = "",
                Url = "",
                UrlsToFormatRegex = ""
            };

            return Task.FromResult(resp);
        }

        public override Task<EthereumSettingsResponse> GetEthereumSettings(Empty request, ServerCallContext context)
        {
            var resp = new EthereumSettingsResponse();
            resp.Result = new EthereumSettingsResponse.Types.EthereumSettings()
            {
                Bitcoin = new EthereumSettingsResponse.Types.BitcoinFee()
                {
                    MaxFee = "0.001",
                    MinFee = "0.00001"
                },
                EthAssetId = "ETH",
                MaxGas = "21000",
                MaxGasPrice = "600000000",
                MinGas = "0",
                MinGasPrice = "20000000",
                StepGas = "5000000",
                StepsCount = 100
            };

            return Task.FromResult(resp);
        }

        public override Task<CryptoDepositAddressResponse> GetCryptoDepositAddress(CryptoDepositAddressRequest request, ServerCallContext context)
        {
            var resp = new CryptoDepositAddressResponse();
            resp.Address = new CryptoDepositAddressResponse.Types.CryptoDepositAddress()
            {
                Address = "ADDRESS",
                AddressExtension = "EXTENSION",
                BaseAddress = "BASE"
            };

            return Task.FromResult(resp);
        }

        public override Task<WithdrawalCryptoInfoResponse> GetWithdrawalCryptoInfo(WithdrawalCryptoInfoRequest request, ServerCallContext context)
        {
            var resp = new WithdrawalCryptoInfoResponse();
            resp.WithdrawalInfo = new WithdrawalCryptoInfoResponse.Types.WithdrawalCryptoInfo()
            {
                AddressExtensionMandatory = false,
                BaseAddressTitle = "BASE-ADDRESS",
                AddressExtensionTitle = "EXTENSION-TITLE"
            };

            return Task.FromResult(resp);
        }

        public override Task<CheckCryptoAddressResponse> IsCryptoAddressValid(CheckCryptoAddressRequest request, ServerCallContext context)
        {
            var resp = new CheckCryptoAddressResponse();
            resp.Result = new CheckCryptoAddressResponse.Types.CheckCryptoAddressPayload()
            {
                IsValid = true
            };

            return Task.FromResult(resp);
        }

        public override Task<SwiftCashoutInfoResponse> GetSwiftCashoutInfo(Empty request, ServerCallContext context)
        {
            var resp = new SwiftCashoutInfoResponse();
            resp.Result = new SwiftCashoutInfoResponse.Types.SwiftCashoutInfo()
            {
                Bic = "BIC123",
                AccHolderAddress = "AccHolderAddress",
                AccHolderCity = "City",
                AccHolderCountry = "Country",
                AccHolderZipCode = "ZipCode",
                AccName = "AccName",
                AccNumber = "Acc-number",
                BankName = "Bank-name"
            };

            return Task.FromResult(resp);
        }

        public override Task<SwiftCashoutFeeResponse> GetSwiftCashoutFee(SwiftCashoutFeeRequest request, ServerCallContext context)
        {
            var resp = new SwiftCashoutFeeResponse();
            resp.Result = new SwiftCashoutFeeResponse.Types.SwiftCashoutFee()
            {
                AssetId = "ETH",
                Country = "AnyCountry",
                Size = "0.005"
            };

            return Task.FromResult(resp);
        }

        public override Task<SwiftCashoutResponse> SwiftCashout(SwiftCashoutRequest request, ServerCallContext context)
        {
            var resp = new SwiftCashoutResponse();
            resp.Result = new SwiftCashoutResponse.Types.SwiftCashoutData()
            {
                TransferId = "123123"
            };

            return Task.FromResult(resp);
        }

        public override Task<EmptyResponse> CryptoCashout(CryptoCashoutRequest request, ServerCallContext context)
        {
            var resp = new EmptyResponse();
            return Task.FromResult(resp);
        }

        #endregion


        #region Wallets

        public override Task<WalletsResponse> GetWallets(Empty request, ServerCallContext context)
        {
            var resp = new WalletsResponse();
            resp.Result = new WalletsResponse.Types.GetWalletsPayload()
            {
                Lykke = new WalletsResponse.Types.LykkeWalletsPayload()
                {
                    Equity = "10000",
                    Assets =
                    {
                        new WalletsResponse.Types.WalletAsset()
                        {
                            Id = "BTC",
                            AssetPairId = "BTCUSD",
                            Symbol = "BTC",
                            Balance = "1",
                            Reserved = "0",
                            Accuracy = 4,
                            AmountInBase = "10000",
                            Name = "Bitcoin",
                            CategoryId = "Crypto",
                            HideIfZero = false,
                            IssuerId = "market"
                        }
                    }
                }
            };


            return Task.FromResult(resp);
        }

        public override Task<WalletResponse> GetWallet(WalletRequest request, ServerCallContext context)
        {
            var resp = new WalletResponse();
            resp.Result = new WalletResponse.Types.WalletPayload()
            {
                Id = "BTC",
                AssetPairId = "BTCUSD",
                Symbol = "BTC",
                Balance = "1",
                Reserved = "0",
                Accuracy = 4,
                AmountInBase = "10000",
                Name = "Bitcoin",
                CategoryId = "Crypto",
                HideIfZero = false,
                IssuerId = "market"
            };

            return Task.FromResult(resp);
        }

        public override Task<GenerateWalletResponse> GenerateWallet(GenerateWalletRequest request, ServerCallContext context)
        {
            var resp = new GenerateWalletResponse();
            resp.Result = new GenerateWalletResponse.Types.WalletAddress()
            {
                Address = "qwweqweqwe",
                AddressExtension = new GenerateWalletResponse.Types.BcnAddressExtension()
                {
                    AddressExtensionDisplayName = "Display-name",
                    BaseAddressDisplayName = "base-address-display",
                    DepositAddressExtension = "deposit-address",
                    TypeForDeposit = "type-of-deposit",
                    TypeForWithdrawal = "type-of-withdrawal"
                }
            };

            return Task.FromResult(resp);
        }

        public override Task<EmptyResponseV2> GenerateWalletV2(GenerateWalletV2Request request, ServerCallContext context)
        {
            var resp = new EmptyResponseV2();
            return Task.FromResult(resp);
        }

        public override Task<PrivateWalletsResponse> GetPrivateWallets(Empty request, ServerCallContext context)
        {
            var resp = new PrivateWalletsResponse();
            resp.Result = new PrivateWalletsResponse.Types.PrivateWalletsPayload();
            resp.Result.Wallets.Add(new PrivateWallet()
            {
                Address = "address-wallet",
                Name = "name-wallet",
                Balances =
                {
                    new BalanceRecord()
                    {
                        AssetId = "BTC",
                        Balance = "1",
                        BaseAssetId = "USD",
                        AmountInBase = "10000"
                    }
                },
                Blockchain = "Bitcoin",
                IsColdStorage = false,
                LargeIconUrl = "",
                MediumIconUrl = "",
                Number = 1,
                SmallIconUrl = "icon-url",
                EncodedPrivateKey = "" //todo: remove EncodedPrivateKey from contract
            });

            return Task.FromResult(resp);
        }

        #endregion


        #region Other

        [AllowAnonymous]
        public override Task<CountryPhoneCodesResponse> GetCountryPhoneCodes(Empty request, ServerCallContext context)
        {
            var resp = new CountryPhoneCodesResponse();
            resp.Result = new CountryPhoneCodesResponse.Types.CountryPhoneCodes()
            {
                Current = "bgn",
                CountriesList =
                {
                    new Country()
                    {
                        Id = "usa",
                        Name = "USA",
                        Iso2 = "us",
                        Prefix = "+1"
                    },
                    new Country()
                    {
                        Id = "rus",
                        Name = "Russia",
                        Iso2 = "ru",
                        Prefix = "+7"
                    }
                }
            };

            return Task.FromResult(resp);
        }

        public override Task<AppSettingsResponse> GetAppSettings(Empty request, ServerCallContext context)
        {
            var resp = new AppSettingsResponse()
            {
                Result = new AppSettingsResponse.Types.AppSettingsData()
                {
                    BaseAsset = new AppSettingsResponse.Types.ApiAsset()
                    {
                        Id = "usd",
                        Symbol = "USD",
                        Name = "USD",
                        Accuracy = 4,
                        SwiftDepositEnabled = false,
                        CategoryId = "currency",
                        BankCardsDepositEnabled = false,
                        BlockchainDepositEnabled = false,
                        HideDeposit = true,
                        HideWithdraw = true,
                        KycNeeded = true,
                        OtherDepositOptionsEnabled = false
                    },
                    DebugMode = false,
                    DepositUrl = "deposit-url",

                }
            }; //todo: remove not usable app settings

            return Task.FromResult(resp);
        }

        public override Task<AssetDisclaimersResponse> GetAssetDisclaimers(Empty request, ServerCallContext context)
        {
            var resp = new AssetDisclaimersResponse();
            resp.Result = new AssetDisclaimersResponse.Types.AssetDisclaimersPayload() {Disclaimers = { }};
            return Task.FromResult(resp);
        }

        public override Task<EmptyResponse> ApproveAssetDisclaimer(AssetDisclaimerRequest request, ServerCallContext context)
        {
            var resp = new EmptyResponse();
            return Task.FromResult(resp);
        }

        public override Task<EmptyResponse> DeclineAssetDisclaimer(AssetDisclaimerRequest request, ServerCallContext context)
        {
            var resp = new EmptyResponse();
            return Task.FromResult(resp);
        }

        private static List<IServerStreamWriter<PriceUpdate>> _priceUpdate = new List<IServerStreamWriter<PriceUpdate>>();
        private static List<IServerStreamWriter<CandleUpdate>> _candleUpdate = new List<IServerStreamWriter<CandleUpdate>>();
        private static List<IServerStreamWriter<Orderbook>> _orderBookUpdate = new List<IServerStreamWriter<Orderbook>>();
        private static List<IServerStreamWriter<PublicTradeUpdate>> _publicTradeUpdate = new List<IServerStreamWriter<PublicTradeUpdate>>();

        public override Task GetPriceUpdates(PriceUpdatesRequest request, IServerStreamWriter<PriceUpdate> responseStream, ServerCallContext context)
        {
            _priceUpdate.Add(responseStream);
            return Task.CompletedTask;
        }

        public override Task GetCandleUpdates(CandleUpdatesRequest request, IServerStreamWriter<CandleUpdate> responseStream, ServerCallContext context)
        {
            _candleUpdate.Add(responseStream);
            return Task.CompletedTask;
        }

        public override Task GetOrderbookUpdates(OrderbookUpdatesRequest request,
            IServerStreamWriter<Orderbook> responseStream,
            ServerCallContext context)
        {
            _orderBookUpdate.Add(responseStream);
            return Task.CompletedTask;
        }

        public override Task GetPublicTradeUpdates(PublicTradesUpdatesRequest request,
            IServerStreamWriter<PublicTradeUpdate> responseStream,
            ServerCallContext context)
        {
            _publicTradeUpdate.Add(responseStream);
            return Task.CompletedTask;
        }

        #endregion
    }
}
