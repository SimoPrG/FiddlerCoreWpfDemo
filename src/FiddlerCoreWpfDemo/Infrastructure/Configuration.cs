using System;
using System.Net;
using System.Threading;
using Telerik.NetworkConnections;

namespace FiddlerCoreWpfDemo.Infrastructure
{
    public sealed class Configuration : IDisposable
    {
        private const string HttpScheme = "http";
        private const string Localhost = "127.0.0.1";

        private bool capture = true;
        private readonly ReaderWriterLockSlim captureLock = new ReaderWriterLockSlim();
        private readonly NetworkConnectionsConfig networkConnectionsConfig = new NetworkConnectionsConfig();
        private readonly FiddlerCoreConfig fiddlerCoreConfig = new FiddlerCoreConfig();

        private Configuration()
        {
            networkConnectionsConfig.ConfigureNetworkConnections();
            ProxySettings upstreamProxySettings = networkConnectionsConfig.UpstreamProxySettings;
            fiddlerCoreConfig.ConfigureFiddlerCore(upstreamProxySettings);
        }

        public static Configuration Instance { get; } = new Configuration();

        public bool Capture
        {
            get
            {
                try
                {
                    this.captureLock.EnterReadLock();
                    return this.capture;
                }
                finally
                {
                    this.captureLock.ExitReadLock();
                }
            }

            set
            {
                try
                {
                    this.captureLock.EnterWriteLock();
                    this.capture = value;
                }
                finally
                {
                    this.captureLock.ExitWriteLock();
                }
            }
        }

        public ushort ListenPort => fiddlerCoreConfig.ListenPort;

        public Uri ProxyUri => new Uri($"{HttpScheme}://{Localhost}:{fiddlerCoreConfig.ListenPort}");

        public IWebProxy UpstreamProxy { get; } = new UpstreamProxy();

        public void Dispose()
        {
            fiddlerCoreConfig.Dispose();
            networkConnectionsConfig.Dispose();
        }
    }
}
