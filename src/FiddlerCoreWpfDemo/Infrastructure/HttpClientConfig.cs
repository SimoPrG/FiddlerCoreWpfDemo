using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FiddlerCoreWpfDemo.Infrastructure
{
    public class HttpClientConfig : IDisposable
    {
        public static HttpClient HttpClient { get; private set; }

        public void ConfigureHttpClient(ushort fiddlerCorePort)
        {
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
