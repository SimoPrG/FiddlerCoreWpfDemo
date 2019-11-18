using System;
using System.Net;

using FiddlerCoreWpfDemo.Infrastructure;

namespace FiddlerCoreWpfDemo.Proxy
{
    public class FiddlerCoreWebProxy : IWebProxy
    {
        public ICredentials Credentials
        {
            get
            {
                return Configuration.Instance.Capture ?
                    null :
                    Configuration.Instance.UpstreamProxy.Credentials;
            }

            set
            {
                if (Configuration.Instance.Capture)
                {
                    return;
                }

                Configuration.Instance.UpstreamProxy.Credentials = value;
            }
        }

        public Uri GetProxy(Uri destination) =>
            Configuration.Instance.Capture ?
            Configuration.Instance.ProxyUri :
            Configuration.Instance.UpstreamProxy.GetProxy(destination);

        public bool IsBypassed(Uri host) =>
            Configuration.Instance.Capture ?
            false :
            Configuration.Instance.UpstreamProxy.IsBypassed(host);
    }
}
