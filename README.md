# FiddlerCoreWpfDemo

This is a demo project, which shows how FiddlerCore is used from a WPF application.

In the `App.xaml` file the `Application_Startup` and the `Application_Exit` methods are attached to the `Application.Startup` and `Application.Exit` events.

The `Application_Startup` and the `Application_Exit` methods are inside the `App` class from the `App.xaml.cs` file and their job is to initialize and dispose the `NetworkConnectionsConfig`, `FiddlerCoreConfig` and the `HttpClientConfig` classes from the `FiddlerCoreWpfDemo.Infrastructure` namespace.

There is also a `UseFiddlerCore` bool const in the `App` class, which could be toggled during development, in order to compare the performance of the application with and without FiddlerCore.

In the `Application_Startup` method a `networkConnectionsConfig` instance is configured first. During the initialization, the system proxy settings are being read from the WinINet API. Latter these proxy settings are passed to FiddlerCore in order to use them as an upstream proxy. This is how FiddlerCore is chaining to the current proxy chain, if any.

In the `NetworkConnectionsConfig` class there is a `UpstreamProxySettingsChanged` event, which could be used to regain connectivity if the system proxy is changed. This could be achieved by shutting down FiddlerCore and starting it again with the new upstream proxy settings.

The second thing which gets configured later is the `fiddlerCoreConfig` instance, which accepts the upstream proxy settings. In order for FiddlerCore to decrypt HTTPS traffic, it generates a X509 root certificate, which should be trusted by the user and it is stored in a `RootCertificate.p12` file for later usages.
In order for FiddlerCore to be able to write and read SAZ files, it needs an `ISAZProvider` implementation, which is set during the configuration. We are also attaching to the `FiddlerApplication.BeforeRequest` event in order to collect the HTTP/S messages, captured by FiddlerCore. Then the `fiddler.ui.rules.bufferresponses` preference is set in order to allow for response streaming which improves the performance.
