# FiddlerCoreWpfDemo

This is a demo project, which shows how FiddlerCore is used from a WPF application, in order to capture the traffic from the whole application.

In the `app.config` file, inside the `configuration` element the following code is added:
```xml
<configuration>
  <!-- ... -->
  <system.net>
    <defaultProxy>
      <module type="AppProxy.FiddlerCoreWebProxy, AppProxy" />
    </defaultProxy>
  </system.net>
  <!-- ... -->
</configuration>
```

This makes the app to use a default proxy - the `AppProxy.FiddlerCoreWebProxy` class, implementing the `IWebProxy` interface, from the `AppProxy` assembly. The `GetProxy` method implementation returns the `Uri` with the IP and port pair where FiddlerCore listens for connections.

In the `App.xaml` file the `Application_Startup` and the `Application_Exit` methods are attached to the `Application.Startup` and `Application.Exit` events.

The `App` class from the `App.xaml.cs` file has a reference to a `Configuration.Instance` which configures the `NetworkConnectionsConfig`, `FiddlerCoreConfig` and the `HttpClientConfig` classes from the `Infrastructure` assembly.

In the `Configuration` class a `networkConnectionsConfig` instance is configured first. During the initialization, the system proxy settings are being read from the WinINet API. Latter these proxy settings are passed to FiddlerCore in order to use them as an upstream proxy. This is how FiddlerCore is chaining to the current proxy chain, if any.

In the `NetworkConnectionsConfig` class there is a `UpstreamProxySettingsChanged` event, which could be used to regain connectivity if the system proxy is changed. This could be achieved by shutting down FiddlerCore and starting it again with the new upstream proxy settings.

The second thing which gets configured later is the `fiddlerCoreConfig` instance, which accepts the upstream proxy settings. In order for FiddlerCore to decrypt HTTPS traffic, it generates a X509 root certificate, which should be trusted by the user and it is stored in a `RootCertificate.p12` file for later usages.
In order for FiddlerCore to be able to write and read SAZ files, it needs an `ISAZProvider` implementation, which is set during the configuration. We are also attaching to the `FiddlerApplication.BeforeRequest` event in order to collect the HTTP/S messages, captured by FiddlerCore. Then the `fiddler.ui.rules.bufferresponses` preference is set in order to allow for response streaming which improves the performance. Then we build a `FiddlerCoreStartupSettings` by passing the `upstreamProxySettings` arguments and we are also enabling the decryption of the SSL traffic and finally we `Startup` the `FiddlerApplication`.

On `Applicaiton.Exit` we dispose the `Configuration` instance last in order to make sure that we will capture all the traffic before exit.

The persistance of the HTTP/S sessions is taken care by the `SessionsPersister` class. Please, note that this only one of the many possible solutions how to write the HTTP/S traffic into files. Here, we are using a `sessions` list which is a collection of the captured sessions and for demo purposes we are saving all completed sessions into SAZ files and then we are removing these sessions from the `sessions` list in order release them for garbage collection. 