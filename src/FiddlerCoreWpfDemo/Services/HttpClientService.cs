using FiddlerCoreWpfDemo.Infrastructure;
using System.Threading.Tasks;

namespace FiddlerCoreWpfDemo.Services
{
    public class HttpClientService : IHttpClientService
    {
        public async Task<string> GetAsync(string uri)
        {
            var response = await HttpClientConfig.HttpClient.GetAsync(uri).ConfigureAwait(false);

            return
$@"Request:
{response.RequestMessage.Method} {response.RequestMessage.RequestUri} {response.RequestMessage.Version}
{response.RequestMessage.Headers}
{response.RequestMessage.Content?.ReadAsStringAsync().Result}

Response:
{response.Version} {response.StatusCode}
{response.Headers}
{response.Content?.ReadAsStringAsync().Result}";
        }
    }
}
