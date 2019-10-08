using FiddlerCoreWpfDemo.Infrastructure;
using System.Windows;
using System.Windows.Threading;
using Telerik.NetworkConnections;

namespace FiddlerCoreWpfDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// You can toggle this const in order to execute performance tests with and without FiddlerCore
        /// </summary>
        private const bool UseFiddlerCore = true;
        private readonly NetworkConnectionsConfig networkConnectionsConfig = UseFiddlerCore ? new NetworkConnectionsConfig() : null;
        private readonly FiddlerCoreConfig fiddlerCoreConfig = UseFiddlerCore ? new FiddlerCoreConfig() : null;
        private readonly HttpClientConfig httpClientConfig = new HttpClientConfig();

        /// <summary>
        /// A global exception handler for simplicity.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Process unhandled exception
            MessageBox.Show(e.Exception.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            // Prevent default unhandled exception processing
            e.Handled = true;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (UseFiddlerCore)
            {
                networkConnectionsConfig.ConfigureNetworkConnections();
                ProxySettings upstreamProxySettings = networkConnectionsConfig.UpstreamProxySettings;
                fiddlerCoreConfig.ConfigureFiddlerCore(upstreamProxySettings);

            }

            httpClientConfig.ConfigureHttpClient(UseFiddlerCore ? fiddlerCoreConfig.ListenPort : (ushort)0);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            httpClientConfig.Dispose();

            if (UseFiddlerCore)
            {
                fiddlerCoreConfig.Dispose();

                networkConnectionsConfig.Dispose();
            }
        }
    }
}
