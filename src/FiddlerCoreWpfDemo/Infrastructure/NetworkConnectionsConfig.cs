using System;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;

using Telerik.NetworkConnections;

namespace FiddlerCoreWpfDemo.Infrastructure
{
    class NetworkConnectionsConfig : IDisposable
    {
        private const string DefaultNetworkConnectionNamespace = "WinINet";
        private const string DefaultNetworkConnectionName = "DefaultLAN";

        private CompositionContainer compositionContainer;
        private NetworkConnectionsManager networkConnectionsManager;

        public void ConfigureNetworkConnections()
        {
            string networkConnectionsDir = Path.Combine(Common.AssemblyDir, "lib", "NetworkConnections");

            using (AssemblyCatalog networkConnectionsManagerCatalog = new AssemblyCatalog(typeof(NetworkConnectionsManager).Assembly))
            using (DirectoryCatalog networkConnectionsCatalog = new DirectoryCatalog(networkConnectionsDir))
            using (AggregateCatalog aggregateCatalog = new AggregateCatalog(networkConnectionsManagerCatalog, networkConnectionsCatalog))
            {
                this.compositionContainer = new CompositionContainer(aggregateCatalog);
                this.networkConnectionsManager = compositionContainer.GetExportedValue<NetworkConnectionsManager>();
            }

            this.networkConnectionsManager.ProxySettingsChanged += NetworkConnectionsManager_ProxySettingsChanged;

            this.UpdateUpstreamProxySettings();
        }

        public ProxySettings UpstreamProxySettings { get; private set; }


        public event EventHandler UpstreamProxySettingsChanged;

        public void Dispose()
        {
            if (!ReferenceEquals(this.compositionContainer, null))
            {
                this.compositionContainer.Dispose();
            }

            if (!ReferenceEquals(this.networkConnectionsManager, null))
            {
                this.networkConnectionsManager.ProxySettingsChanged -= NetworkConnectionsManager_ProxySettingsChanged;
            }
        }

        protected virtual void OnUpstreamProxySettingsChanged(EventArgs ea)
        {
            this.UpstreamProxySettingsChanged?.Invoke(this, ea);
        }

        private void NetworkConnectionsManager_ProxySettingsChanged(object sender, ProxySettingsChangedEventArgs e)
        {
            if (e.NetworkConnectionFullName.Namespace == DefaultNetworkConnectionNamespace &&
                e.NetworkConnectionFullName.Name == DefaultNetworkConnectionName)
            {
                this.UpdateUpstreamProxySettings();
                this.OnUpstreamProxySettingsChanged(EventArgs.Empty);
            }
        }

        private void UpdateUpstreamProxySettings()
        {
            var networkConnectionNames = this.networkConnectionsManager.GetAllConnectionFullNames();

            this.UpstreamProxySettings = this.networkConnectionsManager
                .GetCurrentProxySettingsForConnection(networkConnectionNames
                .First(n =>
                n.Namespace == DefaultNetworkConnectionNamespace &&
                n.Name == DefaultNetworkConnectionName));
        }
    }
}
