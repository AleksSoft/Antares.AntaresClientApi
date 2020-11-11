using System.Threading.Tasks;
using AntaresClientApi.Domain.Models.MyNoSql;
using Assets.Domain.Entities;

namespace AntaresClientApi.Domain.Services
{
    public interface ICashInOutProcessor
    {
        Task ChangeBalance(ClientWalletEntity wallet,
            Asset asset,
            decimal amount,
            string comment);
    }
}
