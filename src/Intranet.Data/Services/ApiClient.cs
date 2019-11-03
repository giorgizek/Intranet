using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Intranet.Model.Config;
using Microsoft.Extensions.Options;
using Zek;
using Zek.Extensions.Net;
using Zek.Utils;

namespace Intranet.Data.Services
{
    public interface IApiClient : IDisposable
    {
        Task<HttpResponseMessage> PostAsJsonAsync<T>(string requestUri, T value);
    }

    public class HttpApiClient : DisposableObject, IApiClient
    {
        private readonly ApiOptions _apiOptions;

        public HttpApiClient(ApiOptions apiOptions)
        {
            _apiOptions = apiOptions;
        }
        public HttpApiClient(IOptions<ApiOptions> apiOptionsAccessor) : this(apiOptionsAccessor.Value)
        {
        }


        private HttpClient _client;
        private HttpClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new HttpClient();
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Key", _apiOptions.Key);
                }
                return _client;
            }
        }


        public Task<HttpResponseMessage> PostAsJsonAsync<T>(string requestUri, T value) => Client.PostAsJsonAsync(_apiOptions.Url + requestUri, value);


        protected override void DisposeResources()
        {
            _client?.Dispose();
        }
    }
}
