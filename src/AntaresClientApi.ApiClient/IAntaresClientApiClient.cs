using Swisschain.Antares.AntaresClientApi.ApiContract;

namespace Swisschain.Antares.AntaresClientApi.ApiClient
{
    public interface IAntaresClientApiClient
    {
        Monitoring.MonitoringClient Monitoring { get; }
    }
}
