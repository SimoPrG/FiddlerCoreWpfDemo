using System;
using Telerik.NetworkConnections;

namespace Infrastructure
{
    public class Configuration : IDisposable
    {
        private const string HttpScheme = "http";
        private const string Localhost = "127.0.0.1";

        private static Configuration instance;

        private readonly NetworkConnectionsConfig networkConnectionsConfig = new NetworkConnectionsConfig();
        private readonly FiddlerCoreConfig fiddlerCoreConfig = new FiddlerCoreConfig();

        private Configuration()
        {
            networkConnectionsConfig.ConfigureNetworkConnections();
            ProxySettings upstreamProxySettings = networkConnectionsConfig.UpstreamProxySettings;
            fiddlerCoreConfig.ConfigureFiddlerCore(upstreamProxySettings);
        }

        public static Configuration Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Configuration();
                }

                return instance;
            }
        }

        public Uri ProxyUri => new Uri($"{HttpScheme}://{Localhost}:{fiddlerCoreConfig.ListenPort}");

        public void Dispose()
        {
            fiddlerCoreConfig.Dispose();
            networkConnectionsConfig.Dispose();
        }
    }
}
