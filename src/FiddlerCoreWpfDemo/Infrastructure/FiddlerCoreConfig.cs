using System;
using System.IO;

using Fiddler;
using Telerik.NetworkConnections;

using FiddlerCoreWpfDemo.Properties;

namespace FiddlerCoreWpfDemo.Infrastructure
{
    internal class FiddlerCoreConfig : IDisposable
    {
        private readonly SessionsPersister sessionsPersister = new SessionsPersister();

        public void ConfigureFiddlerCore(ProxySettings upstreamProxySettings)
        {
            this.EnsureRootCertificate();

            FiddlerApplication.BeforeRequest += this.sessionsPersister.AddSession;

            FiddlerApplication.Prefs.SetBoolPref("fiddler.ui.rules.bufferresponses", false);

            FiddlerCoreStartupSettings startupSettings =
                new FiddlerCoreStartupSettingsBuilder()
                    .ListenOnPort(Settings.Default.FiddlerCoreListenPort)
                    .SetUpstreamProxySettingsTo(upstreamProxySettings)
                    .DecryptSSL()
                    .Build();

            FiddlerApplication.Startup(startupSettings);
        }

        public ushort ListenPort => (ushort)CONFIG.ListenPort;

        public void Dispose()
        {
            FiddlerApplication.Shutdown();
            FiddlerApplication.BeforeRequest -= this.sessionsPersister.AddSession;
            this.sessionsPersister.Cancel();
            this.sessionsPersister.PersistSessionsAsync(false).GetAwaiter().GetResult();
        }

        private void EnsureRootCertificate()
        {
            CertMaker.EnsureReady();
            ICertificateProvider5 certificateProvider = (ICertificateProvider5)CertMaker.oCertProvider;

            string rootCertificatePath = Path.Combine(Common.AssemblyDir, "..", "..", "RootCertificate.p12");
            string rootCertificatePassword = "S0m3T0pS3cr3tP4ssw0rd";
            if (File.Exists(rootCertificatePath))
            {
                certificateProvider.ReadRootCertificateAndPrivateKeyFromPkcs12File(rootCertificatePath, rootCertificatePassword);
            }
            else
            {
                certificateProvider.CreateRootCertificate();
                certificateProvider.WriteRootCertificateAndPrivateKeyToPkcs12File(rootCertificatePath, rootCertificatePassword);
            }

            if (!certificateProvider.rootCertIsTrusted(out bool userTrusted, out bool machineTrusted))
            {
                certificateProvider.TrustRootCertificate();
            }
        }
    }
}
