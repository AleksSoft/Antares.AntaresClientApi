using System;
using System.Globalization;
using System.Threading.Tasks;
using AntaresClientApi.Domain.Models.Exceptions;
using AntaresClientApi.Domain.Models.MyNoSql;
using Assets.Domain.Entities;
using Common;
using MatchingEngine.Client;
using MatchingEngine.Client.Contracts.Incoming;
using Microsoft.Extensions.Logging;

namespace AntaresClientApi.Domain.Services
{
    public class CashInOutProcessor : ICashInOutProcessor
    {
        private readonly IMatchingEngineClient _matchingEngineClient;
        private readonly ILogger<CashInOutProcessor> _logger;

        public CashInOutProcessor(IMatchingEngineClient matchingEngineClient, ILogger<CashInOutProcessor> logger)
        {
            _matchingEngineClient = matchingEngineClient;
            _logger = logger;
        }

        public async Task ChangeBalance(ClientWalletEntity wallet,
            Asset asset,
            decimal amount,
            string comment)
        {
            var operationId = Guid.NewGuid().ToString("N");

            var request = new CashInOutOperation
            {
                BrokerId = wallet.Client.TenantId,
                AccountId = (ulong) wallet.Client.ClientId,
                WalletId = (ulong) wallet.WalletId,
                AssetId = asset.Symbol,
                Description = comment,
                Volume = amount.ToString(CultureInfo.InvariantCulture),
                Id = operationId
            };

            _logger.LogInformation($"CashInRequest: {request.ToJson()}");

            var response = await _matchingEngineClient.CashOperations.CashInOutAsync(request);

            _logger.LogInformation(
                "ME operations {MeOperation}, Id: {OperationId}. Status: {MeStatus}, Reason: {MeReason}; BrokerId: {TenantId}; AccountId: {AccountId}; WalletId: {WalletId}; Asset: {AssetSymbol}; Volume: {Volume}",
                "CashInOut",
                operationId,
                response.Status.ToString(),
                response.StatusReason,
                wallet.Client.TenantId,
                (ulong) wallet.Client.ClientId,
                (ulong) wallet.WalletId,
                asset.Symbol,
                amount);

            if (response.Status != Status.Ok)
            {
                throw new MatchingEngineException(response.Status.ToString(),
                    response.StatusReason,
                    "CashInOutAsync",
                    "CashOperations",
                    request,
                    response);
            }
        }
    }
}
