using System;
using System.Threading;
using Assets.Client;
using Autofac;

namespace AntaresClientApi.Domain.Services
{
    public class AssetManagment: IStartable
    {
        private Timer _timer;
        private readonly IAssetsClient _assetsClient;

        public AssetManagment(IAssetsClient assetsClient)
        {
            _assetsClient = assetsClient;
        }

        public void Start()
        {
            _timer = new Timer(UpdateData, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
        }

        private void UpdateData(object state)
        {
            try
            {
                var assets = _assetsClient.Assets.GetAllAsync().GetAwaiter().GetResult();



            }
            catch (Exception ex)
            {

            }

        }
    }
}
