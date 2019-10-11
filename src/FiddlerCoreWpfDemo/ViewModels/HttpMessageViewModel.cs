using System;
using System.Windows.Input;
using FiddlerCoreWpfDemo.Behavior;
using FiddlerCoreWpfDemo.Services;

namespace FiddlerCoreWpfDemo.ViewModels
{
    public class HttpMessageViewModel : ViewModelBase
    {
        private readonly IHttpClientService httpClient;

        private ICommand get;
        private string response;

        public HttpMessageViewModel() : this(new HttpClientService())
        {
        }

        public HttpMessageViewModel(IHttpClientService httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public bool Capture
        {
            get;
            set;
        } = true;

        public string Url { get; set; } = "https://www.telerik.com/";

        public string Response
        {
            get
            {
                return this.response;
            }

            set
            {
                response = value;
                this.OnPropertyChanged(nameof(Response));
            }
        }

        public ICommand Get
        {
            get
            {
                if (get == null)
                {
                    this.get = new RelayCommand(this.GetHandler);
                }

                return get;
            }
        }

        private async void GetHandler(object parameter)
        {
            this.Response = "Loading...";
            this.Response = await this.httpClient.GetAsync(this.Url);
        }
    }
}
