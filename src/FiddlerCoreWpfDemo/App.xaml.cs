using System.Windows;
using System.Windows.Threading;

namespace FiddlerCoreWpfDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
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
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
        }
    }
}
