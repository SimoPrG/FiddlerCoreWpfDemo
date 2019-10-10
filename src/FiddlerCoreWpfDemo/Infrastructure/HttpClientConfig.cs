using System;
using System.Net.Http;

namespace FiddlerCoreWpfDemo.Infrastructure
{
    public class HttpClientConfig : IDisposable
    {
        public static HttpClient HttpClient { get; private set; }

        public void ConfigureHttpClient()
        {
            HttpClient = new HttpClient();
        }

        public void Dispose()
        {
            HttpClient?.Dispose();
        }
    }
}
