using System.Windows;
using System.Windows.Threading;

using FiddlerCoreWpfDemo.Infrastructure;

namespace FiddlerCoreWpfDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly Configuration configuration = Configuration.Instance;
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
            httpClientConfig.ConfigureHttpClient();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            httpClientConfig.Dispose();

            configuration.Dispose();
        }
    }
}
