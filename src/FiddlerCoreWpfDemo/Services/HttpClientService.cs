using System.Net.Http;
using System.Threading.Tasks;

namespace FiddlerCoreWpfDemo.Services
{
    public class HttpClientService : IHttpClientService
    {
        private readonly HttpClient httpClient = new HttpClient();

        public async Task<string> GetAsync(string uri)
        {
            var response = await this.httpClient.GetAsync(uri);

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
