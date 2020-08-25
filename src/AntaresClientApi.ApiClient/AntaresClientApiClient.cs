using Swisschain.Antares.AntaresClientApi.ApiClient.Common;
using Swisschain.Antares.AntaresClientApi.ApiContract;

namespace Swisschain.Antares.AntaresClientApi.ApiClient
{
    public class AntaresClientApiClient : BaseGrpcClient, IAntaresClientApiClient
    {
        public AntaresClientApiClient(string serverGrpcUrl) : base(serverGrpcUrl)
        {
            Monitoring = new Monitoring.MonitoringClient(Channel);
        }

        public Monitoring.MonitoringClient Monitoring { get; }
    }
}
