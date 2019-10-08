using System;
using System.Net.Http;

namespace FiddlerCoreWpfDemo.Infrastructure
{
    public class HttpClientConfig : IDisposable
    {
        public static HttpClient HttpClient { get; private set; }

        public void ConfigureHttpClient(ushort fiddlerCorePort)
        {
            
            if (fiddlerCorePort == 0) // FiddlerCore is not used
            {
                // Please, provide your proxy settings if needed.
                HttpClient = new HttpClient();
                return;
            }

            HttpClient = new HttpClient(
                new HttpClientHandler()
                {
                    AllowAutoRedirect = true,
                    Proxy = new HttpClientProxy(fiddlerCorePort),
                    UseProxy = true
                });
        }

        public void Dispose()
        {
            HttpClient?.Dispose();
        }
    }
}
