using System;
using System.Linq;

using Telerik.NetworkConnections;
using Telerik.NetworkConnections.Windows;

namespace Infrastructure
{
    internal class NetworkConnectionsConfig : IDisposable
    {
        private NetworkConnectionsManager networkConnectionsManager;

        public void ConfigureNetworkConnections()
        {
            this.networkConnectionsManager = new NetworkConnectionsManager(new[]
            {
                new WinINetNetworkConnectionsDetector()
            });

            this.networkConnectionsManager.ProxySettingsChanged += NetworkConnectionsManager_ProxySettingsChanged;

            this.UpdateUpstreamProxySettings();
        }

        public ProxySettings UpstreamProxySettings { get; private set; }


        public event EventHandler UpstreamProxySettingsChanged;

        public void Dispose()
        {
            if (!ReferenceEquals(this.networkConnectionsManager, null))
            {
                this.networkConnectionsManager.ProxySettingsChanged -= NetworkConnectionsManager_ProxySettingsChanged;
                this.networkConnectionsManager.Dispose();
            }
        }

        protected virtual void OnUpstreamProxySettingsChanged(EventArgs ea)
        {
            this.UpstreamProxySettingsChanged?.Invoke(this, ea);
        }

        private void NetworkConnectionsManager_ProxySettingsChanged(object sender, ProxySettingsChangedEventArgs e)
        {
            this.UpdateUpstreamProxySettings();
            this.OnUpstreamProxySettingsChanged(EventArgs.Empty);
        }

        private void UpdateUpstreamProxySettings()
        {
            var connectionName = this.networkConnectionsManager
                .GetAllConnectionFullNames()
                .First();

            this.UpstreamProxySettings = this.networkConnectionsManager
                .GetCurrentProxySettingsForConnection(connectionName);
        }
    }
}
