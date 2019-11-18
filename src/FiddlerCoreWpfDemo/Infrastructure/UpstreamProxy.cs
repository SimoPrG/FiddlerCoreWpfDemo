using Fiddler;
using System;
using System.Net;

namespace FiddlerCoreWpfDemo.Infrastructure
{
    internal class UpstreamProxy : IWebProxy
    {
        public ICredentials Credentials { get; set; } = CredentialCache.DefaultNetworkCredentials;

        public Uri GetProxy(Uri destination)
        {
            return CheckUri(destination, () => destination, (uri) => uri);
        }

        public bool IsBypassed(Uri host)
        {
            return CheckUri(host, () => true, (uri) => false);
        }

        private T CheckUri<T>(Uri uri, Func<T> bypass, Func<Uri, T> proxy)
        {
            if (!uri.IsAbsoluteUri)
            {
                return bypass();
            }

            if (ReferenceEquals(FiddlerApplication.oProxy, null))
            {
                return bypass();
            }

            IPEndPoint ipEndPoint = FiddlerApplication.oProxy.FindGatewayForOrigin(uri.Scheme, $"{uri.Host}:{uri.Port}");
            if (ReferenceEquals(ipEndPoint, null))
            {
                return bypass();
            }

            return proxy(new UriBuilder(ipEndPoint.ToString()).Uri);
        }
    }
}
