using System.Threading.Tasks;
using AntaresClientApi.Domain.Models;

namespace AntaresClientApi.Domain.Services
{
    public interface IClientWalletService
    {
        Task RegisterDefaultWallets(ClientIdentity client);
    }
}
