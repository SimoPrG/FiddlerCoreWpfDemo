using Fiddler;
using System;
using System.IO;
using Telerik.NetworkConnections;

namespace FiddlerCoreWpfDemo.Infrastructure
{
    public class FiddlerCoreConfig : IDisposable
    {
        public void ConfigureFiddlerCore(ProxySettings upstreamProxySettings)
        {
            this.EnsureRootCertificate();

            FiddlerApplication.Prefs.SetBoolPref("fiddler.ui.rules.bufferresponses", false);

            FiddlerCoreStartupSettings startupSettings =
                new FiddlerCoreStartupSettingsBuilder()
                    .SetUpstreamProxySettingsTo(upstreamProxySettings)
                    .DecryptSSL()
                    .Build();

            FiddlerApplication.Startup(startupSettings);
        }

        public ushort ListenPort => (ushort)CONFIG.ListenPort;

        public void Dispose()
        {
            FiddlerApplication.Shutdown();
        }

        private void EnsureRootCertificate()
        {
            string certMakerPath = Path.Combine(Common.AssemblyDir, "lib", "CertMaker.dll");
            FiddlerApplication.Prefs.SetStringPref("fiddler.certmaker.assembly", certMakerPath);
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

            if (!CertMaker.rootCertIsTrusted())
            {
                CertMaker.trustRootCert();
            }
        }
    }
}
