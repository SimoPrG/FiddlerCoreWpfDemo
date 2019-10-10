using System;
using System.Net;

using Infrastructure;

namespace AppProxy
{
    public class FiddlerCoreWebProxy : IWebProxy
    {
        public ICredentials Credentials { get; set; }

        public Uri GetProxy(Uri destination) => Configuration.Instance.ProxyUri;

        public bool IsBypassed(Uri host) => false;
    }
}
