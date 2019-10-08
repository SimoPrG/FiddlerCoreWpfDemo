using System;
using System.Net;

namespace FiddlerCoreWpfDemo.Infrastructure
{
    internal class HttpClientProxy : IWebProxy
    {
        private const string HttpScheme = "http";
        private const string Localhost = "localhost";
        private readonly ushort port;

        public ICredentials Credentials { get; set; }

        public HttpClientProxy(ushort port)
        {
            this.port = port;
        }

        public Uri GetProxy(Uri destination)
        {
            return new Uri($"{HttpScheme}://{Localhost}:{port}");
        }

        public bool IsBypassed(Uri host)
        {
            return false;
        }
    }
}
