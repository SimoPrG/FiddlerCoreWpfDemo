using System.Threading.Tasks;

namespace FiddlerCoreWpfDemo.Services
{
    public interface IHttpClientService
    {
        Task<string> GetAsync(string uri);
    }
}
