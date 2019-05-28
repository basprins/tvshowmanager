using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using PerfectCode.Logging;

namespace PerfectCode.TvDbRestApi
{
    public class HttpThrottlingClient
    {
        private static readonly ILogger Log = new Logger(typeof(HttpThrottlingClient));

        private readonly string _apiUri;
        private readonly TimeSpan _timeout;
        private HttpClient _httpClient;

        /// <summary>
        /// Creates a http client which can be throttled by a specified amount of requests over a specified amount of time
        /// </summary>
        /// <param name="apiUri">the uri of the API</param>
        /// <param name="timeout">the timeout of the http client</param>
        public HttpThrottlingClient(string apiUri, TimeSpan timeout)
        {
            _apiUri = apiUri;
            _timeout = timeout;

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(apiUri),
                Timeout = timeout
            };
        }

        public async Task<HttpResponseMessage> GetAsync(string requestUri, string debugMessage)
        {
            for (var i = 1; i <= 3; i++)
            {
                try
                {
                    return await GetAsyncImpl(requestUri, debugMessage);
                }
                catch (Exception e)
                {
                    if (i >= 3)
                    {
                        Log.Debug($"Exception: {e}");

                        _httpClient = new HttpClient
                        {
                            BaseAddress = new Uri(_apiUri),
                            Timeout = _timeout
                        };

                        return await GetAsyncImpl(requestUri, debugMessage);
                    }

                    await Task.Delay(5000);
                }
            }

            return null;
        }

        private async Task<HttpResponseMessage> GetAsyncImpl(string requestUri, string debugMessage)
        {
            Log.Debug($"Requesting: '{_httpClient.BaseAddress}{requestUri}'");

            var result = await _httpClient.GetAsync(requestUri);

            if (!result.IsSuccessStatusCode)
            {
                Log.Error($"Request '{debugMessage}' resulted in error code '{result.StatusCode}'");
            }

            var throttleInfoAvailable = result.Headers.TryGetValues("X-RateLimit-Remaining", out var headers);

            if (throttleInfoAvailable)
            {
                var remainingRequests = int.Parse(headers.First());

                if (remainingRequests < 5)
                {
                    Log.Debug("Throttling by delaying task for 5000 ms");
                    await Task.Delay(5000);
                }
            }

            return result;
        }
    }
}